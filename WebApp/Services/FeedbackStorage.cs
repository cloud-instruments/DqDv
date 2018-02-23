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
using Microsoft.WindowsAzure.Storage;

namespace WebApp.Services
{
    public class FeedbackStorage
    {
        #region Private fields

        private readonly bool _local;
        private readonly string _directory;
        private readonly string _connectionString;
        private readonly string _container;

        #endregion

        #region Constructor

        public FeedbackStorage(string directory)
        {
            _local = true;
            _directory = directory;
        }

        public FeedbackStorage(string connectionString, string container)
        {
            _local = false;
            _connectionString = connectionString;
            _container = container;
        }

        #endregion

        #region IStorage implementation

        public async Task<string> Save(HttpPostedFile file, string name)
        {
            if (_local)
            {
                var path = Path.Combine(_directory, name);
                file.SaveAs(path);
                return path;
            }
            else
            {
                var client = CloudStorageAccount.Parse(_connectionString).CreateCloudBlobClient();
                var container = client.GetContainerReference(_container);
                var blob = container.GetBlockBlobReference(name);
                await blob.UploadFromStreamAsync(file.InputStream);
                return blob.Uri.ToString();
            }
        }

        #endregion
    }
}
