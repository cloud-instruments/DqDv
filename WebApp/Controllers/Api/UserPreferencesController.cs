using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using DataLayer;
using Microsoft.AspNet.Identity;
using WebApp.Interfaces;
using WebApp.Models.Preferences;

namespace WebApp.Controllers.Api
{
    [Authorize]
    [Route("api/preferences")]
    public class UserPreferencesController : ApiController
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private readonly DqdvContext _db;
        private readonly ICacheProvider _cacheProvider;

        ////////////////////////////////////////////////////////////
        // Constructors
        ////////////////////////////////////////////////////////////

        /// <inheritdoc />
        /// <summary>
        /// Initialize a new instance of <see cref="T:WebApp.Controllers.Api.UserPreferencesController" />
        /// </summary>
        /// <param name="db"></param>
        /// <param name="cacheProvider"></param>
        public UserPreferencesController(DqdvContext db, ICacheProvider cacheProvider)
        {
            _db = db;
            _cacheProvider = cacheProvider;
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> SavePreferences(UserPreferencesModel model)
        {
            var entity = await _db.UserPreferences.FindAsync(User.Identity.GetUserId()) ?? new AppUserPreferences();

            entity.Id = User.Identity.GetUserId();
            entity.ChartPreferences = new ChartPreferences
            {
                LegendShowen = model.ChartPreferences.ShowLegend,
                XLineVisible = model.ChartPreferences.XLineVisible,
                YLineVisible = model.ChartPreferences.YLineVisible,
                PointSize = model.ChartPreferences.PointSize,
                FontSize = model.ChartPreferences.FontSize,
                FontFamilyName = model.ChartPreferences.FontFamilyName,
                PaletteColors = model.ChartPreferences.PaletteColors.Select(item => item.Color).ToArray()
            };

            if (_db.Entry(entity).State == EntityState.Detached)
                _db.UserPreferences.Add(entity);

            await _db.SaveChangesAsync();
            _cacheProvider.Clear();
            return Ok();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetPreferences()
        {
            var entity = await _db.UserPreferences.FindAsync(User.Identity.GetUserId()) ?? AppUserPreferences.Default;
            return Ok(new UserPreferencesModel
            {
                ChartPreferences  = new ChartPreferencesModel
                {
                    ShowLegend = entity.ChartPreferences.LegendShowen,
                    XLineVisible = entity.ChartPreferences.XLineVisible,
                    YLineVisible = entity.ChartPreferences.YLineVisible,
                    PointSize = entity.ChartPreferences.PointSize,
                    FontSize = entity.ChartPreferences.FontSize,
                    FontFamilyName = entity.ChartPreferences.FontFamilyName,
                    PaletteColors = entity.ChartPreferences.PaletteColors.Select(item => new PaletteColorItem { Color = item })
                }
            });
        }
    }
}
