using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.API.Controllers
{
    /// <summary>
    /// Provides API endpoints for simulating error responses to facilitate client-side error handling and testing.
    /// </summary>
    /// <remarks>This controller exposes endpoints that intentionally return HTTP 500 Internal Server Error
    /// and HTTP 404 Not Found responses. These endpoints are intended for use in development and testing scenarios to
    /// verify how client applications handle various error conditions.</remarks>
    [ApiController]
    [Route("error")]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Throws an exception to simulate a server error for testing purposes.
        /// </summary>
        /// <remarks>This method is typically used in testing scenarios to verify error handling in
        /// clients.</remarks>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown to indicate an intentional test exception from the API.</exception>
        [HttpGet("500")]
        public IActionResult Throw500()
        {
            throw new Exception("Intentional test exception from API");
        }

        /// <summary>
        /// Returns a 404 Not Found response to simulate a missing resource or for testing error handling in the API.
        /// </summary>
        /// <remarks>This method can be used to verify client-side handling of 404 errors or to test error
        /// middleware in the application.</remarks>
        /// <returns>An <see cref="IActionResult"/> that produces a 404 Not Found response with a message indicating the error is
        /// intentional.</returns>
        [HttpGet("404")]
        public IActionResult Throw404()
        {
            return NotFound("Intentional 404 from API");
        }
    }
}
