using Microsoft.AspNetCore.Mvc;
using BusinessAsUsual.Admin.Models;
using BusinessAsUsual.Admin.Services;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides API endpoints for retrieving and updating system settings in the administration area.
    /// </summary>
    /// <remarks>This controller is intended for use by administrative clients to manage application-wide
    /// configuration. All routes are prefixed with 'Admin/'. Access to these endpoints may require appropriate
    /// authorization, depending on application configuration.</remarks>
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [ApiController]
    public class SettingsApiController : ControllerBase
    {
        private readonly SystemSettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the SettingsApiController class using the specified system settings service.
        /// </summary>
        /// <param name="settingsService">The service used to manage and retrieve system settings. Cannot be null.</param>
        public SettingsApiController(SystemSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Retrieves the current system settings.
        /// </summary>
        /// <remarks>This endpoint is typically used to obtain configuration values required by clients or
        /// administrative tools. The returned settings reflect the current state as loaded from the underlying settings
        /// service.</remarks>
        /// <returns>An <see cref="ActionResult{T}"/> containing the current <see cref="SystemSettings"/>. Returns an HTTP 200
        /// response with the settings if successful.</returns>
        [HttpGet]
        public ActionResult<SystemSettings> Get()
        {
            var settings = _settingsService.Load();
            return Ok(settings);
        }

        /// <summary>
        /// Saves the specified system settings.
        /// </summary>
        /// <remarks>This action is typically called by clients to update application-wide configuration.
        /// The request body must contain a valid <see cref="SystemSettings"/> object.</remarks>
        /// <param name="settings">The system settings to be saved. Cannot be null.</param>
        /// <returns>An <see cref="OkResult"/> indicating that the settings were saved successfully.</returns>
        [HttpPost]
        public IActionResult Save([FromBody] SystemSettings settings)
        {
            _settingsService.Save(settings);
            return Ok();
        }
    }
}
