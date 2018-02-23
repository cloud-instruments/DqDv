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
using System.Threading.Tasks;
using DataLayer;
using Dqdv.Internal.Contracts.Settings;
using Hangfire;
using Messages;

namespace WebApp.Services
{
    /// <summary>
    /// Project service
    /// </summary>
    public class ProjectService
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private readonly DqdvContext _db;
        private readonly ProjectPlotCache _projectPlotCache;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IOptions _options;

        ////////////////////////////////////////////////////////////
        // Constructors
        ////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize a new instance of <see cref="ProjectService"/>
        /// </summary>
        /// <param name="db">Instance of <see cref="DqdvContext"/></param>
        /// <param name="projectPlotCache">Instance of <see cref="ProjectPlotCache"/></param>
        /// <param name="jobClient">Instance of <see cref="IBackgroundJobClient"/></param>
        /// <param name="options">Background job options</param>
        public ProjectService(
            DqdvContext db,
            ProjectPlotCache projectPlotCache,
            IBackgroundJobClient jobClient,
            IOptions options)
        {
            _db = db;
            _projectPlotCache = projectPlotCache;
            _jobClient = jobClient;
            _options = options;
        }

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="project">Project to be created</param>
        /// <returns></returns>
        public async Task CreateProject(Project project)
        {
            var dateTime = DateTime.UtcNow;

            project.CreatedAt = dateTime;
            project.UpdatedAt = dateTime;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            await StartProjectProcessingJobAsync(project);
        }

        /// <summary>
        /// Update project data
        /// </summary>
        /// <param name="project">Project to update</param>
        /// <returns></returns>
        public async Task UpdateProject(Project project)
        {
            project.UpdatedAt = DateTime.UtcNow;
            project.IsReady = false;

            await _db.SaveChangesAsync();

            _projectPlotCache.FlushProject(project.Id); // removes cached data for a given project
            StopProjectProcessingJob(project);
            await StartProjectProcessingJobAsync(project);
        }

        public Task Stitch()
        {
            //ToDo: move logic from a controller
            throw new NotImplementedException();
        }

        ////////////////////////////////////////////////////////////
        // Private Methods/Atributes
        ////////////////////////////////////////////////////////////

        private async Task StartProjectProcessingJobAsync(Project entity)
        {
            await PushProjectToProcessingQueue(entity);
            ScheduleProjectForProcessing(entity);
        }

        private async Task PushProjectToProcessingQueue(Project entity)
        {
            var jobId = _jobClient.Enqueue<IBackgroundProcessor>(p => p.PrepareProject(entity.TraceId.ToString(), entity.Id, JobCancellationToken.Null));
            entity.JobId = jobId;

            await _db.SaveChangesAsync();
        }

        private void ScheduleProjectForProcessing(Project entity)
        {
            var timeoutJobId = _jobClient.Schedule<IBackgroundProcessor>(p => p.HandleTimeout(entity.TraceId.ToString(), entity.Id), _options.ProjectPrepareTimeout);
            _jobClient.ContinueWith<IBackgroundProcessor>(entity.JobId, p => p.CancelTimeout(entity.TraceId.ToString(), entity.Id, timeoutJobId), JobContinuationOptions.OnAnyFinishedState);
        }

        private void StopProjectProcessingJob(Project entity)
        {
            _jobClient.Delete(entity.JobId);
        }
    }
}