using BusinessAsUsual.Application.Platform.Health;
using BusinessAsUsual.Domain.Platform.Health;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.API.Controllers.Platform
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly IHealthService _health;

        public HealthController(IHealthService health)
        {
            _health = health;
        }

        [HttpGet]
        public ActionResult<HealthStats> GetHealth() => Ok(_health.GetHealth());
    }
}
