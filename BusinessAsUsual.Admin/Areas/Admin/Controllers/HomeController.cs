using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers.Admin
{
    /// <summary>
    /// Admin landing page
    /// </summary>
    [Area("Admin")]
    public class HomeController : Controller
    {
        /// <summary>
        /// home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}