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
using System.Runtime.Caching;
using WebApp.Interfaces;

namespace WebApp.Services
{
    public sealed class CacheProvider : ICacheProvider, IDisposable
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private readonly MemoryCache _cache = new MemoryCache("PlotParameters");

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        public void Set(string key, object value)
        {
            _cache.Set(key, value, new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration });
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            _cache.Trim(100);
        }

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cache?.Dispose();
        }
    }
}
