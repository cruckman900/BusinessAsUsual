using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Represents the controller responsible for handling requests to the application's home page.
    /// </summary>
    /// <remarks>This controller typically serves the default landing page of the application. It inherits
    /// from <see cref="Controller"/>, providing standard MVC controller functionality.</remarks>
    public class HomeController : Controller
    {
        /// <summary>
        /// Handles requests for the default page of the controller.
        /// </summary>
        /// <returns>A view that renders the default page.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
