using System.Threading.Tasks;
using System.Web;
using WebApp.Models.Chart;

namespace WebApp.Interfaces
{
    public interface IStorage
    {
        void SaveAs(HttpPostedFile file, string name);

        /// <summary>
        /// Save file stream to a storage 
        /// </summary>
        /// <param name="file">File stream to save</param>
        /// <param name="name">File name in a storage</param>
        /// <returns></returns>
        Task SaveAsAsync(HttpPostedFile file, string name);

        /// <summary>
        /// Download file stream from a storage 
        /// </summary>
        /// <param name="name">File name in a storage</param>
        /// <param name="internalId">Internal File Name</param>
        /// <returns>Files</returns>
        Task<BlobDownloadModel> DownloadToStreamAsync(string name, string internalId);
    }
}
