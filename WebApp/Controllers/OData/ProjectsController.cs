using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using DataLayer;
using Dqdv.Internal.Contracts.Settings;
using Hangfire;
using Messages;
using Microsoft.AspNet.Identity;
using WebApp.Data;
using WebApp.Extensions;
using WebApp.Search;

namespace WebApp.Controllers.OData
{
    [Authorize]
    public class ProjectsController : ODataController
    {
        #region Constants

        private static readonly HashSet<string> NonUpdatableProperties = new HashSet<string>
        {
            "Id",
            "TraceId",
            "InternalFileName",
            "FileSize",
            "CreatedAt",
            "UpdatedAt",
            "IsReady",
            "Failed",
            "Error",
            "Owner",
            "FileName",
            "NumCycles",
            "JobId",
            "StitchedFrom",
            "StitchedFromNames"
        };

        #endregion

        #region Private fields

        private readonly DqdvContext _db;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IOptions _options;

        #endregion

        #region Constructor

        public ProjectsController(DqdvContext db, IBackgroundJobClient jobClient, IOptions options)
        {
            _db = db;
            _jobClient = jobClient;
            _options = options;
        }

        #endregion

        #region Public methods

        [EnableQuery(AllowedFunctions = AllowedFunctions.AllFunctions, EnsureStableOrdering = false)]
        public IEnumerable<Project> Get(string customQuery)
        {
            IEnumerable<Project> query = GetAllUserProjects();

            if (!string.IsNullOrWhiteSpace(customQuery))
            {
                Expression<Func<DqdvContext, Project, bool>> predicate = ParseCustomQuery(customQuery);
                query = query.Where(p => predicate.Invoke(_db, p));
            }

            return query.OrderByDescending(p => p.Id);
        }

        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var project = GetAllUserProjects().SingleOrDefault(p => p.Id == key);
            if (project == null)
                return NotFound();
            var userId = HttpContext.Current.User.Identity.GetUserId();

            var traceId = project.TraceId.ToString();

            _db.SP_DeleteProject(userId, key);
            _db.SaveChanges();

            if (!string.IsNullOrWhiteSpace(project.JobId) && _jobClient.Delete(project.JobId))
            {
                _jobClient.ContinueWith<IBackgroundProcessor>(project.JobId, p => p.DeleteProjectData(traceId, project.Id), JobContinuationOptions.OnAnyFinishedState);
                if (project.InternalFileName != null)
                {
                    if (_options.RawFileRetentionPeriod > TimeSpan.Zero)
                    {
                        _jobClient.Schedule<IBackgroundProcessor>(p => p.DeleteRawFile(traceId, project.Id, project.InternalFileName), _options.RawFileRetentionPeriod);
                    }
                    else
                    {
                        _jobClient.ContinueWith<IBackgroundProcessor>(project.JobId, p => p.DeleteRawFile(traceId, project.Id, project.InternalFileName), JobContinuationOptions.OnAnyFinishedState);
                    }
                }
            }
            else
            {
                _jobClient.Enqueue<IBackgroundProcessor>(p => p.DeleteProjectData(traceId, project.Id));
                if (project.InternalFileName != null)
                {
                    if (_options.RawFileRetentionPeriod > TimeSpan.Zero)
                    {
                        _jobClient.Schedule<IBackgroundProcessor>(p => p.DeleteRawFile(traceId, project.Id, project.InternalFileName), _options.RawFileRetentionPeriod);
                    }
                    else
                    {
                        _jobClient.Enqueue<IBackgroundProcessor>(p => p.DeleteRawFile(traceId, project.Id, project.InternalFileName));
                    }
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        public IHttpActionResult Patch([FromODataUri] int key, Delta<Project> delta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = GetAllUserProjects().SingleOrDefault(p => p.Id == key);
            if (project == null)
                return NotFound();

            if (delta.GetChangedPropertyNames().Intersect(NonUpdatableProperties).Any())
                return BadRequest();

            delta.Patch(project);
            project.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Updated(project);
        }

        #endregion

        #region Private methods

        private IQueryable<Project> GetProjects()
        {
            return _db.GetProjects(User.Identity.GetUserId());
        }

        private IEnumerable<Project> GetAllUserProjects()
        {
            return _db.GetAllUserProjects(User.Identity.GetUserId());
        }

        private Expression<Func<DqdvContext, Project, bool>> ParseCustomQuery(string customQuery)
        {
            try
            {
                return new QueryParser().Parse(customQuery);
            }
            catch (QueryParserException ex)
            {
                var message = $"Syntax error at postion {ex.CharPosition + 1}: {ex.Message}";
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError(message)));
            }
        }

        #endregion
    }
}
