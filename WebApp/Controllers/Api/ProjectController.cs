using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DataLayer;
using Dqdv.Internal.Contracts.Settings;
using Hangfire;
using log4net;
using LinqKit;
using Messages;
using Microsoft.AspNet.Identity;
using WebApp.Data;
using WebApp.Extensions;
using WebApp.Interfaces;
using WebApp.Models.Projects;
using WebApp.Services;

namespace WebApp.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/projects")]
    public class ProjectController : ApiController
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IStorage _storage;
        private readonly DqdvContext _db;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IOptions _options;
        private readonly ProjectService _projectService;

        #region Constructor

        public ProjectController(
            IStorage storage,
            DqdvContext db,
            ProjectService projectService,
            IBackgroundJobClient jobClient,
            IOptions options)
        {
            _storage = storage;
            _db = db;
            _jobClient = jobClient;
            _options = options;
            _projectService = projectService;
        }

        #endregion

        #region Public methods

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> Create()
        {
            var model = BindCreateProjectModel();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model.Files.Any(file => ExtractFileExtension(file) != ".xls" && ExtractFileExtension(file) != ".xlsx"))
            {
                return BadRequest("Only Excel files are accepted");
            }

            foreach (var file in model.Files)
            {
                string internalFileName = Guid.NewGuid().ToString();
                await _storage.SaveAsAsync(file, internalFileName);

                Project newProject = MapToProject(model, file, internalFileName);

                Project existingProject = await _db.Projects.FirstOrDefaultAsync(
                    item =>
                    item.FileName == newProject.FileName &&
                    item.Name == newProject.Name);

                if (existingProject == null)
                {
                    await _projectService.CreateProject(newProject);
                }
                else
                {
                    DateTime now = DateTime.UtcNow;
                    existingProject.TraceId = newProject.TraceId;
                    existingProject.InternalFileName = newProject.InternalFileName;
                    existingProject.FileSize = newProject.FileSize;
                    existingProject.TestName = newProject.TestName;
                    existingProject.TestType = newProject.TestType;
                    existingProject.Channel = newProject.Channel;
                    existingProject.Tag = newProject.Tag;
                    existingProject.CreatedAt = now;
                    existingProject.UpdatedAt = now;
                    existingProject.Comments = newProject.Comments;

                    //Do not override populated properties with empty while update
                    if (newProject.Mass != null && newProject.Mass != double.NaN)
                    {
                        existingProject.Mass = newProject.Mass;
                    }
                    if (newProject.Area != null && newProject.Area != double.NaN)
                    {
                        existingProject.Area = newProject.Area;
                    }
                    if (newProject.ActiveMaterialFraction != null && newProject.ActiveMaterialFraction != double.NaN)
                    {
                        existingProject.ActiveMaterialFraction = newProject.ActiveMaterialFraction;
                    }
                    if (newProject.TheoreticalCapacity != null && newProject.TheoreticalCapacity != double.NaN)
                    {
                        existingProject.TheoreticalCapacity = newProject.TheoreticalCapacity;
                    }

                    await _projectService.UpdateProject(existingProject);
                }
            }

            return Ok();
        }

        [Route("unique")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckProjectExist(IEnumerable<ProjectExistCheckModel> checkModel)
        {
            IQueryable<Project> query = _db.GetProjects(User.Identity.GetUserId());

            ExpressionStarter<Project> predicate = PredicateBuilder.New<Project>(false);
            predicate = checkModel.Aggregate(
                predicate,
                (current, cm) => current.Or(p => p.FileSize == cm.FileSize && p.FileName == cm.FileName && p.Name == cm.Name));

            var exists = await query.AnyAsync(predicate);

            return Ok(new { isAllUnique = !exists });
        }
        
        [Route("average")]
        [HttpPost]
        public async Task<IHttpActionResult> Average(AverageProjectsModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (!AllProjectsExist(model.Projects))
                return NotFound();

            var now = DateTime.UtcNow;
            var project = new Project
            {
                UserId = User.Identity.GetUserId(),
                TraceId = Guid.NewGuid(),
                Name = model.Name,
                FileName = FormatStitchedFrom(model.Projects),
                TestName = model.TestName,
                TestType = model.TestType,
                Channel = model.Channel,
                Tag = model.Tag,
                Mass = model.Mass,
                Area = model.Area,
                Comments = $"{model.Comments} {FormatStitchedFromNames(model.Projects)}",
                IsAveragePlot = true,
                CreatedAt = now,
                UpdatedAt = now,
                StitchedFromNames = FormatStitchedFromNames(model.Projects),
            };
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            var traceId = project.TraceId.ToString();

            project.JobId = _jobClient.Enqueue<IBackgroundProcessor>(
                p => p.PrepareAverageProject(
                    traceId,
                    project.Id,
                    model.Projects,
                    JobCancellationToken.Null));

            await _db.SaveChangesAsync();

            var timeoutJobId = _jobClient.Schedule<IBackgroundProcessor>(
                p => p.HandleTimeout(traceId, project.Id),
                _options.ProjectPrepareTimeout);

            _jobClient.ContinueWith<IBackgroundProcessor>(project.JobId,
                p => p.CancelTimeout(traceId, project.Id, timeoutJobId),
                JobContinuationOptions.OnAnyFinishedState);

            return Ok();
        }

        [Route("stitch")]
        [HttpPost]
        public IHttpActionResult Stitch([FromBody] StitchProjectsModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!AllProjectsExist(model.Projects))
                return NotFound();

            var now = DateTime.UtcNow;
            var project = new Project
            {
                UserId = User.Identity.GetUserId(),
                TraceId = Guid.NewGuid(),
                Name = model.Name,
                FileName = FormatStitchedFrom(model.Projects),
                TestName = model.TestName,
                TestType = model.TestType,
                Channel = model.Channel,
                Tag = model.Tag,
                Mass = model.Mass,
                Area = model.Area,
                Comments = model.Comments,
                StitchedFrom = FormatStitchedFrom(model.Projects),
                StitchedFromNames = FormatStitchedFromNames(model.Projects),
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Projects.Add(project);
            _db.SaveChanges();

            var traceId = project.TraceId.ToString();

            project.JobId = _jobClient.Enqueue<IBackgroundProcessor>(
                p => p.StitchProjects(
                    traceId,
                    project.Id,
                    model.Projects,
                    model.TryMergeAdjacentCycles,
                    JobCancellationToken.Null));

            _db.SaveChanges();

            var timeoutJobId = _jobClient.Schedule<IBackgroundProcessor>(
                p => p.HandleTimeout(traceId, project.Id),
                _options.ProjectPrepareTimeout);

            _jobClient.ContinueWith<IBackgroundProcessor>(project.JobId,
                p => p.CancelTimeout(traceId, project.Id, timeoutJobId),
                JobContinuationOptions.OnAnyFinishedState);

            return Ok();
        }

        [Route("{id}/download")]
        [HttpPost]
        public async Task<HttpResponseMessage> DownloadProject(int id)
        {
            //DownloadRequestModel parameters = parametersModel.GetValue("parametersModel").ToObject<DownloadRequestModel>();
            var userId = User.Identity.GetUserId();
            try
            {
                var project = await _db.GetProjects(userId).FirstAsync(p => p.Id == id);
                var result = await _storage.DownloadToStreamAsync(project.FileName, project.InternalFileName);
                if (result == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                // Reset the stream position; otherwise, download will not work
                result.BlobStream.Position = 0;

                // Set content headers
                var content = new StreamContent(result.BlobStream);
                content.Headers.ContentLength = result.BlobLength;
                content.Headers.ContentType = new MediaTypeHeaderValue(result.BlobContentType);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"\"{result.BlobFileName}\"",
                    Size = result.BlobLength
                };

                // Create response message with blob stream as its content
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }
        }

        #endregion

        #region Private methods

        private CreateProjectModel BindCreateProjectModel()
        {
            var model = new CreateProjectModel
            {
                Name = HttpContext.Current.Request.Form["name"],
                TestName = HttpContext.Current.Request.Form["testName"],
                TestType = HttpContext.Current.Request.Form["testType"],
                Channel = HttpContext.Current.Request.Form["channel"],
                Tag = HttpContext.Current.Request.Form["tag"],
                Mass = HttpContext.Current.Request.Form["mass"].ToDouble(),
                Area = HttpContext.Current.Request.Form["area"].ToDouble(),
                Comments = HttpContext.Current.Request.Form["comments"],
                ActiveMaterialFraction = HttpContext.Current.Request.Form["activeMaterialFraction"].ToDouble(),
                TheoreticalCapacity = HttpContext.Current.Request.Form["theoreticalCapacity"].ToDouble(),
                OverwriteExisting = HttpContext.Current.Request.Form["overwriteExisting"].ToBoolean()
            };

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("name", "'Name' is required");

            var files = HttpContext.Current.Request.Files;
            if (files.Count <= 0)
                ModelState.AddModelError("file", "Please select at least one file");
            else
                model.Files = new List<HttpPostedFile>(files.GetMultiple("file"));

            return model;
        }

        private static string ExtractFileExtension(HttpPostedFile file)
        {
            try
            {
                return Path.GetExtension(file.FileName).ToLowerInvariant();
            }
            catch (ArgumentException)
            {
                return string.Empty;
            }
        }

        private static string ExtractFileName(HttpPostedFile file)
        {
            try
            {
                return Path.GetFileName(file.FileName);
            }
            catch (ArgumentException)
            {
                return file.FileName;
            }
        }

        private bool AllProjectsExist(int[] projectId)
        {
            var foundProjects = _db.GetProjects(User.Identity.GetUserId())
                .Count(p => p.IsReady && !p.Failed && projectId.Contains(p.Id));
            return foundProjects == projectId.Length;
        }

        private static string FormatStitchedFrom(int[] projects)
        {
            return string.Join(", ", projects.Select(id => id.ToString(CultureInfo.InvariantCulture)));
        }

        private string FormatStitchedFromNames(int[] projects)
        {
            var names = _db.GetProjects(User.Identity.GetUserId())
                .Where(p => p.IsReady && !p.Failed && projects.Contains(p.Id))
                .ToDictionary(p => p.Id, p => p.Name);

            return string.Join(", ", projects.Select(id => names[id]));
        }

        private Project MapToProject(CreateProjectModel model, HttpPostedFile file, string internalFileName)
        {
            return new Project
            {
                UserId = User.Identity.GetUserId(),
                TraceId = Guid.NewGuid(),
                Name = model.Name,
                FileName = ExtractFileName(file),
                InternalFileName = internalFileName,
                FileSize = file.ContentLength,
                TestName = model.TestName,
                TestType = model.TestType,
                Channel = model.Channel,
                Tag = model.Tag,
                Mass = model.Mass,
                TheoreticalCapacity = model.TheoreticalCapacity,
                ActiveMaterialFraction = model.ActiveMaterialFraction,
                Area = model.Area,
                Comments = model.Comments,
            };
        }

        #endregion
    }
}
