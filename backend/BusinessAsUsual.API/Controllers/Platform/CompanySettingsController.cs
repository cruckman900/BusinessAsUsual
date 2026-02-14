using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.API.Controllers.Platform
{
    public class CompanySettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
