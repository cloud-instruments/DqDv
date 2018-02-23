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
using System.Drawing;
using System.Linq;
using Dqdv.Types;
using Dqdv.Types.Plot;

namespace Plotting
{
    public abstract class ChartPlotterBase : PlotterBase
    {
        public Chart Plot(bool projectsSumCyclesGreaterThanMax, ChartPlotterContext ctx)
        {
            Context = ctx;

            var forcedEveryNthCycle = CalcForcedEveryNthCycle(projectsSumCyclesGreaterThanMax, ctx.ProjectIds, ctx.Parameters, ctx.Trace);
            var param = MakeParameters(ctx.Parameters, forcedEveryNthCycle);
            Chart chart = CreateChart(param);
            chart.ForcedEveryNthCycle = forcedEveryNthCycle;

            foreach (var pid in ctx.ProjectIds)
            {
                Plot(chart, pid, param, ctx.Trace);
            }

            return chart;
        }

        protected ChartPlotterContext Context { get; private set; }

        public virtual bool IsCalcEveryNthCycleForcedDisabled => true;
     
        protected static string FormatCurrentAxis(CurrentUoM currentUoM, NormalizeBy normalizeBy)
        {
            switch (currentUoM)
            {
                case CurrentUoM.A:
                    switch (normalizeBy)
                    {
                        case NormalizeBy.Area:
                            return Title.CurrentCustom2;

                        case NormalizeBy.Mass:
                            return Title.CurrentCustom3;

                        default:
                            return Title.CurrentCustom1;
                    }
                case CurrentUoM.mA:
                    switch (normalizeBy)
                    {
                        case NormalizeBy.Area:
                            return Title.CurrentCustom5;

                        case NormalizeBy.Mass:
                            return Title.CurrentCustom6;

                        default:
                            return Title.CurrentCustom4;
                    }
                case CurrentUoM.uA:
                    switch (normalizeBy)
                    {
                        case NormalizeBy.Area:
                            return Title.CurrentCustom8;

                        case NormalizeBy.Mass:
                            return Title.CurrentCustom9;

                        default:
                            return Title.CurrentCustom7;
                    }
                default:
                    return Title.Current;
            }
        }

        protected void Plot(Chart chart, int projectId, Parameters parameters, string trace)
        {
            var project = ProjectDataRepository.GetProject(projectId, trace);
            chart.Projects.Add(project);

            Plot(chart, project, parameters, trace);
        }


        private Chart CreateChart(Parameters parameters)
        {
            Chart chart = new Chart
            {
                Projects = new List<Project>(),
                Series = new List<Series>(),
                Label = new Label { Font = new Font(FontFamily.GenericMonospace, 8) }
            };
            
            SetupChart(chart, parameters);
            return chart;
        }

        protected abstract void SetupChart(Chart chart, Parameters parameters);

        protected abstract void Plot(Chart chart, Project project, Parameters parameters, string trace);

        protected static string FormatCurrentAxisTitle(Parameters parameters)
        {
            return FormatCurrentAxis(parameters.CurrentUoM, parameters.NormalizeBy);
        }

        protected static string FormatCapacityAxisTitle(Parameters parameters)
        {
            return FormatCapacityAxis(parameters.CapacityUoM, parameters.NormalizeBy);
        }

        protected static string FormatTimeAxisTitle(Parameters parameters)
        {
            return FormatTimeAxis(parameters.TimeUoM);
        }

        protected static string FormatPowerAxisTitle(Parameters parameters)
        {
            return FormatPowerAxis(parameters.PowerUoM);
        }

        protected static string FormatEnergyAxisTitle(Parameters parameters)
        {
            return FormatEnergyAxis(parameters.EnergyUoM);
        }
        
        protected static string FormatResistanceAxisTitle(Parameters parameters)
        {
            return FormatResistanceAxis(parameters.ResistanceUoM);
        }

        private static string FormatPowerAxis(PowerUoM parametersPowerUoM)
        {
            switch (parametersPowerUoM)
            {
                case PowerUoM.W:
                    return Title.PowerCustom1;
                case PowerUoM.muW:
                    return Title.PowerCustom2;
                case PowerUoM.mW:
                    return Title.PowerCustom3;
                case PowerUoM.kW:
                    return Title.PowerCustom4;
                case PowerUoM.MW:
                    return Title.PowerCustom5;
                case PowerUoM.GW:
                    return Title.PowerCustom6;
                default:
                    return Title.PowerCustom1;
            }
        }

        private static string FormatResistanceAxis(ResistanceUoM resistanceUoM)
        {
            switch (resistanceUoM)
            {
                case ResistanceUoM.Ohm:
                    return Title.ResistanceCustom1;
                case ResistanceUoM.muOhm:
                    return Title.ResistanceCustom2;
                case ResistanceUoM.mOhm:
                    return Title.ResistanceCustom3;
                case ResistanceUoM.kOhm:
                    return Title.ResistanceCustom4;
                case ResistanceUoM.MOhm:
                    return Title.ResistanceCustom5;
                case ResistanceUoM.GOhm:
                    return Title.ResistanceCustom6;
                default:
                    return Title.ResistanceCustom1;
            }
        }

        private static string FormatEnergyAxis(EnergyUoM energyUoM)
        {
            switch (energyUoM)
            {
                case EnergyUoM.Wh:
                    return Title.EnergyCustom1;
                case EnergyUoM.muWh:
                    return Title.EnergyCustom2;
                case EnergyUoM.mWh:
                    return Title.EnergyCustom3;
                case EnergyUoM.kWh:
                    return Title.EnergyCustom4;
                case EnergyUoM.MWh:
                    return Title.EnergyCustom5;
                case EnergyUoM.GWh:
                    return Title.EnergyCustom6;
                default:
                    return Title.EnergyCustom1;
            }
        }

        private static string FormatTimeAxis(TimeUoM timeUoM)
        {
            switch (timeUoM)
            {
                case TimeUoM.Seconds:
                    return Title.TimeCustom1;
                case TimeUoM.Minutes:
                    return Title.TimeCustom2;
                case TimeUoM.Hours:
                    return Title.TimeCustom3;
                case TimeUoM.Days:
                    return Title.TimeCustom4;
                default:
                    return Title.TimeCustom1;
            }
        }

        private static string FormatCapacityAxis(CapacityUoM capacityUoM, NormalizeBy normalizeBy)
        {
            switch (capacityUoM)
            {
                case CapacityUoM.Ah:
                    switch (normalizeBy)
                    {
                        case NormalizeBy.Area:
                            return Title.CapacityCustom2;

                        case NormalizeBy.Mass:
                            return Title.CapacityCustom3;

                        default:
                            return Title.CapacityCustom1;
                    }
                case CapacityUoM.mAh:
                    switch (normalizeBy)
                    {
                        case NormalizeBy.Area:
                            return Title.CapacityCustom4;

                        case NormalizeBy.Mass:
                            return Title.CapacityCustom5;

                        default:
                            return Title.CapacityCustom6;
                    }
                default:
                    return Title.Capacity;
            }
        }

        private int? CalcForcedEveryNthCycle(bool projectsSumCyclesGreaterThanMax, 
            int[] projects, 
            PlotParameters parameters, 
            string trace)
        {
            if (!projectsSumCyclesGreaterThanMax || IsCalcEveryNthCycleForcedDisabled)
            {
                return null;
            }

            var maxCycles = parameters?.MaxCycles ?? 0;
            if (maxCycles <= 0)
                return null;

            var maxCyclesPerProject = Math.Max(maxCycles / projects.Length - 1, 1);
            var forcedEveryNthCycle = projects.Max(pid =>
            {
                int cycles = ProjectDataRepository.GetCycles(pid, trace).Count;

                if (parameters != null && string.IsNullOrEmpty(parameters.CustomCycleFilter))
                {
                    int fromCycle = Math.Max(parameters.FromCycle ?? 1, 1);
                    int toCycle = Math.Min(parameters.ToCycle ?? cycles, cycles);

                    cycles = toCycle - fromCycle + 1;
                    int result = cycles / maxCyclesPerProject;
                    if (cycles % maxCyclesPerProject != 0)
                    {
                        result += 1;
                    }

                    return result;
                }
                else
                {
                    if (parameters != null)
                    {
                        var rangeFilter = new IndexRangeFilter(parameters.CustomCycleFilter).RangesItems;
                        cycles = rangeFilter.Count;
                    }
                    int result = cycles / maxCyclesPerProject;
                    if (cycles % maxCyclesPerProject != 0)
                    {
                        result += 1;
                    }

                    return result;
                }
            });

            if (forcedEveryNthCycle < 2)
            {
                return null;
            }

            if (parameters?.EveryNthCycle != null &&
                parameters.EveryNthCycle.Value >= forcedEveryNthCycle)
            {
                return null;
            }

            return forcedEveryNthCycle;
        }


        protected ChartPlotterBase(IProjectDataRepository projectDataRepository) : base(projectDataRepository)
        {
        }
    }
}