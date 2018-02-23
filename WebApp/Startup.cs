/*
Copyright(c) <2018> <University of Washington>
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Autofac;
using Autofac.Integration.WebApi;
using DataLayer;
using Dqdv.External;
using Hangfire;
using log4net;
using log4net.Config;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.OData.Edm;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Newtonsoft.Json.Serialization;
using Owin;
using Plotting;
using WebApp;
using WebApp.Interfaces;
using WebApp.Services;
using GlobalConfiguration = Hangfire.GlobalConfiguration;
using Project = DataLayer.Project;
using Dqdv.External.Contracts.Azure;
using Dqdv.Internal.Contracts.Settings;
using Dqdv.Internal.Settings;
using Plotting.Plotters;
using WebApp.Search.Queries;

[assembly: OwinStartup(typeof(Startup))]

namespace WebApp
{
    class Startup
    {
        #region Logging

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public methods

        public void Configuration(IAppBuilder app)
        {
            try
            {
                XmlConfigurator.Configure();
                Log.Info("Startup.Configuration: Starting");

                var container = GetDependencyContainer(app);
                var config = new HttpConfiguration
                {
                    DependencyResolver = new AutofacWebApiDependencyResolver(container)
                };
                Configure(config);

                app.UseCookieAuthentication(
                    new CookieAuthenticationOptions
                    {
                        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                        Provider = new CookieAuthenticationProvider()
                    });
                app.UseAutofacMiddleware(container);
                app.UseAutofacWebApi(config);
                app.UseWebApi(config);

                RegisterColumnEncryptionKeyStoreProviders(container);
                MigrateDatabase();
                ConfigureHangfire();

                Log.Info("Startup.Configuration: Started");
            }
            catch (Exception ex)
            {
                Log.Error("Startup.Configuration: Unexpected exception during stratup", ex);
                throw;
            }
        }

        #endregion

        #region Private methods

        private static void Configure(HttpConfiguration config)
        {
            config.SetTimeZoneInfo(TimeZoneInfo.Utc);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Services.Add(typeof(IExceptionLogger), new WebApiExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new WebApiExceptionHandler());
            config.MapHttpAttributeRoutes();

            config.Select()
                .Expand()
                .Filter()
                .OrderBy()
                .MaxTop(null)
                .Count();
            config.MapODataServiceRoute("odata", "odata", GetODataModel());
        }

        private static IContainer GetDependencyContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // Singleton
            builder.RegisterInstance(app.GetDataProtectionProvider());
            builder.RegisterType<OptionSettings>().As<IOptions>().SingleInstance();
            builder.RegisterType<AzureActiveDirectorySettings>().As<IAzureActiveDirectorySettings>().SingleInstance();
            builder.RegisterType<ClientCredentialFactory>().As<IClientCredentialFactory>().SingleInstance();
            builder.RegisterType<AzureKeyVaultTokenProvider>().As<IAzureKeyVaultTokenProvider>().SingleInstance();
            builder.RegisterType<AzureSqlColumnEncryptionKeyStoreProvider>().As<IAzureSqlColumnEncryptionKeyStoreProvider>().SingleInstance();
            builder.RegisterType<CacheProvider>().As<ICacheProvider>().SingleInstance();
            
            // Scoped
            builder.Register(c => new DqdvContext(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, TimeSpan.FromMinutes(30))).InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().InstancePerRequest();
            builder.RegisterType<ApplicationSignInManager>().InstancePerRequest();
            builder.RegisterType<ProjectPlotCache>().AsSelf().InstancePerRequest();
            builder.RegisterType<ProjectService>().AsSelf().InstancePerRequest();
            builder.Register(c => new UserStore<AppUser>(c.Resolve<DqdvContext>())).As<IUserStore<AppUser>>().InstancePerRequest();
            builder.Register(c => c.Resolve<IOwinContext>().Authentication).As<IAuthenticationManager>().InstancePerRequest();
            builder.Register<IStorage>(c =>
            {
                var section = (NameValueCollection)ConfigurationManager.GetSection("storage");
                var type = section["Type"];
                switch (type)
                {
                    case "Local":
                        return new LocalStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, section["Directory"]));
                    case "Azure":
                        return new AzureStorage(section["ConnectionString"], section["Container"]);
                    default:
                        throw new ApplicationException($"Unknown storage type: {type}");
                }
            }).InstancePerRequest();
            builder.Register(c =>
            {
                var section = (NameValueCollection)ConfigurationManager.GetSection("feedback-storage");
                var type = section["Type"];
                switch (type)
                {
                    case "Local":
                        return new FeedbackStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, section["Directory"]));
                    case "Azure":
                        return new FeedbackStorage(section["ConnectionString"], section["Container"]);
                    default:
                        throw new ApplicationException($"Unknown feedback storage type: {type}");
                }
            }).InstancePerRequest();

            builder.RegisterType<PlotTemplateService>().AsSelf().InstancePerRequest();
            builder.RegisterType<ChartSettingProvider>().AsSelf().InstancePerRequest();
            
            builder.RegisterType<ResistanceChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.ResistanceOhms).InstancePerRequest();
            builder.RegisterType<CoulombicEfficiencyChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.CoulombicEfficiency).InstancePerRequest();
            builder.RegisterType<CyclicVoltammetryChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.CyclicVoltammetry).InstancePerRequest();
            builder.RegisterType<DifferentialCapacityChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.DifferentialCapacity).InstancePerRequest();
            builder.RegisterType<DifferentialVoltageChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.DifferentialVoltage).InstancePerRequest();
            builder.RegisterType<EndCapacityChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.EndCapacity).InstancePerRequest();
            builder.RegisterType<CapacityRetentionChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.CapacityRetention).InstancePerRequest();
            builder.RegisterType<EnergyChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.Energy).InstancePerRequest();
            builder.RegisterType<PowerChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.Power).InstancePerRequest();
            builder.RegisterType<EndCurrentChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.EndCurrent).InstancePerRequest();
            builder.RegisterType<EndTimeEndCurrentChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.EndTimeEndCurrent).InstancePerRequest();
            builder.RegisterType<EndTimeEndVoltageChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.EndTimeEndVoltage).InstancePerRequest();
            builder.RegisterType<EndVoltageChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.EndVoltage).InstancePerRequest();
            builder.RegisterType<MidVoltageChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.MidVoltage).InstancePerRequest();
            builder.RegisterType<CyclicVoltammetryChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.CyclicVoltammetry).InstancePerRequest();
            builder.RegisterType<VoltageCapacityChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.VoltageCapacity).InstancePerRequest();
            builder.RegisterType<SocChartPlotter>().As<ChartPlotterBase>().Keyed<ChartPlotterBase>(PlotType.Soc).InstancePerRequest();
            builder.RegisterType<TemplatePlotter>().AsSelf().InstancePerRequest();

            builder.RegisterType<SelectProjectListQuery>().AsSelf().InstancePerRequest();
            
            builder.Register<Func<PlotType, ChartPlotterBase>>(c =>
            {
                var componentContext = c.Resolve<IComponentContext>();
                return plotName => componentContext.ResolveKeyed<ChartPlotterBase>(plotName);
            });

            // Transient
            builder.RegisterType<ChartExporter>().InstancePerDependency();
            builder.RegisterType<ProjectDataRepository>().As<IProjectDataRepository>().InstancePerDependency();
            builder.RegisterType<BackgroundJobClient>().As<IBackgroundJobClient>().InstancePerDependency();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            
            return builder.Build();
        }

        private static IEdmModel GetODataModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EnableLowerCamelCase();

            var project = builder.EntitySet<Project>("projects").EntityType;
            project.Ignore(p => p.InternalFileName);
            project.Ignore(p => p.JobId);
            return builder.GetEdmModel();
        }

        private static void RegisterColumnEncryptionKeyStoreProviders(IContainer container)
        {
            var providerRegistration = container.Resolve<IAzureSqlColumnEncryptionKeyStoreProvider>();
            providerRegistration.Register();
        }

        private static void MigrateDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"];
            DqdvContext.Migrate(connectionString.ConnectionString, connectionString.ProviderName);
        }

        private static void ConfigureHangfire()
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("Default");
        }

        #endregion
    }
}
