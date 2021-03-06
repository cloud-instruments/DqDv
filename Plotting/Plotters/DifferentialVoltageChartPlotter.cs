﻿/*
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
using Dqdv.Types;

namespace Plotting.Plotters
{
    public class DifferentialVoltageChartPlotter : ChartPlotterBase
    {
        public DifferentialVoltageChartPlotter(IProjectDataRepository projectDataRepository) : base(projectDataRepository)
        {
        }

        public override bool IsCalcEveryNthCycleForcedDisabled => false;

        ////////////////////////////////////////////////////////////
        // Protected Methods/Atributes
        ////////////////////////////////////////////////////////////

        protected override void SetupChart(Chart chart, Parameters parameters)
        {
            chart.XAxisText = FormatCapacityAxisTitle(parameters);
            chart.YAxisText = new [] { Title.DVDQ };
        }

        protected override void Plot(Chart chart, Project project, Parameters parameters, string trace)
        {
            var cycles = ProjectDataRepository.GetCycles(project.Id, trace);
            var allPoints = ProjectDataRepository.GetPoints(project.Id, trace);
            var multiplier = GetCapacityMultiplier(project, parameters);

            foreach (var cycle in FilterCycles(cycles, parameters))
            {
                if (parameters.IsChargeEnabled)
                {
                    var points = PlotDerivative(allPoints, cycle.FirstPointIndex, cycle.PointCount,
                        p => p.CycleStep == CycleStep.ChargeCC || p.CycleStep == CycleStep.ChargeCV, p => multiplier * p.Capacity, p => p.Voltage, parameters).ToList();
                    AddSeries(chart, project, parameters, points, Title.Charge, cycle.Index);
                }

                if (parameters.IsDischargeEnabled)
                {
                    var points = PlotDerivative(allPoints, cycle.FirstPointIndex, cycle.PointCount,
                        p => p.CycleStep == CycleStep.Discharge, p => multiplier * p.Capacity, p => p.Voltage, parameters).ToList();
                    AddSeries(chart, project, parameters, points, Title.Discharge, cycle.Index);
                }
            }
        }
    }
}