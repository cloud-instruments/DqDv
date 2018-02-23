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

using System.Linq;
using DataLayer;
using Dqdv.Types.Plot;
using Newtonsoft.Json;
using WebApp.Interfaces;

namespace WebApp.Services
{
    public class ChartSettingProvider
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private readonly ICacheProvider _cacheProvider;
        private readonly DqdvContext _db;

        ////////////////////////////////////////////////////////////
        // Constructors
        ////////////////////////////////////////////////////////////

        public ChartSettingProvider(DqdvContext db, ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _db = db;
        }

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        public PlotParameters GetSettings(string userId)
        {
            return (PlotParameters)_cacheProvider.Get($"PlotParameters_{userId}") ?? PlotParameters.Default;
        }

        public void SetSettings(string userId, PlotParameters parameters)
        {
            _cacheProvider.Set($"PlotParameters_{userId}", parameters);
        }

        public void ClearSettings(string userId)
        {
            _cacheProvider.Remove($"PlotParameters_{userId}");
        }

        public  PlotParameters MergeWithGlobal(string userId, PlotParameters origiParameters)
        {
            var globalPreferences = _db.UserPreferences.Find(userId) ?? AppUserPreferences.Default;
            var deepCopy = JsonConvert.DeserializeObject<PlotParameters>(JsonConvert.SerializeObject(origiParameters));

            deepCopy.LegendShowen = deepCopy.LegendShowen ?? globalPreferences.ChartPreferences.LegendShowen;
            deepCopy.FontFamilyName = string.IsNullOrEmpty(deepCopy.FontFamilyName) ? globalPreferences.ChartPreferences.FontFamilyName : deepCopy.FontFamilyName;
            deepCopy.FontSize = deepCopy.FontSize <= 0 ? globalPreferences.ChartPreferences.FontSize : deepCopy.FontSize;
            deepCopy.PointSize = deepCopy.PointSize ?? globalPreferences.ChartPreferences.PointSize;
            deepCopy.xLineVisible = deepCopy.xLineVisible ?? globalPreferences.ChartPreferences.XLineVisible;
            deepCopy.yLineVisible = deepCopy.yLineVisible ?? globalPreferences.ChartPreferences.YLineVisible;
            deepCopy.LegendShowen = deepCopy.LegendShowen ?? globalPreferences.ChartPreferences.LegendShowen;
            deepCopy.ChartPalette = globalPreferences.ChartPreferences.PaletteColors.ToList();

            return deepCopy;
        }

        public PlotParameters MergePlotParameters(PlotParameters origiParameters, PlotParameters mergeParameters)
        {
            var deepCopy = JsonConvert.DeserializeObject<PlotParameters>(JsonConvert.SerializeObject(origiParameters));
            deepCopy.LegendShowen = mergeParameters.LegendShowen;
            deepCopy.FontFamilyName = string.IsNullOrEmpty(mergeParameters.FontFamilyName) ? deepCopy.FontFamilyName : mergeParameters.FontFamilyName;
            deepCopy.FontSize = mergeParameters.FontSize <= 0 ? deepCopy.FontSize : mergeParameters.FontSize;
            deepCopy.PointSize = mergeParameters.PointSize ?? deepCopy.PointSize  ;
            deepCopy.xLineVisible = mergeParameters.xLineVisible ?? deepCopy.xLineVisible;
            deepCopy.yLineVisible = mergeParameters.yLineVisible ?? deepCopy.yLineVisible;
            deepCopy.LegendShowen = mergeParameters.LegendShowen ?? deepCopy.LegendShowen;
            deepCopy.ChartPalette = mergeParameters.ChartPalette;

            deepCopy.NormalizeBy = mergeParameters.NormalizeBy ?? deepCopy.NormalizeBy;
            deepCopy.CurrentUoM = mergeParameters.CurrentUoM ?? deepCopy.CurrentUoM;
            deepCopy.TimeUoM = mergeParameters.TimeUoM ?? deepCopy.TimeUoM;
            deepCopy.PowerUoM = mergeParameters.PowerUoM ?? deepCopy.PowerUoM;
            deepCopy.EnergyUoM = mergeParameters.EnergyUoM ?? deepCopy.EnergyUoM;
            deepCopy.ResistanceUoM = mergeParameters.ResistanceUoM ?? deepCopy.ResistanceUoM;
            deepCopy.CapacityUoM = mergeParameters.CapacityUoM ?? deepCopy.CapacityUoM;
            deepCopy.VoltageUoM = mergeParameters.VoltageUoM ?? deepCopy.VoltageUoM;
            deepCopy.EnergyUoM = mergeParameters.EnergyUoM ?? deepCopy.EnergyUoM;
            
            deepCopy.DisableDischarge = mergeParameters.DisableDischarge ?? deepCopy.DisableDischarge;
            deepCopy.DisableCharge = mergeParameters.DisableCharge ?? deepCopy.DisableCharge;

            deepCopy.FromCycle = mergeParameters.FromCycle ?? deepCopy.FromCycle;
            deepCopy.ToCycle = mergeParameters.ToCycle ?? deepCopy.ToCycle;
            deepCopy.EveryNthCycle = mergeParameters.EveryNthCycle ?? deepCopy.EveryNthCycle;

            deepCopy.xAxisText = mergeParameters.xAxisText ?? deepCopy.xAxisText;
            deepCopy.yAxisText = mergeParameters.yAxisText.Any() ? mergeParameters.yAxisText : deepCopy.yAxisText;

            deepCopy.ChartTitle = mergeParameters.ChartTitle ?? deepCopy.ChartTitle;

            return deepCopy;
        }
    }
}