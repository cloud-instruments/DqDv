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
using System.Collections.Generic;
using System.Linq;
using Dqdv.Types;
using Dqdv.Types.Plot;

namespace Plotting
{
    public abstract class PlotterBase
    {
        private static readonly List<Point> NoPoints = new List<Point>();

        ////////////////////////////////////////////////////////////
        // Protected Methods/Atributes
        ////////////////////////////////////////////////////////////

        protected readonly IProjectDataRepository ProjectDataRepository;

        protected PlotterBase(IProjectDataRepository projectDataRepository)
        {
            ProjectDataRepository = projectDataRepository;
        }

        protected Parameters MakeParameters(PlotParameters parameters, int? forcedEveryNthCycle)
        {
            return new Parameters
            {
                MaxPointsPerSeries = parameters?.MaxPointsPerSeries,
                Simplification = parameters?.Simplification ?? 0,
                FromCycle = parameters?.FromCycle,
                ToCycle = parameters?.ToCycle,
                EveryNthCycle = forcedEveryNthCycle ?? parameters?.EveryNthCycle,
                CustomCycleFilter = string.IsNullOrWhiteSpace(parameters?.CustomCycleFilter) ? 
                    null : 
                    new IndexRangeFilter(parameters.CustomCycleFilter),
                IsChargeEnabled = !(parameters?.DisableCharge ?? false),
                IsDischargeEnabled = !(parameters?.DisableDischarge ?? false),
                Threshold = parameters?.Threshold,
                MinY = parameters?.MinY,
                MaxY = parameters?.MaxY,
                CurrentUoM = parameters?.CurrentUoM ?? CurrentUoM.mA,
                CapacityUoM = parameters?.CapacityUoM ?? CapacityUoM.mAh,
                TimeUoM = parameters?.TimeUoM ?? TimeUoM.Seconds,
                PowerUoM = parameters?.PowerUoM ?? PowerUoM.W,
                EnergyUoM = parameters?.EnergyUoM ?? EnergyUoM.Wh,
                ResistanceUoM = parameters?.ResistanceUoM ?? ResistanceUoM.Ohm,
                NormalizeBy = parameters?.NormalizeBy ?? NormalizeBy.None
            };
        }

        protected static void AddSeries(Chart chart, 
            Project project, 
            Parameters parameters, 
            List<Point> points, 
            string name, 
            int? cycleIndex = null, 
            bool isZAxis = false)
        {
            var displayName = $"{project.Name}: {name}";
            if (cycleIndex != null)
                displayName += $" (Cycle {cycleIndex})";

            if (points != null && (parameters.Simplification > 0 || parameters.MaxPointsPerSeries != null))
                points = new LineSimplifier().Simplify(points, parameters.MaxPointsPerSeries ?? 1000);

            if(chart.YAxisText.Length == 0 && !isZAxis)
            {
                chart.YAxisText = new[] { name };
            }
            if (chart.YAxisText.Length == 1 && isZAxis)
            {
                chart.YAxisText = new[] { chart.YAxisText[0], name };
            }

            chart.Series.Add(
                new Series
                {
                    ProjectId = project.Id,
                    Name = name,
                    IsZAxis = isZAxis,
                    CycleIndex = cycleIndex,
                    DisplayName = displayName,
                    Points = points ?? NoPoints
                });
        }

        protected static IEnumerable<Cycle> FilterCycles(List<Cycle> cycles, Parameters parameters)
        {
            var lastCycleIndex = cycles.LastOrDefault()?.Index ?? 0;
            var query = cycles.AsEnumerable();

            if (parameters.FromCycle != null)
                query = query.Where(c => c.Index >= parameters.FromCycle.Value);

            if (parameters.ToCycle != null)
                query = query.Where(c => c.Index <= parameters.ToCycle.Value);

            if (parameters.EveryNthCycle != null)
            {
                var first = parameters.FromCycle ?? 1;
                var last = parameters.ToCycle ?? lastCycleIndex;
                query = query.Where(c => (c.Index - first + 1) % 
                    parameters.EveryNthCycle.Value == 0 || 
                    c.Index == first || 
                    c.Index == last || 
                    c.Index == lastCycleIndex);
            }

            if (parameters.CustomCycleFilter != null)
                query = query.Where(c => parameters.CustomCycleFilter.Contains(c.Index));

            return query;
        }

        protected static IEnumerable<Point> Plot(List<DataPoint> points, 
            Cycle cycle, 
            Func<DataPoint, double?> getX, 
            Func<DataPoint, double?> getY, 
            Parameters parameters, 
            Func<DataPoint, bool> filter = null)
        {
            return ApplyYFilter(
                Plot(points, cycle.FirstPointIndex, cycle.PointCount, getX, getY, filter).ToList(), 
                parameters);
        }

        protected static IEnumerable<Point> Plot(List<DataPoint> points, 
            int offset, 
            int count, 
            Func<DataPoint, double?> getX, 
            Func<DataPoint, double?> getY, 
            Func<DataPoint, bool> filter = null)
        {
            for (var i = offset; i < offset + count; i++)
            {
                if (filter != null && i < points.Count && !filter(points[i]))
                    continue;

                var x = getX(points[i]);
                if (x == null)
                    continue;

                var y = getY(points[i]);
                if (y == null)
                    continue;

                yield return new Point
                {
                    X = x.Value,
                    Y = y.Value
                };
            }
        }

        protected static List<Point> Plot(List<Cycle> cycles, 
            Parameters parameters, Func<Cycle, double?> getValue, Func<Cycle, double?> getStdDevValue)
        {
            List<Point> points = FilterCycles(cycles, parameters)
                .Select(c =>
                    new
                    {
                        c.Index,
                        Value = getValue(c),
                        HighError = getStdDevValue(c).HasValue ? getValue(c) + getStdDevValue(c) : null,
                        LowError = getStdDevValue(c).HasValue ? getValue(c) - getStdDevValue(c) : null,
                    })
                .Where(c => c.Value != null)
                .Select(c =>
                    new Point
                    {
                        X = c.Index,
                        Y = c.Value.Value,
                        HighError = c.HighError,
                        LowError = c.LowError
                    }).ToList();

            points = ApplyYFilter(points, parameters).ToList();
            return points.ToList();
        }
        protected IEnumerable<Point> PlotDerivative(List<DataPoint> points, 
            int offset, 
            int count, 
            Func<DataPoint, bool> filter, 
            Func<DataPoint, double?> getX, 
            Func<DataPoint, double?> getY, 
            Parameters parameters)
        {
            var prevX = (Double?)null;
            var prevY = (Double?)null;

            for (var i = offset; i < offset + count; i++)
            {
                if (!filter(points[i]))
                    continue;

                var currX = getX(points[i]);
                if (currX == null)
                    continue;

                var currY = getY(points[i]);
                if (currY == null)
                    continue;

                if (prevX == null)
                {
                    prevX = currX;
                    prevY = currY;
                    continue;
                }

                var dX = currX.Value - prevX.Value;
                if (parameters.Threshold != null && Math.Abs(dX) < parameters.Threshold)
                    continue;

                var dY = currY.Value - prevY.Value;
                var value = dY / dX;
                if (double.IsNaN(value) || double.IsInfinity(value))
                    continue;

                prevX = currX;
                prevY = currY;

                if (parameters.MinY != null && value < parameters.MinY.Value)
                    continue;

                if (parameters.MaxY != null && value > parameters.MaxY.Value)
                    continue;

                yield return new Point
                {
                    X = prevX.Value,
                    Y = value
                };
            }
        }

        protected static IEnumerable<Point> ApplyYFilter(List<Point> points, Parameters parameters)
        {
            var minY = parameters.MinY;
            if (minY != null)
                points = points.Where(p => p.Y >= minY.Value).ToList();

            var maxY = parameters.MaxY;
            if (maxY != null)
                points = points.Where(p => p.Y <= maxY.Value).ToList();

            return points;
        }

        protected static double GetCurrentMultiplier(Project project, Parameters parameters)
        {
            var multiplier = GetCurrentMultiplier(parameters.CurrentUoM) / GetNormalizeByDivider(project, parameters);
            return double.IsInfinity(multiplier) || double.IsNaN(multiplier) ? 1.0 : multiplier;
        }

        protected static double GetCapacityMultiplier(Project project, Parameters parameters)
        {
            var multiplier = GetCapacityMultiplier(parameters.CapacityUoM) / GetNormalizeByDivider(project, parameters);
            return double.IsInfinity(multiplier) || double.IsNaN(multiplier) ? 1.0 : multiplier;
        }

        protected static double GetTimeMultiplier(Project project, Parameters parameters)
        {
            var multiplier = GetTimeMultiplier(parameters.TimeUoM) / GetNormalizeByDivider(project, parameters);
            return double.IsInfinity(multiplier) || double.IsNaN(multiplier) ? 1.0 : multiplier;
        }

        protected static double GetEnergyMultiplier(Project project, Parameters parameters)
        {
            var multiplier = GetEnergyMultiplier(parameters.EnergyUoM) / GetNormalizeByDivider(project, parameters);
            return double.IsInfinity(multiplier) || double.IsNaN(multiplier) ? 1.0 : multiplier;
        }

        protected static double GetResistanceMultiplier(Project project, Parameters parameters)
        {
            var multiplier = GetResistanceMultiplier(parameters.ResistanceUoM) / GetNormalizeByDivider(project, parameters);
            return double.IsInfinity(multiplier) || double.IsNaN(multiplier) ? 1.0 : multiplier;
        }

        protected static double GetPowerMultiplier(Project project, Parameters parameters)
        {
            var multiplier = GetPowerMultiplier(parameters.PowerUoM) / GetNormalizeByDivider(project, parameters);
            return double.IsInfinity(multiplier) || double.IsNaN(multiplier) ? 1.0 : multiplier;
        }

        private static double GetTimeMultiplier(TimeUoM timeUoM)
        {
            switch (timeUoM)
            {
                case TimeUoM.Seconds:
                    return 1;
                case TimeUoM.Minutes:
                    return 0.0166666666666667; // 1 / 60
                case TimeUoM.Hours:
                    return 2.777777777777778e-4; // 1 / 3600
                case TimeUoM.Days:
                    return 1.157407407407407e-5; // 1 / (3600 * 24)
                default:
                    return 1;
            }
        }

        private static double GetEnergyMultiplier(EnergyUoM energyUoM)
        {
            switch (energyUoM)
            {
                case EnergyUoM.Wh:
                    return 1;
                case EnergyUoM.muWh:
                    return 1000000;
                case EnergyUoM.mWh:
                    return 1000;
                case EnergyUoM.kWh:
                    return 0.001;
                case EnergyUoM.MWh:
                    return 0.000001;
                case EnergyUoM.GWh:
                    return 0.000000001;
                default:
                    return 1;
            }
        }
        
        private static double GetPowerMultiplier(PowerUoM powerUoM)
        {
            switch (powerUoM)
            {
                case PowerUoM.W:
                    return 1;
                case PowerUoM.muW:
                    return 1000000;
                case PowerUoM.mW:
                    return 1000;
                case PowerUoM.kW:
                    return 0.001;
                case PowerUoM.MW:
                    return 0.000001;
                case PowerUoM.GW:
                    return 0.000000001;
                default:
                    return 1;
            }
        }

        private static double GetResistanceMultiplier(ResistanceUoM resistanceUoM)
        {
            switch (resistanceUoM)
            {
                case ResistanceUoM.Ohm:
                    return 1;
                case ResistanceUoM.muOhm:
                    return 1000000;
                case ResistanceUoM.mOhm:
                    return 1000;
                case ResistanceUoM.kOhm:
                    return 0.001;
                case ResistanceUoM.MOhm:
                    return 0.000001;
                case ResistanceUoM.GOhm:
                    return 0.000000001;
                default:
                    return 1;
            }
        }
        private static double GetCapacityMultiplier(CapacityUoM capacityUoM)
        {
            switch (capacityUoM)
            {
                case CapacityUoM.Ah:
                    return 0.001;

                default:
                    return 1;
            }
        }

        public static double GetNormalizeByDivider(Project project, Parameters parameters)
        {
            switch (parameters.NormalizeBy)
            {
                default: return 1.0;

                case NormalizeBy.Area:
                    return project.Area ?? 1.0;

                case NormalizeBy.Mass:
                    return project.Mass.GetValueOrDefault(1.0) * project.ActiveMaterialFraction.GetValueOrDefault(1.0);

                case NormalizeBy.Asi:
                    return project.Area ?? 1.0;
            }
        }

        private static double GetCurrentMultiplier(CurrentUoM currentUoM)
        {
            switch (currentUoM)
            {
                case CurrentUoM.A:
                    return 0.001;

                case CurrentUoM.uA:
                    return 1000;

                default:
                    return 1;
            }
        }


        #region Private types

        public class Parameters
        {
            public int? MaxPointsPerSeries { get; set; }
            public int Simplification { get; set; }
            public int? FromCycle { get; set; }
            public int? ToCycle { get; set; }
            public int? EveryNthCycle { get; set; }
            public IndexRangeFilter CustomCycleFilter { get; set; }

            public bool IsChargeEnabled { get; set; }
            public bool IsDischargeEnabled { get; set; }

            public double? Threshold { get; set; }
            public double? MinY { get; set; }
            public double? MinY2 { get; set; }
            public double? MaxY { get; set; }
            public double? MaxY2 { get; set; }
            public CurrentUoM CurrentUoM { get; set; }
            public CapacityUoM CapacityUoM { get; set; }
            public VoltageUoM VoltageUoM { get; set; }
            public TimeUoM TimeUoM { get; set; }
            public PowerUoM PowerUoM { get; set; }
            public EnergyUoM EnergyUoM { get; set; }
            public ResistanceUoM ResistanceUoM { get; set; }
            public TemperatureUoM TemperatureUoM { get; set; }
            public NormalizeBy NormalizeBy { get; set; }
        }

        #endregion
    }
}