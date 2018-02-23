using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DataLayer;
using Microsoft.AspNet.Identity.Owin;
using WebApp.Models.Auth;
using WebApp.Services;

namespace WebApp.Controllers.Api
{
    public class AuthController : ApiController
    {
        #region Private fields

        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;

        #endregion

        #region Constructor

        public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #endregion

        #region Public methods

        [Route("api/auth/login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username ?? string.Empty, model.Password ?? string.Empty, model.RememberMe, true);
            if (result == SignInStatus.Failure)
            {
                var user = await _userManager.FindByEmailAsync(model.Username ?? string.Empty);
                if (user == null)
                    return AuthFailed();

                result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password ?? string.Empty, model.RememberMe, true);
            }

            switch (result)
            {
                case SignInStatus.Success:
                    var userId = GetCurrentUserId();
                    if (userId != null && !await _userManager.IsEmailConfirmedAsync(userId))
                    {
                        await SendConfirmEmail(userId);
                        return AuthFailed("E-mail confirmation required.\r\nThe confirmation token has been resent to your email account");
                    }

                    return AuthSucceeded();

                case SignInStatus.LockedOut:
                    return AuthFailed("Account is temporarily locked");

                default:
                    return AuthFailed();
            }
        }

        [Route("api/auth/logoff")]
        [HttpPost]
        public IHttpActionResult Logoff()
        {
            _signInManager.SignOut();
            return Ok();
        }

        [Route("api/auth/signup")]
        [HttpPost]
        public async Task<IHttpActionResult> Signup(SignupModel model)
        {
            var user = new AppUser
            {
                UserName = model.Username ?? string.Empty,
                Email = model.Email ?? string.Empty
            };

            var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
            if (!result.Succeeded)
                return AuthFailed(result.Errors.ToArray());

            await SendConfirmEmail(user.Id);
            return Succeded();
        }

        [Route("api/auth/confirm")]
        [HttpGet]
        public async Task<IHttpActionResult> ConfirmEmail(string id, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(id, code);
            var url = new Uri(Request.RequestUri, result.Succeeded ? "/confirm-success" : "/confirm-failure").ToString();
            return Redirect(url);
        }

        [Route("api/auth/acquire-password-reset")]
        [HttpPost]
        public async Task<IHttpActionResult> AcquirePasswordReset(AcquirePasswordResetModel model)
        {
            var username = model.Username ?? string.Empty;
            var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user.Id))
                return AuthFailed("User not found");

            await SendResetPasswordEmail(user.Id);
            return Succeded();
        }

        [Route("api/auth/reset-password")]
        [HttpPost]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var result = await _userManager.ResetPasswordAsync(model.Id ?? string.Empty, model.Code ?? string.Empty, model.Password ?? string.Empty);
                if (!result.Succeeded)
                    return AuthFailed(result.Errors.ToArray());

                return Succeded();
            }
            catch (InvalidOperationException ex) when (ex.Message == "UserId not found.")
            {
                return AuthFailed("Password reset token has already expired");
            }
        }

        #endregion

        #region Private methods

        private string GetCurrentUserId()
        {
            return _signInManager.AuthenticationManager.AuthenticationResponseGrant.Identity.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        private IHttpActionResult Succeded()
        {
            return Ok(new AuthResponse {Success = true});
        }

        private IHttpActionResult AuthSucceeded()
        {
            return Ok(new AuthResponse
            {
                Success = true,
                Message = null,
                Username = _signInManager.AuthenticationManager.AuthenticationResponseGrant.Identity.Name
            });
        }

        private IHttpActionResult AuthFailed(params string[] errors)
        {
            var message = errors.Length == 0 ? "Invalid username and/or password" : string.Join("<br>", errors);

            return Ok(new AuthResponse
            {
                Success = false,
                Message = message,
                Username = null
            });
        }

        public async Task SendConfirmEmail(string userId)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userId);
            var url = new Uri(Request.RequestUri, "/api/auth/confirm").ToString();
            url += "?id=" + HttpUtility.UrlEncode(userId);
            url += "&code=" + HttpUtility.UrlEncode(code);

            var body = $"Please confirm your account by clicking <a href=\"{url}\">here</a>";
            await _userManager.SendEmailAsync(userId, "dqdv.org: Confirm your account", body);
        }

        public async Task SendResetPasswordEmail(string userId)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(userId);
            var url = new Uri(Request.RequestUri, "/reset-password").ToString();
            url += "?id=" + HttpUtility.UrlEncode(userId);
            url += "&code=" + HttpUtility.UrlEncode(code);

            var body = $"Please reset your password by clicking <a href=\"{url}\">here</a>";
            await _userManager.SendEmailAsync(userId, "dqdv.org: Password reset", body);
        }

        #endregion
    }
}
