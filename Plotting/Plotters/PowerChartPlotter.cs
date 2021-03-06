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

using Dqdv.Types;

namespace Plotting.Plotters
{
    public class PowerChartPlotter : ChartPlotterBase
    {
        public PowerChartPlotter(IProjectDataRepository projectDataRepository) : base(projectDataRepository)
        {
        }

        protected override void SetupChart(Chart chart, Parameters parameters)
        {
            chart.XAxisText = Title.CycleNumber;
            chart.XAxisIsInteger = true;
            chart.YAxisText = new [] { FormatPowerAxisTitle(parameters) };
        }

        protected override void Plot(Chart chart, Project project, Parameters parameters, string trace)
        {
            var cycles = ProjectDataRepository.GetCycles(project.Id, trace);
            var multiplier = GetPowerMultiplier(project, parameters);

            if (parameters.IsChargeEnabled)
            {
                var points = Plot(cycles, parameters, c => multiplier * c.Power, cycle => multiplier * cycle.StatisticMetaData.PowerStdDev);
                AddSeries(chart, project, parameters, points, Title.Charge);
            }

            if (parameters.IsDischargeEnabled)
            {
                var points = Plot(cycles, parameters, c => multiplier * c.DischargePower, cycle => multiplier * cycle.StatisticMetaData.DischargePowerStdDev);
                AddSeries(chart, project, parameters, points, Title.Discharge);
            }
        }
    }
}
