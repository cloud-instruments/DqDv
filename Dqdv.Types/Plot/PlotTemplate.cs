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
using System.Xml.Serialization;

namespace Dqdv.Types.Plot
{
    [Serializable]
    [XmlRootAttribute("plotTemplate")]
    public class PlotTemplate
    {
        public PlotTemplate()
        {
            xAxis = new SeriesTemplate();
            yAxis = new List<SeriesTemplate>();
            CheckChargeDischarge = false;
            UseCycleData = false;
            UseFirstCycle = false;
            UseAgregateData = false;
        }
        [XmlElement("id")]
        public string Id { get; set; }
        [XmlElement("userId")]
        public string UserId { get; set; }
        [XmlElement("useCycleData")]
        public bool UseCycleData { get; set; }
        [XmlElement("useCRateCalculation")]
        public bool UseCRateCalculation { get; set; }
        [XmlElement("useDischargeCRateCalculation")]
        public bool UseDischargeCRateCalculation { get; set; }
        [XmlElement("useAgregateData")]
        public bool UseAgregateData { get; set; }
        [XmlElement("useFirstCycle")]
        public bool UseFirstCycle { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("checkChargeDischarge")]
        public bool CheckChargeDischarge { get; set; }
        [XmlElement("xAxis")]
        public SeriesTemplate xAxis { get; set; }
        [XmlElement("yAxis")]
        public List<SeriesTemplate> yAxis { get; set; }
        [XmlElement("plotParameters")]
        public PlotParameters PlotParameters { get; set; }
        [XmlElement("canEdit")]
        public bool CanEdit { get; set; }
    }
}
