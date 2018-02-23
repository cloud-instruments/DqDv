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
using System.Linq;
using WebApp.Models.Chart;

namespace WebApp.Models.Mappings
{
    static class ChartToDto
    {
        public static ChartDto ToChartDto(this Plotting.Chart chart, string title = "")
        {
            var dto = new ChartDto
            {
                ForcedEveryNthCycle = chart.ForcedEveryNthCycle,
                Points = chart.Series
                    .Select((series, index) => new {series, index})
                    .SelectMany(item =>
                        item.series.Points
                            .Where(p => !double.IsNaN(p.Y))
                            .Select(p => new Dictionary<string, double?>
                            {
                                {"x", p.X},
                                {item.series.IsZAxis ? "z1" : $"y{item.index}", p.Y},
                                {"highError", p.HighError.GetValueOrDefault(p.Y)},
                                {"lowError", p.LowError.GetValueOrDefault(p.Y)},
                                {"discrete", p.Discrete}
                            }))
                    .ToList(),
                PlotParameters = chart.PlotParameters
            };

            var yAxisTextNonNull = chart.PlotParameters.yAxisText.Where(text => !string.IsNullOrWhiteSpace(text)).ToList();

            dto.Series = chart.Series.Select((series, index) => new { series, index })
                // IMPORTANT: there is a problem with empty series, let's remove them temporarily.
                // https://github.com/agafonovslava/dqdv/issues/23
                // https://www.devexpress.com/Support/Center/Question/Details/T557040/chart-sets-vertical-range-incorrectly-when-using-aggregation
                .Where(item => item.series.Points != null && item.series.Points.Count != 0)
                .Select(item =>
                {
                    var seriesDto = new SeriesDto();
                    seriesDto.Id = $"{item.series.ProjectId}_{item.series.DisplayName}";
                    seriesDto.ProjectId = item.series.ProjectId;
                    seriesDto.Name = item.series.DisplayName;
                    seriesDto.ValueField = item.series.IsZAxis ?
                        "z1" :
                        $"y{item.index}";

                    seriesDto.Axis = item.series.IsZAxis ?
                        $"z1" :
                        null;

                    return seriesDto;
                })
                .ToList();
            dto.Projects = chart.Projects
                .Select(p =>
                    new ProjectDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        IsAveragePlot = p.IsAveragePlot
                    })
                .ToList();
            dto.ProjectIds = chart.Projects.Select(p => p.Id).ToList();
            dto.YAxisText = yAxisTextNonNull.Count > 0 ?
                yAxisTextNonNull.ToArray() :
                chart.YAxisText.ToArray();

            dto.Label = chart.Label;
            dto.XAxisIsInteger = chart.XAxisIsInteger;
            dto.SelectedTemplateName = chart.SelectedTemplateName;
            dto.XAxisText = string.IsNullOrEmpty(chart.PlotParameters.xAxisText) ?
                chart.XAxisText :
                chart.PlotParameters.xAxisText;

            dto.Title = string.IsNullOrEmpty(chart.PlotParameters.ChartTitle) ?
                title :
                chart.PlotParameters.ChartTitle;

            return dto;
        }
    }
}