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
using System.Reflection;
using System.Web;
using log4net;

namespace WebApp
{
    public class Global : HttpApplication
    {
        ////////////////////////////////////////////////////////////
        // Constants, Enums and Class members
        ////////////////////////////////////////////////////////////

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ////////////////////////////////////////////////////////////
        // Protected Methods/Atributes
        ////////////////////////////////////////////////////////////

        protected void Application_End()
        {
            Log.Info("Global.Application_End: Stopped");
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            if (ex is OperationCanceledException)
            {
                Log.Info("Global.Application_Error: OperationCanceledException", ex);
                Server.ClearError();
            }
            else if (ex is HttpException && ex.Message.Contains("Server cannot append header after HTTP headers have been sent"))
            {
                // IMPORTANT: this is a known bug in OWIN/ASP.NET with CookieAuthentication.
                // http://katanaproject.codeplex.com/discussions/540202
                // https://connect.microsoft.com/VisualStudio/Feedback/Details/3065110
                // https://github.com/aspnet/AspNetKatana/issues/74

                Log.Info("Global.Application_Error: Unexpected exception", ex);
                Server.ClearError();
            }
            else
            {
                Log.Error("Application_Error: Unexpected exception", ex);
            }
        }
    }
}
