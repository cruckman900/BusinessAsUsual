using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.API.Controllers.Platform
{
    public class CompanyProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
