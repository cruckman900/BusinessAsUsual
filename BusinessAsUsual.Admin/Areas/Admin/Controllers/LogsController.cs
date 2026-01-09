using Microsoft.AspNetCore.Mvc;
using BusinessAsUsual.Admin.Services.Logs;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for viewing and retrieving application log entries.
    /// </summary>
    /// <remarks>This controller is intended for use within the administrative area of the application. Access
    /// to its actions may be restricted to authorized users. The controller supports viewing logs in the browser and
    /// retrieving log data in JSON format for further analysis or display.</remarks>
    [Area("Admin")]
    [Route("Admin/logs")]
    public class LogsController : Controller
    {
        private readonly ILogReader _logReader;

        /// <summary>
        /// Initializes a new instance of the LogsController class using the specified log reader.
        /// </summary>
        /// <param name="logReader">The log reader used to retrieve log entries for this controller. Cannot be null.</param>
        public LogsController(ILogReader logReader)
        {
            _logReader = logReader;
        }

        /// <summary>
        /// Handles HTTP GET requests for the root URL and returns the default view for the controller.
        /// </summary>
        /// <returns>A view result that renders the default view associated with the action.</returns>
        [HttpGet("")]
        public IActionResult Index() => View();

        /// <summary>
        /// Retrieves a collection of log entries filtered by log level, search term, and result limit.
        /// </summary>
        /// <param name="level">The log level to filter by (for example, "Error", "Warning", or "Information"). If null, logs of all levels
        /// are included.</param>
        /// <param name="search">A search term to filter log messages. If null or empty, no search filtering is applied.</param>
        /// <param name="limit">The maximum number of log entries to return. Must be a positive integer. The default is 200.</param>
        /// <returns>An <see cref="IActionResult"/> containing the filtered log entries in JSON format.</returns>
        [HttpGet("data")]
        public async Task<IActionResult> GetLogs(
            string? level, string? search, int limit = 200, int page = 1
        )
        {
            var query = new LogQuery
            {
                Level = level,
                Search = search,
                Limit = limit,
                Page = page
            };

            var logs = await _logReader.GetLogsAsync(query);
            return Json(logs);
        }
    }
}
