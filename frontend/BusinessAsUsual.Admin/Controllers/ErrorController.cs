using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Provides endpoints for simulating HTTP error responses, such as 500 Internal Server Error and 404 Not Found, to
    /// facilitate testing of error handling in client applications.
    /// </summary>
    /// <remarks>Use this controller to intentionally trigger server and not found errors during development
    /// or automated testing. These endpoints are not intended for production use and should be secured or removed in
    /// production environments to prevent misuse.</remarks>
    [Route("error")]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Throws an exception to simulate an internal server error for testing error handling scenarios.
        /// </summary>
        /// <remarks>Use this method in development or test environments to verify that error handling and
        /// logging mechanisms respond correctly to unhandled exceptions.</remarks>
        /// <returns></returns>
        /// <exception cref="Exception"><exception cref="System.Exception">Always thrown to simulate a server error for testing
        /// purposes.</exception></exception>
        [HttpGet("500")]
        public IActionResult Throw500()
        {
            throw new Exception("Intentional test exception from Admin");
        }

        /// <summary>
        /// Returns a 404 Not Found response with a custom message for testing or demonstration purposes.
        /// </summary>
        /// <remarks>This method is typically used to simulate a not found error, allowing developers to
        /// verify error handling and user experience for missing resources within the application.</remarks>
        /// <returns>An <see cref="IActionResult"/> that produces a 404 Not Found response containing a custom error message.</returns>
        [HttpGet("404")]
        public IActionResult Throw404()
        {
            return NotFound("Intentional 404 from Admin");
        }
    }
}
