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

using System.IO;
using System.Threading.Tasks;
using System.Web;
using WebApp.Interfaces;
using WebApp.Models.Chart;

namespace WebApp.Services
{
    class LocalStorage : IStorage
    {
        #region Private fields

        private readonly string _directory;

        #endregion

        #region Constructor

        public LocalStorage(string directory)
        {
            _directory = directory;
        }

        #endregion

        #region IStorage implementation

        public void SaveAs(HttpPostedFile file, string name)
        {
            file.SaveAs(Path.Combine(_directory, name));
        }

        /// <inheritdoc />
        /// <summary>
        /// Save file stream to a storage 
        /// </summary>
        /// <param name="file">File stream to save</param>
        /// <param name="name">File name in a storage</param>
        /// <returns></returns>
        public async Task SaveAsAsync(HttpPostedFile file, string name)
        {
            using (var stream = File.Create(Path.Combine(_directory, name)))
            {
                await file.InputStream.CopyToAsync(stream).ConfigureAwait(false);
            }
        }

        public async Task<BlobDownloadModel> DownloadToStreamAsync(string name, string internalId)
        {
            string path = Path.Combine(_directory, internalId);

            if (!File.Exists(path))
            {
                return null;
            }
            using (Stream sourceStream = File.Open(path, FileMode.Open))
            {
                byte[] result = new byte[sourceStream.Length];
                await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length);

                // Build and return the download model with the blob stream and its relevant info
                var download = new BlobDownloadModel
                {
                    BlobStream = new MemoryStream(result),
                    BlobFileName = name,
                    BlobLength = result.Length,
                    BlobContentType = MimeMapping.GetMimeMapping(name)
                };

                return download;
            }
        }

        #endregion
    }
}
