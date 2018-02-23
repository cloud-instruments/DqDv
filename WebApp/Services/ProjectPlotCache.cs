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

using System.Runtime.Caching;

namespace WebApp.Services
{
    /// <summary>
    /// Project plot cache
    /// </summary>
    public class ProjectPlotCache
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private static readonly MemoryCache Cache = new MemoryCache("Projects");

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        /// <summary>
        /// Store object in the cache 
        /// </summary>
        /// <param name="key">Entry key</param>
        /// <param name="value">Instance to store in the cache</param>
        /// <param name="policy">Eviction</param>
        public void Set(string key, object value, CacheItemPolicy policy)
        {
            Cache.Set(key, value, policy);
        }

        /// <summary>
        /// Returns object from the cache
        /// </summary>
        /// <param name="key">Entry key</param>
        /// <returns>object instance or null</returns>
        public object Get(string key)
        {
            return Cache.Get(key);
        }

        /// <summary>
        /// Clear all entries in the cache
        /// </summary>
        public void Flush()
        {
            Cache.Trim(100);
        }

        /// <summary>
        /// Clear entries related to a given project 
        /// </summary>
        /// <param name="projectId">Project identifier</param>
        public void FlushProject(int projectId)
        {
            Cache.Remove($"Points_{projectId}");
            Cache.Remove($"Cycles_{projectId}");
        }
    }
}