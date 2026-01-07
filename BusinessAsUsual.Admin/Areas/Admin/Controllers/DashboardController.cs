using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides actions for displaying the administrative dashboard within the Admin area of the application.
    /// </summary>
    /// <remarks>This controller is designated for the Admin area and is typically used to render views
    /// related to administrative overview and management. Access to this controller may require appropriate
    /// authorization depending on application configuration.</remarks>
    [Area("Admin")]
    public class DashboardController : Controller
    {
        /// <summary>
        /// Returns the default view for the Index page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Index view.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
