using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BusinessAsUsual.Admin.Attributes
{
    /// <summary>
    /// Custom authorization attribute for admin routes.
    /// Redirects unauthenticated users to the home page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AdminAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var isAuthenticated = session.GetString("IsAuthenticated") == "true";

            if (!isAuthenticated)
            {
                // Store the original request path for redirect after login
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Index", "Home", new { returnUrl });
            }
        }
    }
}
