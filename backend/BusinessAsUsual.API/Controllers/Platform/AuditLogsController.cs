using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.API.Controllers.Platform
{
    public class AuditLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
