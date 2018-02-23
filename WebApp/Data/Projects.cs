using System.Linq;
using DataLayer;
using LinqKit;
using System.Collections.Generic;

namespace WebApp.Data
{
    static class DqdvProjectsExtensions
    {
        public static IQueryable<Project> GetProjects(this DqdvContext @this, string userId)
        {
            return @this.Projects
                .AsExpandable()
                .Where(p => p.UserId == userId);
        }

        public static IEnumerable<Project> GetAllUserProjects(this DqdvContext @this, string userId)
        {
            List<Project> projects = new List<Project>();
            AppUser user = @this.Users.FirstOrDefault(p => p.Id == userId);

            if (user != null)
            {
                projects.AddRange(user.Projects);
                projects.AddRange(user.SharedProjects);
            }
            return projects;
        }
    }

    static class DqdvPlotTemplatesExtensions
    {
        public static IQueryable<PlotTemplate> GetPlotTemplates(this DqdvContext @this, string userId)
        {
            return @this.PlotTemplates
                .AsExpandable()
                .Where(p => p.UserId == userId);
        }

        public static IEnumerable<PlotTemplate> GetAllUserPlotTemplates(this DqdvContext @this, string userId, string templateUserId)
        {
            List<PlotTemplate> templates = new List<PlotTemplate>();
            var user = @this.Users.FirstOrDefault(p => p.Id == userId);
            var templateUser = @this.Users.FirstOrDefault(p => p.Id == templateUserId);
            if (user != null && templateUser != null)
            {
                templates.AddRange(templateUser.PlotTemplates);
                templates.AddRange(user.PlotTemplates);
                templates.AddRange(user.SharedTemplates);
            }
            return templates;
        }
    }
}
