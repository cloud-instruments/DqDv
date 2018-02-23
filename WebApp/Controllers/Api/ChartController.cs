using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using DataLayer;
using Dqdv.Types;
using Dqdv.Types.Plot;
using log4net;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using Plotting;
using WebApp.Extensions;
using WebApp.Interfaces;
using WebApp.Models.Chart;
using WebApp.Models.Mappings;
using WebApp.Search.Extensions;
using WebApp.Search.Queries;
using WebApp.Services;

namespace WebApp.Controllers.Api
{
    [Authorize]
    public class ChartController : ApiController
    {
        #region Logging

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Private fields
        private const int PointSizeOnOneCyclePlots = 10;
        private const int MaxSeriesPerPlot = 100;
        private readonly IStorage _storage;
        private readonly DqdvContext _db;
        private readonly ChartSettingProvider _chartSettingProvider;
        private readonly PlotTemplateService _plotTemplateService;
        private readonly Func<PlotType, ChartPlotterBase> _chartPlotterFactory;
        private readonly ChartExporter _exporter;
        private readonly SelectProjectListQuery _selectProjectListQuery;

        #endregion

        #region Constructor

        public ChartController(
            IStorage storage,
            DqdvContext db,
            ChartSettingProvider chartSettingProvider,
            PlotTemplateService plotTemplateService,
            Func<PlotType, ChartPlotterBase> chartPlotterFactory, 
            ChartExporter exporter,
            SelectProjectListQuery selectProjectListQuery)
        {
            _storage = storage;
            _db = db;
            _chartSettingProvider = chartSettingProvider;
            _chartPlotterFactory = chartPlotterFactory;
            _exporter = exporter;
            _selectProjectListQuery = selectProjectListQuery;
            _plotTemplateService = plotTemplateService;
        }

        #endregion

        #region Public methods

        [Route("api/chart")]
        [HttpGet]
        public IHttpActionResult GetChart(
            [FromUri] int[] projectId,
            [FromUri] PlotType plotType,
            [FromUri] string format = null)
        {
            Stopwatch timer = Stopwatch.StartNew();

            Log.Info($"ChartController.GetChart: ChartController.GetChart: Plot({plotType}, ProjectIds: {string.Join(",", projectId)} UTC Time: {DateTime.UtcNow}");
            var result = _selectProjectListQuery.Execute(User.Identity.GetUserId(), projectId);
            if (result.Status != ProjectStatusDto.Ready)
            {
                return Content(HttpStatusCode.NotFound, new { ProjectStatus = result.Status });
            }

            var parameters = _chartSettingProvider.MergeWithGlobal(User.Identity.GetUserId(), _chartSettingProvider.GetSettings(User.Identity.GetUserId()));

            var projectWithOneCycleExist = result.Projects.HasProjectsWithOneCycle();
            var projectsSumCyclesGreaterThanMax = result.Projects.IsTotalProjectsCyclesGreaterThanMaximum(MaxSeriesPerPlot);

            parameters.PointSize = projectWithOneCycleExist ? PointSizeOnOneCyclePlots : parameters.PointSize;
            parameters.MaxCycles = projectsSumCyclesGreaterThanMax ? MaxSeriesPerPlot : parameters.MaxCycles;

            var chart = _chartPlotterFactory(plotType).Plot(projectsSumCyclesGreaterThanMax, 
                new ChartPlotterContext
                {
                    Parameters = parameters,
                    ProjectIds = projectId,
                    Trace = User.Identity.Name
                });

            chart.PlotParameters = parameters;

            if (format?.ToLower() == "xlsx")
            {
                return new ExcelHttpActionResult(_exporter.ToExcel(chart));
            }

            var dto = chart.ToChartDto();
            Log.Info($"ChartController.GetChart: Plot({plotType}, {parameters}, ProjectIds: {string.Join(", ", projectId)} UTC Time: {DateTime.UtcNow} toDto {timer.ElapsedMilliseconds} ms");

            return Ok(dto);
        }

        [Route("api/setParameters"), HttpPost]
        public IHttpActionResult SetParameters(JObject parametersModel)
        {
            var parameters = _chartSettingProvider.GetSettings(User.Identity.GetUserId());
            var parametersObj = parametersModel.GetValue("parametersModel").ToObject<PlotParameters>();
            if (!parametersObj.IsInitialized)
            {
                parametersObj = parameters;
            }
            parametersObj.CustomCycleFilter = HttpContext.Current.Server.UrlDecode(parametersObj.CustomCycleFilter);
            parametersObj.IsInitialized = true;
            parametersObj = _chartSettingProvider.MergeWithGlobal(User.Identity.GetUserId(), parametersObj);

            _chartSettingProvider.SetSettings(User.Identity.GetUserId(), parametersObj);
            return Ok(parametersObj);
        }


        [Route("api/setStateOfCharge")]
        [HttpGet]
        public IHttpActionResult SetStateOfCharge(
            [FromUri] int[] projectId,
            [FromUri] double? chargeFrom = null,
            [FromUri] double? chargeTo = null)
        {
            var result = _selectProjectListQuery.Execute(User.Identity.GetUserId(), projectId);
            if (result.Status != ProjectStatusDto.Ready)
                return Content(HttpStatusCode.NotFound, new { result.Status });

            StateOfCharge stateOfCharge = new StateOfCharge {
                ChargeFrom = chargeFrom,
                ChargeTo = chargeTo
            };

            
            var parameters = _chartSettingProvider.MergeWithGlobal(User.Identity.GetUserId(), _chartSettingProvider.GetSettings(User.Identity.GetUserId()));
            var projectWithOneCycleExist = result.Projects.HasProjectsWithOneCycle();
            var projectsSumCyclesGreaterThanMax = result.Projects.IsTotalProjectsCyclesGreaterThanMaximum(MaxSeriesPerPlot);

            parameters.PointSize = projectWithOneCycleExist ? PointSizeOnOneCyclePlots : parameters.PointSize;
            parameters.MaxCycles = projectsSumCyclesGreaterThanMax ? MaxSeriesPerPlot : parameters.MaxCycles;

            var timer = Stopwatch.StartNew();

            var chart = _chartPlotterFactory(PlotType.Soc).Plot(projectsSumCyclesGreaterThanMax,
                new ChartPlotterContext
                {
                    Parameters = parameters, ProjectIds = projectId,
                    Trace = User.Identity.Name,
                    Data = stateOfCharge
                });

            chart.PlotParameters = parameters;
            timer.Restart();

            return Ok(chart.ToChartDto($"SOC ChargeFrom - {chargeFrom} ;ChargeTo - {chargeTo} "));
        }

        [Route("api/setDefaultParameters"), HttpPost]
        public IHttpActionResult SetDefaultParameters(JObject parametersModel)
        {
            var parametersObj = parametersModel.GetValue("parametersModel").ToObject<PlotParameters>();
            
            parametersObj.FromCycle = null;
            parametersObj.ToCycle = null;
            parametersObj.EveryNthCycle = null;
            parametersObj.CustomCycleFilter = string.Empty;
            parametersObj.LegendShowen = null;
            parametersObj.DisableCharge = null;
            parametersObj.DisableDischarge = null;
            parametersObj.Threshold = null;
            parametersObj.MinY = null;
            parametersObj.Simplification = 1;
            parametersObj.MaxY = null;
            parametersObj.CurrentUoM = CurrentUoM.mA;
            parametersObj.CapacityUoM = CapacityUoM.mAh;
            parametersObj.TimeUoM = TimeUoM.Seconds;
            parametersObj.PowerUoM = PowerUoM.W;
            parametersObj.EnergyUoM = EnergyUoM.Wh;
            parametersObj.ResistanceUoM = ResistanceUoM.Ohm;
            parametersObj.NormalizeBy = NormalizeBy.None;
            parametersObj.IsInitialized = true;
            parametersObj.AxisRange = new AxisRange();

            parametersObj = _chartSettingProvider.MergeWithGlobal(User.Identity.GetUserId(), parametersObj);
            _chartSettingProvider.SetSettings(User.Identity.GetUserId(), parametersObj);
            _chartSettingProvider.ClearSettings($"{User.Identity.GetUserId()}_Template");

            return Ok(parametersObj);
        }

        [Route("api/exportchart"), HttpPost]
        public IHttpActionResult GetExportChart(
            [FromUri] int[] projectId,
            [FromUri] PlotType plotType,
            [FromUri] string format = null,
            [FromUri] string templateIdValue = null,
            [FromUri] int? maxCycles = null,
            [FromUri] int? maxPointsPerSeries = null,
            [FromUri] int? fromCycle = null,
            [FromUri] int? toCycle = null,
            [FromUri] int? everyNthCycle = null,
            [FromUri] string customCycleFilter = null,
            [FromUri] bool? disableCharge = null,
            [FromUri] bool? disableDischarge = null,
            [FromUri] double? threshold = null,
            [FromUri] double? minY = null,
            [FromUri] double? maxY = null,
            [FromUri] CurrentUoM? currentUoM = null,
            [FromUri] CapacityUoM? capacityUoM = null,
            [FromUri] TimeUoM? timeUoM = null,
            [FromUri] PowerUoM? powerUoM = null,
            [FromUri] EnergyUoM? energyUoM = null,
            [FromUri] ResistanceUoM? resistanceUoM = null,
            [FromUri] NormalizeBy? normalizeBy = null,
            [FromUri] int? pointSize = null,
            [FromUri] double? chargeFrom = null,
            [FromUri] double? chargeTo = null)
        {
            var result = _selectProjectListQuery.Execute(User.Identity.GetUserId(), projectId);
            if (result.Status != ProjectStatusDto.Ready)
            {
                return NotFound();
            }

            var timer = Stopwatch.StartNew();
            Chart chart;

            if ((plotType == PlotType.View || plotType == PlotType.Template ) && !string.IsNullOrEmpty(templateIdValue))
            {
                int.TryParse(templateIdValue, out var templateId);
                var item = _plotTemplateService.GetChartTemplate(new ChartOwner(User.Identity.GetUserId(), User.Identity.Name), templateId, result.Projects);
                if (item == null)
                {
                    Log.Error($"Template {templateId} not found");
                    return NotFound();
                }
                chart = item.Chart;

                timer.Restart();
            }
            else if (plotType == PlotType.Soc)
            {
                var parameters = new PlotParameters
                {
                    MaxCycles = maxCycles,
                    MaxPointsPerSeries = maxPointsPerSeries,
                    FromCycle = fromCycle,
                    ToCycle = toCycle,
                    EveryNthCycle = everyNthCycle,
                    CustomCycleFilter = customCycleFilter,
                    DisableCharge = disableCharge,
                    DisableDischarge = disableDischarge,
                    Threshold = threshold,
                    MinY = minY,
                    MaxY = maxY,
                    CurrentUoM = currentUoM,
                    CapacityUoM = capacityUoM,
                    TimeUoM = timeUoM,
                    PowerUoM = powerUoM,
                    EnergyUoM = energyUoM,
                    ResistanceUoM = resistanceUoM,
                    NormalizeBy = normalizeBy,
                    PointSize = pointSize
                };

                StateOfCharge stateOfCharge = new StateOfCharge
                {
                    ChargeFrom = chargeFrom,
                    ChargeTo = chargeTo
                };

                chart = _chartPlotterFactory(PlotType.Soc).Plot(false,
                    new ChartPlotterContext
                    {
                        Parameters = parameters, ProjectIds = projectId,
                        Trace = User.Identity.Name,
                        Data = stateOfCharge
                    });

                chart.PlotParameters = parameters;
                Log.Info($"ChartController.GetExportChart: Plot({plotType}, {parameters}, {projectId}): plot {timer.ElapsedMilliseconds} ms");
            }
            else
            {
                PlotParameters parameters = new PlotParameters
                {
                    MaxCycles = maxCycles,
                    MaxPointsPerSeries = maxPointsPerSeries,
                    FromCycle = fromCycle,
                    ToCycle = toCycle,
                    EveryNthCycle = everyNthCycle,
                    CustomCycleFilter = customCycleFilter,
                    DisableCharge = disableCharge,
                    DisableDischarge = disableDischarge,
                    Threshold = threshold,
                    MinY = minY,
                    MaxY = maxY,
                    CurrentUoM = currentUoM,
                    CapacityUoM = capacityUoM,
                    TimeUoM = timeUoM,
                    PowerUoM = powerUoM,
                    EnergyUoM = energyUoM,
                    ResistanceUoM = resistanceUoM,
                    NormalizeBy = normalizeBy,
                    PointSize = pointSize
                };

                chart = _chartPlotterFactory(plotType).Plot(true,
                    new ChartPlotterContext
                    {
                        Parameters = parameters,
                        ProjectIds = projectId,
                        Trace =  User.Identity.Name
                    });

                Log.Info($"ChartController.GetExportChart: Plot({plotType}, {parameters}, {projectId}): plot {timer.ElapsedMilliseconds} ms");

                chart.PlotParameters = parameters;
            }

            var files = HttpContext.Current.Request.Files;
            Stream imageStream = null;
            if (files.Count > 0)
            {
                imageStream = files[0].InputStream;
            }

            return new ExcelHttpActionResult(_exporter.ToExcel(chart, imageStream));
        }


        [Route("api/shareProject"), HttpPost]
        public IHttpActionResult ShareProject(JObject parametersModel)
        {
            var parameters = parametersModel.GetValue("parametersModel").ToObject<ShareRequestModel>();

            var userId = User.Identity.GetUserId();

            var shareUser = _db.Users.FirstOrDefault(u => u.Email == parameters.email);
            if (shareUser == null)
                return NotFound();
            if (parameters.objectIds.Count == 0)
                return NotFound();

            parameters.objectIds.ForEach(pId =>
            {
                try
                {
                    _db.SP_ShareProject(userId, shareUser.Id, pId);
                }
                catch
                {
                    //row already exists               
                }
            });         
            return Ok();
        }

        [Route("api/shareTemplate"), HttpPost]
        public IHttpActionResult ShareTemplate(JObject parametersModel)
        {
            var parameters = parametersModel.GetValue("parametersModel").ToObject<ShareRequestModel>();

            var userId = User.Identity.GetUserId();

            var shareUser =  _db.Users.FirstOrDefault(u => u.Email == parameters.email);
            if(shareUser == null)
                return NotFound();
            if(parameters.objectIds.Count==0)
                return NotFound();

            _db.SP_ShareTemplate(userId, shareUser.Id, parameters.objectIds.First());

            return Ok();
        }

        [Route("api/shareView"), HttpPost]
        public IHttpActionResult ShareView(JObject parametersModel)
        {
            var parameters = parametersModel.GetValue("parametersModel").ToObject<ShareRequestModel>();

            var userId = User.Identity.GetUserId();

            var shareUser = _db.Users.FirstOrDefault(u => u.Email == parameters.email);
            if (shareUser == null)
                return NotFound();
            if (parameters.objectIds.Count == 0)
                return NotFound();

            _db.SP_ShareView(userId, shareUser.Id, parameters.objectIds.First());

            return Ok();
        }

        #endregion
    }
}
