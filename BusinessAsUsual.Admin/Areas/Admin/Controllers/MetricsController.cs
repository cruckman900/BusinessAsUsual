using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for viewing application metrics within the Admin area.
    /// </summary>
    /// <remarks>This controller is intended for use by administrators to access metrics-related views. It is
    /// registered under the "Admin" area, which may restrict access based on routing or authorization
    /// policies.</remarks>
    [Area("Admin")]
    public class MetricsController : Controller
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
