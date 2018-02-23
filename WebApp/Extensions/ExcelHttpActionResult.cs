using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApp.Extensions
{
    public class ExcelHttpActionResult : IHttpActionResult
    {
        private readonly byte[] _data;
        
        public ExcelHttpActionResult(byte[] data)
        {
            _data = data;
        }

        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        /// <inheritdoc />
        /// <summary>Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.</summary>
        /// <returns>A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        ////////////////////////////////////////////////////////////
        // Private Methods/Atributes
        ////////////////////////////////////////////////////////////

        private HttpResponseMessage Execute()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                httpResponseMessage.Content = new ByteArrayContent(_data);
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Project.xlsx"
                };
            }
            catch
            {
                httpResponseMessage.Dispose();
                throw;
            }
            return httpResponseMessage;
        }
    }
}
