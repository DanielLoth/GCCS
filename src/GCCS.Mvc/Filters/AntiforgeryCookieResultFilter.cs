using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GCCS.Mvc.Filters
{
    public class AntiforgeryCookieResultFilter : ResultFilterAttribute
    {
        /// <summary>
        /// The cookie options on this occasion specify HttpOnly = false.
        /// This makes the cookie readable by Angular, which then allows it to attach
        /// the relevant anti-forgery (anti-CSRF / anti-XSRF) token when making requests.
        /// </summary>
        private static readonly CookieOptions _options = new CookieOptions { HttpOnly = false };

        private readonly IAntiforgery _antiforgery;

        public AntiforgeryCookieResultFilter(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Delete("XSRF-TOKEN");
            var tokens = _antiforgery.GetAndStoreTokens(context.HttpContext);
            context.HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, _options);
            //if (context.Result is ViewResult)
            //{
            //    var tokens = _antiforgery.GetAndStoreTokens(context.HttpContext);
            //    context.HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, _options);
            //}
        }
    }
}
