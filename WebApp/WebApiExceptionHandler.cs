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
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace WebApp
{
    class WebApiExceptionHandler : IExceptionHandler
    {
        #region Public methods

        public static bool IsHandled(HttpRequestMessage request, Exception ex)
        {
            return IsDivideByZeroInGetProjects(request, ex);
        }

        #endregion

        #region IExceptionHandler implementation

        public virtual Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (IsDivideByZeroInGetProjects(context.Request, context.Exception))
                context.Result = HandleDivideByZeroInGetProjects(context.Request);

            return Task.CompletedTask;
        }

        #endregion

        #region Private methods

        private static bool IsDivideByZeroInGetProjects(HttpRequestMessage request, Exception ex)
        {
            return request != null &&
                ex != null &&
                request.Method == HttpMethod.Get &&
                request.RequestUri.AbsolutePath == "/odata/projects" &&
                IsDivideByZero(ex);
        }

        private static bool IsDivideByZero(Exception ex)
        {
            if (ex is SqlException sqlEx && sqlEx.Number == 8134)
                return true;

            return ex.InnerException != null && IsDivideByZero(ex.InnerException);
        }

        private static IHttpActionResult HandleDivideByZeroInGetProjects(HttpRequestMessage request)
        {
            const string message = "Division by zero encountered while applying the filter";
            var response = request.CreateResponse(HttpStatusCode.BadRequest, new HttpError(message));
            return new ResponseMessageResult(response);
        }

        #endregion
    }
}
