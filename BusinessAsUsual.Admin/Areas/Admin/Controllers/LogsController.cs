using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Represents an ASP.NET Core MVC controller that manages administrative log-related views and actions.
    /// </summary>
    /// <remarks>This controller is intended for use within the 'Admin' area of the application. Access to its
    /// actions may require appropriate administrative authorization, depending on the application's
    /// configuration.</remarks>
    [Area("Admin")]
    public class LogsController : Controller
    {
        /// <summary>
        /// Returns the default view for the Index page.
        /// </summary>
        /// <returns>A view that renders the Index page.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
