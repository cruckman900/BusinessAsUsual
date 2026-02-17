using BusinessAsUsual.Admin.Dtos;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides endpoints for monitoring platform, service, and infrastructure health within the admin area of the
    /// application.
    /// </summary>
    /// <remarks>The controller exposes both view-based and API endpoints to support real-time health
    /// monitoring and dynamic updates in the administrative interface. It is intended for use by administrators to
    /// assess the operational status of various system components. Endpoints are designed to facilitate AJAX calls and
    /// API polling for up-to-date health information.</remarks>
    [Area("Admin")]
    public class MonitoringController : Controller
    {
        private readonly IMonitoringService _monitoring;

        /// <summary>
        /// Initializes a new instance of the MonitoringController class using the specified monitoring service.
        /// </summary>
        /// <remarks>Use this constructor to provide an implementation of IMonitoringService, enabling the
        /// controller to perform monitoring-related actions. The supplied service must be valid and is required for the
        /// controller's functionality.</remarks>
        /// <param name="monitoring">The monitoring service to be used for performing monitoring operations. This parameter cannot be null.</param>
        public MonitoringController(IMonitoringService monitoring)
        {
            _monitoring = monitoring;
        }

        // ---------------------------------------------------------
        // MAIN PAGE
        // ---------------------------------------------------------

        /// <summary>
        /// Renders the main monitoring dashboard view, which serves as the entry point for administrators to access
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        // ---------------------------------------------------------
        // AJAX ENDPOINTS FOR VIEW COMPONENT REFRESH
        // ---------------------------------------------------------

        /// <summary>
        /// Processes a platform health status update and returns the corresponding view component for display.
        /// </summary>
        /// <remarks>This method is intended to be called via an AJAX POST request to refresh the platform
        /// status section of the user interface.</remarks>
        /// <param name="model">A <see cref="PlatformHealthDto"/> instance containing the current health status information of the platform.
        /// This data is used to render the platform status view component.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the platform status view component with the provided health
        /// data.</returns>
        [HttpPost]
        public IActionResult PlatformStatus([FromBody] PlatformHealthDto model)
        {
            if (model == null)
                return Content("MODEL IS NULL");
            return ViewComponent("PlatformStatus", model);
        }

        /// <summary>
        /// Processes a platform health status request and returns a view component that displays the current service
        /// health.
        /// </summary>
        /// <remarks>The <paramref name="model"/> parameter is expected to contain all necessary health
        /// information for the platform. If the model is invalid or missing required data, the resulting view may not
        /// display accurate health status.</remarks>
        /// <param name="model">A <see cref="PlatformHealthDto"/> object containing the health status data for the platform. This parameter
        /// must not be null.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the service health view component with the provided health data.</returns>
        [HttpPost]
        public IActionResult ServiceHealth([FromBody] PlatformHealthDto model)
        {
            if (model == null)
                return Content("MODEL IS NULL"); 
            return ViewComponent("ServiceHealth", model);
        }

        /// <summary>
        /// Handles HTTP POST requests to retrieve the infrastructure health status based on the provided model.
        /// </summary>
        /// <remarks>This method is typically used to assess the current health of the infrastructure and
        /// may return different views based on the model's state.</remarks>
        /// <param name="model">The model containing the health status information for the platform. This parameter cannot be null.</param>
        /// <returns>An IActionResult that renders the 'InfrastructureHealth' view component with the provided model.</returns>
        [HttpPost]
        public IActionResult InfrastructureHealth([FromBody] PlatformHealthDto model)
        {
            if (model == null)
                return Content("MODEL IS NULL"); 
            return ViewComponent("InfrastructureHealth", model);
        }

        /// <summary>
        /// Processes a request to display the details of a specific alarm by its title.
        /// </summary>
        /// <remarks>This method expects a valid, non-null title to be provided in the request body. If
        /// the title is null or empty, the resulting view may not display the intended alarm details.</remarks>
        /// <param name="title">The title of the alarm to be displayed in the detail view. Cannot be null.</param>
        /// <returns>An IActionResult that renders the AlarmDetail view component for the specified alarm.</returns>
        [HttpPost]
        public IActionResult AlarmDetail([FromBody] string title)
        {
            return ViewComponent("AlarmDetail", new { title });
        }

        // ---------------------------------------------------------
        // API ENDPOINT FOR POLLING (JS fetches this)
        // ---------------------------------------------------------

        /// <summary>
        /// Retrieves the current health status of the platform for monitoring purposes.
        /// </summary>
        /// <remarks>This endpoint is intended to be polled by client applications to monitor the
        /// operational health of the platform. The response provides a snapshot of the platform's current status and is
        /// suitable for integration with automated monitoring tools.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the platform's health status as a data transfer object (DTO).</returns>
        [HttpGet]
        [Route("/api/monitoring/platform-health")]
        public async Task<IActionResult> GetPlatformHealth()
        {
            var dto = await _monitoring.GetPlatformHealthAsync();
            if (dto == null)
                return Content("DTO IS NULL");
            return Ok(dto);
        }
    }
}
