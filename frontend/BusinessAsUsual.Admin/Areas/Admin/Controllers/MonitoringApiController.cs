using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides API endpoints for monitoring and reporting the health status of the platform.
    /// </summary>
    /// <remarks>The MonitoringController exposes endpoints intended for use by health check systems and
    /// monitoring tools. Clients can use these endpoints to verify the operational status of the platform and integrate
    /// with external monitoring solutions. All endpoints are accessible under the 'api/monitoring' route.</remarks>
    [ApiController]
    [Route("api/monitoring")]
    public class MonitoringApiController : ControllerBase
    {
        private readonly IMonitoringService _monitoring;

        /// <summary>
        /// Initializes a new instance of the MonitoringController class using the specified monitoring service.
        /// </summary>
        /// <remarks>This constructor requires an implementation of the IMonitoringService interface.
        /// Passing a null value for the monitoring parameter will result in an exception.</remarks>
        /// <param name="monitoring">The monitoring service that provides monitoring operations. This parameter cannot be null.</param>
        public MonitoringApiController(IMonitoringService monitoring)
        {
            _monitoring = monitoring;
        }

        /// <summary>
        /// Retrieves the current health status of the platform.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to obtain platform health information.
        /// Ensure that the monitoring service is properly configured to provide accurate health data.</remarks>
        /// <returns>An IActionResult containing the health status of the platform. The result indicates whether the platform is
        /// operational and may include relevant health metrics.</returns>
        [HttpGet("platform-health")]
        public async Task<IActionResult> GetPlatformHealth()
        {
            var result = await _monitoring.GetPlatformHealthAsync();
            return Ok(result);
        }
    }
}
