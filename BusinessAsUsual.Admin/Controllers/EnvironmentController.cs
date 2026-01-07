using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Provides API endpoints for retrieving environment configuration and status information for administrative
    /// purposes.
    /// </summary>
    /// <remarks>This controller is intended for use by administrative clients to access environment-related
    /// data. All routes are prefixed with 'admin/environment'.</remarks>
    [ApiController]
    [Route("admin/environment")]
    public class EnvironmentController : ControllerBase
    {
        private readonly EnvironmentService _env;

        /// <summary>
        /// Initializes a new instance of the EnvironmentController class with the specified environment service.
        /// </summary>
        /// <param name="env">The environment service to be used by the controller. Cannot be null.</param>
        public EnvironmentController(EnvironmentService env)
        {
            _env = env;
        }

        /// <summary>
        /// Handles HTTP GET requests and returns environment information for the application.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the environment information. The result has a status code of 200
        /// (OK) and includes the environment details in the response body.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var info = _env.GetEnvironmentInfo();
            return Ok(info);
        }
    }
}
