using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DataLayer;
using log4net;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using WebApp.Data;
using WebApp.Extensions;
using WebApp.Models.Chart;
using WebApp.Models.Mappings;
using WebApp.Models.Projects;
using WebApp.Search.Queries;
using WebApp.Services;

namespace WebApp.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/views")]
    public class ViewController : ApiController
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DqdvContext _db;
        private readonly SelectProjectListQuery _selectProjectListQuery;
        private readonly PlotTemplateService _plotTemplateService;
        private readonly ChartExporter _exporter;

        ////////////////////////////////////////////////////////////
        // Constructors
        ////////////////////////////////////////////////////////////

        /// <inheritdoc />
        /// <summary>
        /// Initialize a new instance of <see cref="T:WebApp.Controller.Api.ViewController" />
        /// </summary>
        /// <param name="db"></param>
        /// <param name="plotTemplateService"></param>
        /// <param name="exporter"></param>
        /// <param name="selectProjectListQuery"></param>
        public ViewController(
            DqdvContext db,
            PlotTemplateService plotTemplateService,
            ChartExporter exporter,
            SelectProjectListQuery selectProjectListQuery)
        {
            _db = db;
            _plotTemplateService = plotTemplateService;
            _exporter = exporter;
            _selectProjectListQuery = selectProjectListQuery;
        }

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetViews()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var result = await _db.Views.Where(v => v.UserId == userId).Select(
                v => new
                {
                    id = v.Id,
                    name = v.Name,
                    comments = v.Comments,
                    template = v.PlotTemplate.Name,
                    projectsCount = v.Projects.Count,
                    projects = v.Projects.Select(p => p.Id).ToList(),
                    createdAt = v.CreatedAt,
                    updatedAt = v.UpdatedAt
                }).ToListAsync();

            return Ok(result);
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetViewChart(int id, [FromUri] string format = null)
        {
            var timer = Stopwatch.StartNew();
            var userId = User.Identity.GetUserId();

            var selectedView = _db.Views.First(v => v.UserId == userId && v.Id == id);
            if (selectedView == null)
                return NotFound();

            var projectIds = selectedView.Projects.Select(p => p.Id).ToArray();
            var result = _selectProjectListQuery.Execute(User.Identity.GetUserId(), projectIds);
            if (result.Status != ProjectStatusDto.Ready)
            {
                return Content(HttpStatusCode.NotFound, new { result.Status });
            }

            var chartTemplate = _plotTemplateService.GetChartTemplate(new ChartOwner(User.Identity.GetUserId(), User.Identity.Name), selectedView.PlotTemplateId, result.Projects);
            if (chartTemplate == null)
            {
                Log.Error($"Template {id} not found");
                return NotFound();
            }

            if (format?.ToLower() == "xlsx")
            {
                return new ExcelHttpActionResult(_exporter.ToExcel(chartTemplate.Chart));
            }

            var dto = chartTemplate.Chart.ToChartDto($"View - {chartTemplate.Template.Name}");
            Log.Info($"View({selectedView.Id}, {selectedView.PlotTemplateId}): toDto {timer.ElapsedMilliseconds} ms");
            return Ok(dto);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult AddView(JObject model)
        {
            var templateObj = model.GetValue("params").ToObject<ViewProjectsModel>();
            if (templateObj == null)
            {
                return BadRequest();
            }

            if (!AllProjectsExist(templateObj.Projects))
            {
                return NotFound();
            }
            var userId = HttpContext.Current.User.Identity.GetUserId();

            var now = DateTime.UtcNow;
            var view = new View
            {
                UserId = userId,
                Name = templateObj.Name,
                PlotTemplateId = templateObj.PlotTemplateId,
                Comments = templateObj.Comments,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Views.Add(view);
            _db.SaveChanges();

            view.Projects = new List<Project>();

            foreach (var projectId in templateObj.Projects)
            {
                view.Projects.Add(_db.Projects.First(p => p.Id == projectId));
            }

            _db.SaveChanges();

            return Ok(_db.Views.Where(v => v.UserId == userId).Select(
                v => new
                {
                    id = v.Id,
                    comments = v.Comments,
                    name = v.Name,
                    template = v.PlotTemplate.Name,
                    projectsCount = v.Projects.Count,
                    projects = v.Projects.Select(p => p.Id).ToList()
                }));
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteView(int id)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var selectedView = await _db.Views.FirstOrDefaultAsync(v => v.UserId == userId && v.Id == id);
            if (selectedView == null)
                return NotFound();

            _db.Views.Remove(selectedView);
            _db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        ////////////////////////////////////////////////////////////
        // Private Methods/Atributes
        ////////////////////////////////////////////////////////////

        private bool AllProjectsExist(int[] projectId)
        {
            var foundProjects = _db.GetProjects(User.Identity.GetUserId())
                .Count(p => p.IsReady && !p.Failed && projectId.Contains(p.Id));
            return foundProjects == projectId.Length;
        }
    }
}
