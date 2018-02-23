using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using DataLayer;
using log4net;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plotting;
using WebApp.Data;
using WebApp.Extensions;
using WebApp.Models.Chart;
using WebApp.Models.Mappings;
using WebApp.Models.Plots;
using WebApp.Search.Extensions;
using WebApp.Search.Queries;
using WebApp.Services;

namespace WebApp.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/plots")]
    public class PlotController : ApiController
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int PointSizeOnOneCyclePlots = 10;
        private const int MaxSeriesPerPlot = 100;

        private readonly DqdvContext _db;
        private readonly ChartExporter _exporter;
        private readonly PlotTemplateService _plotTemplateService;
        private readonly SelectProjectListQuery _selectProjectListQuery;
        private readonly ChartSettingProvider _chartSettingProvider;

        ////////////////////////////////////////////////////////////
        // Constructors
        ////////////////////////////////////////////////////////////

        /// <inheritdoc />
        /// <summary>
        /// Initialize a new instance of <see cref="T:WebApp.Controller.Api.ChartTemplateController" />
        /// </summary>
        /// <param name="db"></param>
        /// <param name="plotTemplateService"></param>
        /// <param name="chartSettingProvider"></param>
        /// <param name="exporter"></param>
        /// <param name="selectProjectListQuery"></param>
        public PlotController(
            DqdvContext db,
            PlotTemplateService plotTemplateService,
            ChartSettingProvider chartSettingProvider,
            ChartExporter exporter,
            SelectProjectListQuery selectProjectListQuery)
        {
            _db = db;
            _plotTemplateService = plotTemplateService;
            _exporter = exporter;
            _selectProjectListQuery = selectProjectListQuery;
            _chartSettingProvider = chartSettingProvider;
        }

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        [Route]
        [HttpGet]
        public IHttpActionResult GetChartTemplates()
        {
            var timer = Stopwatch.StartNew();
            var templateUserId = _db.Users.First(u => u.UserName == "template@dqdv.com");

            var listContent = GetAllUserPlotTemplates(templateUserId.Id)
               .Select(pt => new { pt.Id, pt.Content })
               .ToList();
            var result = listContent.Select(c => {
                var template = JsonConvert.DeserializeObject<Dqdv.Types.Plot.PlotTemplate>(c.Content);
                template.Id = c.Id.ToString();
                return template;
            }).ToList();
            timer.Restart();
            return Ok(result);
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetChartTemplate(string id, [FromUri] int[] projectId, [FromUri] string format = null)
        {
            var result = _selectProjectListQuery.Execute(User.Identity.GetUserId(), projectId);
            if (result.Status != ProjectStatusDto.Ready)
            {
                return Content(HttpStatusCode.NotFound, new { ProjectStatus = result.Status });
            }

            var timer = Stopwatch.StartNew();

            int.TryParse(id, out var templateId);
            var chartTemplate = _plotTemplateService.GetChartTemplate(new ChartOwner(User.Identity.GetUserId(), User.Identity.Name), templateId, result.Projects);
            if (chartTemplate == null)
            {
                Log.Error($"Template {templateId} not found");
                return NotFound();
            }
            timer.Restart();

            if (format?.ToLower() == "xlsx")
            {
                return new ExcelHttpActionResult(_exporter.ToExcel(chartTemplate.Chart));
            }

            var dto = chartTemplate.Chart.ToChartDto($"Template - {chartTemplate.Template.Name}");
            Log.Info($"ChartController.GetChartByTemplate: Plot({chartTemplate.Template.Name}, {chartTemplate.Template.PlotParameters}, ProjectIds: {string.Join(",", projectId)} UTC Time: {DateTime.UtcNow} toDto {timer.ElapsedMilliseconds} ms");

            return Ok(dto);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult SaveChartTemplate(JObject templateModel)
        {
            var templateObj = templateModel.GetValue("templateModel").ToObject<PlotTemplateRequestModel>();
            var result = _selectProjectListQuery.Execute(User.Identity.GetUserId(), templateObj.projectIds);
            if (result.Status != ProjectStatusDto.Ready)
                return Content(HttpStatusCode.NotFound, new { result.Status });

            var templateUserId = _db.Users.First(u => u.UserName == "template@dqdv.com");
            var userId = HttpContext.Current.User.Identity.GetUserId();

            templateObj.newTemplate.UserId = userId;
            templateObj.newTemplate.CanEdit = userId != templateUserId.Id;

            templateObj.newTemplate.PlotParameters.PointSize = result.Projects.HasProjectsWithOneCycle() ? PointSizeOnOneCyclePlots : templateObj.newTemplate.PlotParameters.PointSize;
            templateObj.newTemplate.PlotParameters.MaxCycles = result.Projects.IsTotalProjectsCyclesGreaterThanMaximum(MaxSeriesPerPlot) ? MaxSeriesPerPlot : templateObj.newTemplate.PlotParameters.MaxCycles;

            templateObj.newTemplate.PlotParameters.CustomCycleFilter = HttpContext.Current.Server.UrlDecode(templateObj.newTemplate.PlotParameters.CustomCycleFilter);
            _chartSettingProvider.ClearSettings($"{User.Identity.GetUserId()}_Template");

            var parameters = templateObj.newTemplate.PlotParameters;
            if (string.IsNullOrEmpty(templateObj.newTemplate.Id))
            {
                var itemNew = _db.PlotTemplates.Add(new DataLayer.PlotTemplate
                {
                    UserId = User.Identity.GetUserId(),
                    Name = templateObj.newTemplate.Name,
                    Content = JsonConvert.SerializeObject(templateObj.newTemplate),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                _db.SaveChanges();
                templateObj.newTemplate.Id = itemNew.Id.ToString();
                itemNew.Content = JsonConvert.SerializeObject(templateObj.newTemplate);
                itemNew.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                int.TryParse(templateObj.newTemplate.Id, out var templateId);
                var templateToUpdate = GetAllUserPlotTemplates(templateUserId.Id).FirstOrDefault(pt => pt.Id == templateId);
                if (templateToUpdate != null)
                {
                    templateToUpdate.Content = JsonConvert.SerializeObject(templateObj.newTemplate);
                    templateToUpdate.UpdatedAt = DateTime.UtcNow;
                }
                else 
                {
                    Log.Error($"Template {templateId} not found");
                    return NotFound();
                }
            }

            _db.SaveChanges();

            var timer = Stopwatch.StartNew();
            var chartTemplate = _plotTemplateService.GetChartTemplate(new ChartOwner(User.Identity.GetUserId(), User.Identity.Name), int.Parse(templateObj.newTemplate.Id), result.Projects);
            var dto = chartTemplate.Chart.ToChartDto($"Template - {templateObj.newTemplate.Name}");
            Log.Info($"ChartController.SaveChartTemplate: Plot({templateObj.newTemplate.Name}, {parameters}, {templateObj.projectIds}): plot {timer.ElapsedMilliseconds} ms");

            return Ok(
                new
                {
                    selectedTemplate = templateObj.newTemplate,
                    chart = dto
                });
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteChartTemplate(int id)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var templateToDelete = _db.PlotTemplates.FirstOrDefault(pt => pt.UserId == userId && pt.Id == id);

            if (templateToDelete == null)
                return NotFound();

            var timer = Stopwatch.StartNew();

            _db.SP_DeleteTemplate(userId, id);
            _db.SaveChanges();
            Log.Info($"ChartController.DeleteChartTemplate: Plot({templateToDelete.Name} has been deleted. UTC Time: {DateTime.UtcNow}. Executing time: {timer.ElapsedMilliseconds} ms");

            return StatusCode(HttpStatusCode.NoContent);
        }

        ////////////////////////////////////////////////////////////
        // Private Methods/Atributes
        ////////////////////////////////////////////////////////////

        private IEnumerable<DataLayer.PlotTemplate> GetAllUserPlotTemplates(string templateUserId)
        {
            return _db.GetAllUserPlotTemplates(User.Identity.GetUserId(), templateUserId);
        }
    }
}
