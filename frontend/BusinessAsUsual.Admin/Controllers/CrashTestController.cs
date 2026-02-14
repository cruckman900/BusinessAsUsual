using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for testing error handling scenarios, including simulated exceptions, forced
    /// error responses, timeouts, dependency failures, validation errors, and logging behaviors. Intended for use in
    /// development or diagnostic environments to verify application and infrastructure error handling.
    /// </summary>
    /// <remarks>These endpoints are designed to trigger specific error conditions for testing purposes and
    /// should not be exposed in production environments. Each action simulates a distinct failure mode, such as handled
    /// and unhandled exceptions, forced HTTP 500 responses, artificial timeouts, dependency failures, validation
    /// errors, and error logging without exception. Use these endpoints to validate monitoring, logging, and error
    /// response mechanisms in your application pipeline.</remarks>
    [ApiController]
    [Route("admin/test")]
    public class CrashTestController : ControllerBase
    {
        private readonly ILogger<CrashTestController> _logger;

        /// <summary>
        /// Initializes a new instance of the CrashTestController class with the specified logger.
        /// </summary>
        /// <param name="logger">The logger instance used to record diagnostic and operational information for the controller. Cannot be
        /// null.</param>
        public CrashTestController(ILogger<CrashTestController> logger)
        {
            _logger = logger;
        }

        // 1. Handled exception (logged, but returns 200)
        /// <summary>
        /// Handles a test exception by logging the error and returning a successful response.
        /// </summary>
        /// <remarks>This endpoint demonstrates exception handling by catching and logging an exception,
        /// then returning a 200 OK response. The exception is not exposed to the client.</remarks>
        /// <returns>An <see cref="OkObjectResult"/> containing a message indicating that the exception was logged.</returns>
        /// <exception cref="InvalidOperationException">This method handles <see cref="InvalidOperationException"/> internally and does not propagate it to the
        /// caller.</exception>
        [HttpPost("handled-exception")]
        public IActionResult HandledException()
        {
            try
            {
                throw new InvalidOperationException("This is a handled test exception.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handled exception triggered.");
                return Ok(new { message = "Handled exception logged." });
            }
        }

        // 2. Unhandled exception (crashes the request)
        /// <summary>
        /// Throws an unhandled exception to simulate a server error response for testing purposes.
        /// </summary>
        /// <remarks>Use this endpoint to verify error handling and middleware behavior for unhandled
        /// exceptions in the application pipeline.</remarks>
        /// <returns>This method does not return a value; it always throws an exception.</returns>
        /// <exception cref="Exception">Thrown unconditionally to indicate an unhandled error occurred during the request.</exception>
        [HttpPost("unhandled-exception")]
        public IActionResult UnhandledException()
        {
            throw new Exception("This is an unhandled test exception.");
        }

        // 3. Force a 500 response
        /// <summary>
        /// Forces the API to return an HTTP 500 Internal Server Error response with a predefined error message.
        /// </summary>
        /// <remarks>This method is intended for testing error handling scenarios. The response body
        /// includes a JSON object with a single 'message' property describing the error.</remarks>
        /// <returns>An <see cref="IActionResult"/> that produces a 500 Internal Server Error response containing a JSON object
        /// with an error message.</returns>
        [HttpPost("500")]
        public IActionResult Force500()
        {
            return StatusCode(500, new { message = "Forced 500 error." });
        }

        // 4. Timeout (simulate hung dependency)
        /// <summary>
        /// Simulates a long-running operation by delaying the response for 30 seconds.
        /// </summary>
        /// <remarks>This method can be used to test client-side timeout handling or to simulate scenarios
        /// where a dependent service is unresponsive. The response is always successful unless the request is cancelled
        /// before completion.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a message indicating that the timeout simulation has completed.</returns>
        [HttpPost("timeout")]
        public async Task<IActionResult> Timeout()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            return Ok(new { message = "Timeout completed (unexpected)." });
        }

        // 5. Dependency failure (simulate DB or API failure)
        /// <summary>
        /// Simulates a dependency failure by throwing an exception to represent an unreachable external service.
        /// </summary>
        /// <remarks>Use this endpoint to test error handling for scenarios where external dependencies
        /// fail. The method always throws an exception and does not perform any actual operation.</remarks>
        /// <returns>This method does not return a result; it always throws an exception.</returns>
        /// <exception cref="HttpRequestException">Thrown to indicate that a simulated dependency, such as an external API or database, is unreachable.</exception>
        [HttpPost("dependency-failure")]
        public IActionResult DependencyFailure()
        {
            throw new HttpRequestException("Simulated dependency failure: API unreachable.");
        }

        // 6. Validation failure (400)
        /// <summary>
        /// Simulates a validation failure by returning a 400 Bad Request response with a model validation error.
        /// </summary>
        /// <remarks>This method adds a sample validation error to the model state and returns a
        /// standardized validation problem response. It can be used to test client handling of validation errors in API
        /// requests.</remarks>
        /// <returns>An <see cref="IActionResult"/> that produces a 400 Bad Request response containing validation problem
        /// details.</returns>
        [HttpPost("validation-failure")]
        public IActionResult ValidationFailure()
        {
            ModelState.AddModelError("Field", "Simulated validation error.");
            return ValidationProblem(ModelState);
        }

        // 7. Logging-only error (no exception thrown)
        /// <summary>
        /// Logs an error message without throwing an exception and returns a confirmation response.
        /// </summary>
        /// <remarks>This method is intended for scenarios where an error should be recorded in the
        /// application logs without interrupting the request flow or returning an error status to the client.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a confirmation message indicating that the error log was written.</returns>
        [HttpPost("log-error")]
        public IActionResult LogError()
        {
            _logger.LogError("Simulated error log without exception.");
            return Ok(new { message = "Error log written." });
        }
    }
}