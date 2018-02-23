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
using System.Runtime.Serialization;

namespace Dqdv.Types.Plot
{
    [DataContract]
    public class PlotParameters
    {
        public PlotParameters()
        {
            FontSize = 0;
            FontFamilyName = "";
            yAxisText = new List<string>();
            ChartPalette = new List<string>();
            AxisRange = new AxisRange();
        }

        [DataMember]
        public bool? LegendShowen { get; set; }
        [DataMember]
        public int? MaxCycles { get; set; }
        [DataMember]
        public int? MaxPointsPerSeries { get; set; }
        [DataMember]
        public int? FromCycle { get; set; }
        [DataMember]
        public int? ToCycle { get; set; }
        [DataMember]
        public int? EveryNthCycle { get; set; }
        [DataMember]
        public string CustomCycleFilter { get; set; }
        [DataMember]
        public bool? DisableCharge { get; set; }
        [DataMember]
        public bool? DisableDischarge { get; set; }
        [DataMember]
        public double? Threshold { get; set; }
        [DataMember]
        public double? MinY { get; set; }
        [DataMember]
        public double? MaxY { get; set; }
        [DataMember]
        public CurrentUoM? CurrentUoM { get; set; }
        [DataMember]
        public CapacityUoM? CapacityUoM { get; set; }
        [DataMember]
        public TimeUoM? TimeUoM { get; set; }
        [DataMember]
        public PowerUoM? PowerUoM { get; set; }
        [DataMember]
        public EnergyUoM? EnergyUoM { get; set; }
        [DataMember]
        public ResistanceUoM? ResistanceUoM { get; set; }
        [DataMember]
        public VoltageUoM? VoltageUoM { get; set; }
        [DataMember]
        public NormalizeBy? NormalizeBy { get; set; }
        [DataMember]
        public int? PointSize { get; set; }
        [DataMember]
        public bool IsInitialized { get; set; }
        [DataMember]
        public bool? xLineVisible { get; set; }
        [DataMember]
        public bool? yLineVisible { get; set; }
        [DataMember]
        public int FontSize { get; set; }
        [DataMember]
        public string FontFamilyName { get; set; }
        [DataMember]
        public int Simplification { get; set; }
        [DataMember]
        public string xAxisText { get; set; }
        [DataMember]
        public string ChartTitle { get; set; }
        [DataMember]
        public List<string> yAxisText { get; set; }
        [DataMember]
        public AxisRange AxisRange { get; set; }
        [DataMember]
        public List<string> ChartPalette { get; set; }

        public static PlotParameters Default => new PlotParameters
        {
            FromCycle = null,
            MaxPointsPerSeries = 1000,
            MaxCycles = 100,
            ToCycle = null,
            EveryNthCycle = null,
            CustomCycleFilter = string.Empty,
            DisableCharge = null,
            DisableDischarge = null,
            Threshold = null,
            MinY = null,
            Simplification = 1,
            MaxY = null,
            CurrentUoM = Types.CurrentUoM.mA,
            CapacityUoM = Types.CapacityUoM.mAh,
            TimeUoM = Types.TimeUoM.Seconds,
            PowerUoM = Types.PowerUoM.W,
            EnergyUoM = Types.EnergyUoM.Wh,
            ResistanceUoM = Types.ResistanceUoM.Ohm,
            NormalizeBy = Types.NormalizeBy.None,
            IsInitialized = true,
        };
    }
}
