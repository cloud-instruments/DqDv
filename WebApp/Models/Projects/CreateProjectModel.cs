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
using System.Web;

namespace WebApp.Models.Projects
{
    public class CreateProjectModel
    {
        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a test name
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets a test type
        /// </summary>
        public string TestType { get; set; }

        /// <summary>
        /// Gets or sets a cannel
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets a tag
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets a mass
        /// </summary>
        public double? Mass { get; set; }
        /// <summary>
        /// Gets or sets theoretical capacity
        /// </summary>
        public double? TheoreticalCapacity { get; set; }
        /// <summary>
        /// Gets or sets active material fraction
        /// </summary>
        public double? ActiveMaterialFraction { get; set; }

        /// <summary>
        /// Gets or sets an area
        /// </summary>
        public double? Area { get; set; }

        /// <summary>
        /// Gets or sets comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Indicates of existing project should be overwrite
        /// </summary>
        public bool OverwriteExisting { get; set; }

        /// <summary>
        /// Gets or sets list of uploaded files
        /// </summary>
        public List<HttpPostedFile> Files { get; set; }
    }
}
