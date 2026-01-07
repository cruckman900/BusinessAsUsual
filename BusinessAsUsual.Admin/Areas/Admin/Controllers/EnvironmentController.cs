using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides administrative actions related to environment management within the application.
    /// </summary>
    /// <remarks>This controller is part of the 'Admin' area and is intended for use by users with
    /// administrative privileges. It serves views and actions related to environment configuration or monitoring.
    /// Access to this controller may be restricted based on user roles or authentication settings.</remarks>
    [Area("Admin")]
    public class EnvironmentController : Controller
    {
        /// <summary>
        /// Returns the default view for the Index action.
        /// </summary>
        /// <returns>A view that renders the default page for this action.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
