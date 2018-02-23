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

using Dqdv.Types.Plot;
using Plotting;
using System.Collections.Generic;
using WebApp.Models.Plots;

namespace WebApp.Models.Chart
{
    class ChartDto
    {
        public string Title { get; set; }
        public string XAxisText { get; set; }
        public bool XAxisIsInteger { get; set; }
        public string[] YAxisText { get; set; }
        public List<int> ProjectIds { get; set; }
        public List<ProjectDto> Projects { get; set; }
        public List<SeriesDto> Series { get; set; }
        public List<Dictionary<string, double?>> Points { get; set; }
        public int? ForcedEveryNthCycle { get; set; }
        public Label Label { get; set; }
        public List<PlotTemplate> PlotSettings { get; set; }
        public PlotFormulaModel PlotFormulaModel { get; set; }
        public string SelectedTemplateName { get; set; }
        public PlotParameters PlotParameters { get; set; }
    }
}
