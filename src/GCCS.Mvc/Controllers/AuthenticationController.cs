using System.Threading.Tasks;
using GCCS.Mvc.Security;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GCCS.Mvc.Controllers
{
    public class AuthenticationController : Controller
    {
        public const string LoginRoute = "/authentication/login";
        public const string LogoutRoute = "/authentication/logout";
        public const string RenewXsrfTokenRoute = "/authentication/renew-xsrf-token";
        public const string SinglePageAppHeader = "X-RACI-Angular";

        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IAntiforgery Antiforgery;

        /// <summary>
        /// The cookie options on this occasion specify HttpOnly = false.
        /// This makes the cookie readable by Angular, which then allows it to attach
        /// the relevant anti-forgery (anti-CSRF / anti-XSRF) token when making requests.
        /// </summary>
        private static readonly CookieOptions Options = new CookieOptions { HttpOnly = false };

        public AuthenticationController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAntiforgery antiforgery)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            Antiforgery = antiforgery;
        }

        [AllowAnonymous]
        [HttpGet(LoginRoute)]
        public async Task<IActionResult> Login(string returnUrl)
        {
            await SignInManager.SignOutAsync();

            var user = await UserManager.FindByNameAsync(@"DESKTOP-MPE15R2\daniel");
            await SignInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(RenewXsrfToken), new { returnUrl });
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpGet(LogoutRoute)]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();

            return RedirectToAction(nameof(RenewXsrfToken), new { string.Empty });
        }

        [AllowAnonymous]
        [HttpGet(RenewXsrfTokenRoute)]
        public IActionResult RenewXsrfToken(string returnUrl)
        {
            var tokens = Antiforgery.GetAndStoreTokens(HttpContext);

            HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, Options);

            var hasReturnUrl = !string.IsNullOrWhiteSpace(returnUrl);

            if (hasReturnUrl && Url.IsLocalUrl(returnUrl))
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
            }
            else if (SinglePageAppHeaderPresent())
            {
                return Ok();
            }

            return LocalRedirect("/");
        }

        private bool SinglePageAppHeaderPresent()
        {
            var hasAngularHeader = Request.Headers.TryGetValue(SinglePageAppHeader, out _);

            return hasAngularHeader;
        }
    }
}