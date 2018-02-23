using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DataLayer;
using Microsoft.AspNet.Identity;
using WebApp.Models.Feedback;
using WebApp.Services;

namespace WebApp.Controllers.Api
{
    [Authorize]
    public class FeedbackController : ApiController
    {
        #region Private fields

        private readonly ApplicationUserManager _userManager;
        private readonly FeedbackStorage _storage;

        #endregion

        #region Constructor

        public FeedbackController(ApplicationUserManager userManager, FeedbackStorage storage)
        {
            _userManager = userManager;
            _storage = storage;
        }

        #endregion

        #region Public methods

        [Route("api/feedback")]
        [HttpPost]
        public async Task<IHttpActionResult> Create()
        {
            var model = BindSendFeedbackModel();
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            var name = string.Empty;
            var url = string.Empty;
            if (model.File != null)
            {
                name = Guid.NewGuid().ToString();
                url = await _storage.Save(model.File, name);
            }

            await Send(GenerateBody(user, model, name, url));
            return Ok();
        }

        #endregion

        #region Private methods

        private SendFeedbackModel BindSendFeedbackModel()
        {
            var model = new SendFeedbackModel
            {
                Comment = HttpContext.Current.Request.Form["comment"]
            };

            if (string.IsNullOrWhiteSpace(model.Comment))
                ModelState.AddModelError("comment", "'Comment' is required");

            var files = HttpContext.Current.Request.Files;
            if (files.Count > 1)
                ModelState.AddModelError("file", "Only one file is allowed");
            else if (files.Count == 1)
                model.File = files[0];

            return model;
        }

        private static string GenerateBody(AppUser user, SendFeedbackModel model, string name, string url)
        {
            var body = $"Username: {HttpUtility.HtmlEncode(user.UserName)}<br />";
            body += $"E-mail: {HttpUtility.HtmlEncode(user.Email)}<br />";
            body += $"User Id: {HttpUtility.HtmlEncode(user.Id)}<br />";
            body += $"UTC Time: {DateTime.UtcNow}<br />";

            if (model.File == null)
            {
                body += "Attachment: No<br />";
            }
            else
            {
                body += $"Attachment: {HttpUtility.HtmlEncode(model.File.FileName)}<br />";
                body += $"Blob name: {HttpUtility.HtmlEncode(name)}<br />";
                body += $"Blob url: {HttpUtility.HtmlEncode(url)}<br />";
            }

            body += "Comment:<br />";
            body += HttpUtility.HtmlEncode(model.Comment)?.Replace("\r\n", "<br />");
            return body;
        }

        private static async Task Send(string body)
        {
            using (var msg = new MailMessage())
            {
                msg.To.Add(new MailAddress("support@dqdv.org"));
                msg.From = new MailAddress("info@dqdv.org");
                msg.Subject = "Feedback from a user";
                msg.IsBodyHtml = true;
                msg.Body = body;

                using (var client = new SmtpClient())
                    await client.SendMailAsync(msg);
            }
        }

        #endregion
    }
}
