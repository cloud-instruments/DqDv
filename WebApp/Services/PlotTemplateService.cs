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

using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DataLayer;
using Dqdv.Types.Plot;
using log4net;
using Newtonsoft.Json;
using Plotting;
using WebApp.Data;
using WebApp.Interfaces;
using WebApp.Search.Extensions;
using Project = DataLayer.Project;

namespace WebApp.Services
{

    public class TemplateTempSettings
    {
        public int Id { get; set; }
        public PlotParameters PlotParameters { get; set; }
    }

    public class PlotTemplateService
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int PointSizeOnOneCyclePlots = 10;
        private const int MaxSeriesPerPlot = 100;
        private const string GlobalTemplatesUser = "template@dqdv.com";

        private readonly DqdvContext _db;
        private readonly TemplatePlotter _templatePlotter;
        private readonly ChartSettingProvider _chartSettingProvider;
        private readonly ICacheProvider _cacheProvider;

        public PlotTemplateService(
            DqdvContext db,
            TemplatePlotter templatePlotter,
            ChartSettingProvider chartSettingProvider,
            ICacheProvider cacheProvider)
        {
            _db = db;
            _templatePlotter = templatePlotter;
            _chartSettingProvider = chartSettingProvider;
            _cacheProvider = cacheProvider;
        }

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        public ChartTemplate GetChartTemplate(ChartOwner owner, int templateId, IList<Project> projects)
        {
            var templateUserId = _db.Users.AsNoTracking().First(u => u.UserName == GlobalTemplatesUser);
            var selectedTemplate = _db.GetAllUserPlotTemplates(owner.Id, templateUserId.Id).FirstOrDefault(item => item.Id == templateId);
            if (selectedTemplate == null)
                return null;

            var template = JsonConvert.DeserializeObject<Dqdv.Types.Plot.PlotTemplate>(selectedTemplate.Content);
            var parameters = _chartSettingProvider.MergeWithGlobal(owner.Id, template.PlotParameters);
            //parameters = _chartSettingProvider.MergePlotParameters(parameters, _chartSettingProvider.GetSettings(owner.Id));
            var currentTemp = _cacheProvider.Get($"PlotParameters_{owner.Id}_Template") as TemplateTempSettings;
            var tempSettings = new TemplateTempSettings
            {
                Id = templateId,
                PlotParameters = parameters //_chartSettingProvider.MergePlotParameters(parameters, _chartSettingProvider.GetSettings(owner.Id))
            };
            if (currentTemp == null)
            {
                _cacheProvider.Set($"PlotParameters_{owner.Id}_Template", tempSettings);
                _chartSettingProvider.SetSettings(owner.Id, tempSettings.PlotParameters);

                parameters = tempSettings.PlotParameters;
            }
            else
            {
                if (currentTemp.Id != templateId)
                    _cacheProvider.Set($"PlotParameters_{owner.Id}_Template", tempSettings);
                else
                {
                    parameters = _chartSettingProvider.GetSettings(owner.Id);
                }
            }

            var projectWithOneCycleExist = projects.HasProjectsWithOneCycle();
            var cyclesMaxVerified = projects.IsTotalProjectsCyclesGreaterThanMaximum(MaxSeriesPerPlot);

            parameters.PointSize = projectWithOneCycleExist ? PointSizeOnOneCyclePlots : parameters.PointSize;
            parameters.MaxCycles = cyclesMaxVerified ? MaxSeriesPerPlot : parameters.MaxCycles;

            var timer = Stopwatch.StartNew();
            var chart = _templatePlotter.Plot(template, parameters, projects, owner.Name);
            Log.Info($"PlotTemplateService.GetChartTemplate: Plot({template.Name}, {parameters}): plot {timer.ElapsedMilliseconds} ms");

            chart.SelectedTemplateName = templateId.ToString();
            chart.PlotParameters = parameters;
            chart.PlotParameters.IsInitialized = true;
            
            return new ChartTemplate
            {
                Chart = chart,
                Template = template
            };
        }
    }

    public class ChartOwner
    {
        public ChartOwner(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }

    public class ChartTemplate
    {
        public Chart Chart { get; set; }

        public Dqdv.Types.Plot.PlotTemplate Template { get; set; }
    }
}
