using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Web.Controllers
{
    /// <summary>
    /// Provides endpoints for simulating error responses, such as 500 Internal Server Error and 404 Not Found, to
    /// facilitate testing of error handling in the application.
    /// </summary>
    /// <remarks>Use this controller to verify how the application responds to specific error scenarios during
    /// development or testing. The endpoints intentionally trigger error responses and should not be exposed in
    /// production environments.</remarks>
    [Route("error")]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Throws an exception to simulate an internal server error for testing error handling in the application.
        /// </summary>
        /// <remarks>Use this method in development or test environments to verify that error handling and
        /// error pages are correctly configured for HTTP 500 responses.</remarks>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown every time the method is called to indicate an intentional server error for testing purposes.</exception>
        [HttpGet("500")]
        public IActionResult Throw500()
        {
            throw new Exception("Intentional test exception from Web");
        }

        /// <summary>
        /// Returns a 404 Not Found response to indicate that the requested resource is unavailable by design.
        /// </summary>
        /// <remarks>This method is typically used for testing error handling or simulating missing
        /// resources in web applications. It can be accessed via a GET request to the '404' endpoint.</remarks>
        /// <returns>An <see cref="IActionResult"/> representing the 404 Not Found response with a message indicating the error
        /// is intentional.</returns>
        [HttpGet("404")]
        public IActionResult Throw404()
        {
            return NotFound("Intentional 404 from Web");
        }
    }
}
