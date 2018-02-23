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

using WebApp.Models.Plots;

namespace WebApp.Models
{
    public class Helper
    {
       
        public enum AggregationType
        {
            None = 0,
            Default = 1,
            VisvalingamWhyatt = 2
        }
        internal static PlotSeries AddPlotSeries(string axisText, string name, string title)
        {
            bool exists = axisText.Contains(title);
            return new PlotSeries { Name = name, Selected = exists, Title = exists ? axisText : title };
        }

        internal static PlotSeries AddPlotSeries(string[] axisText, string name, string title)
        {
            foreach (var value in axisText)
            {
                if (value.Contains(title))
                {
                    return new PlotSeries { Name = name, Selected = value.Contains(title), Title = value };
                }
                new PlotSeries { Name = name, Selected = false, Title = title };
            }
            return new PlotSeries { Name = name, Selected = false, Title = title };
        }
    }
}