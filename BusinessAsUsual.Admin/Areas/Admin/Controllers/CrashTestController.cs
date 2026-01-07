using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Represents an ASP.NET Core MVC controller for administering crash test functionality within the application.
    /// </summary>
    /// <remarks>This controller is associated with the "Admin" area and is intended for use in administrative
    /// scenarios. It provides endpoints related to crash test operations, typically for testing or diagnostic purposes
    /// within the administrative interface.</remarks>
    [Area("Admin")]
    public class CrashTestController : Controller
    {
        /// <summary>
        /// Returns the default view for the Index action.
        /// </summary>
        /// <returns>A <see cref="ViewResult"/> that renders the default view for this action.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
