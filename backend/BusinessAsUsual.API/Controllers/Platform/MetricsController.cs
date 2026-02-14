using BusinessAsUsual.Application.Platform.Metrics;
using BusinessAsUsual.Domain.Platform.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.API.Controllers.Platform
{
    [ApiController]
    [Route("api/metrics")]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsService _metrics;

        public MetricsController(IMetricsService metrics)
        {
            _metrics = metrics;
        }

        [HttpGet("system")]
        public async Task<ActionResult<SystemMetrics>> GetSystemMetrics()
        {
            var metrics = await _metrics.GetSystemMetricsAsync();
            return Ok(metrics);
        }
   }
}
