using BusinessAsUsual.Admin.Services.Health;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for monitoring the application's health and resource usage.
    /// </summary>
    /// <remarks>This controller is intended for use within the Admin area and exposes endpoints for
    /// retrieving health status and system statistics, such as CPU, memory, disk usage, and uptime. Access to these
    /// endpoints should be restricted to authorized users, as they may expose sensitive operational
    /// information.</remarks>
    [Area("Admin")]
    [Route("Admin/health")]
    public class HealthController : Controller
    {
        private readonly IHealthMetricsProvider _metrics;

        /// <summary>
        /// Initializes a new instance of the HealthController class with the specified health metrics provider and
        /// logger.
        /// </summary>
        /// <param name="metrics">The provider used to retrieve health metrics for the application. Cannot be null.</param>
        public HealthController(IHealthMetricsProvider metrics)
        {
            _metrics = metrics;
        }

        /// <summary>
        /// Handles HTTP GET requests to the root endpoint and returns a JSON response indicating service status.
        /// </summary>
        /// <returns>A JSON result containing a status message with the value "OK".</returns>
        [HttpGet("")]
        public IActionResult Index()
        {
            return Json(new {status = "OK"});
        }

        /// <summary>
        /// Retrieves the current application metrics and returns them as a JSON response.
        /// </summary>
        /// <remarks>This endpoint provides a snapshot of various runtime statistics for monitoring or
        /// diagnostic purposes. The structure of the returned JSON depends on the implementation of the metrics
        /// provider. This action does not require authentication unless configured by the application.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the serialized metrics data in JSON format.</returns>
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            return Json(_metrics.GetStats());
        }
    }
}
