using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides actions for displaying and managing application settings in the web application.
    /// </summary>
    [Area("Admin")]
    public class SettingsController : Controller
    {
        /// <summary>
        /// Returns the default view for the Index page.
        /// </summary>
        /// <returns>An <see cref="ViewResult"/> that renders the Index view.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
