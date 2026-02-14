using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Controllers
{
    /// <summary>
    /// Provides administrative endpoints for querying and downloading application log files.
    /// </summary>
    /// <remarks>This controller is intended for use by administrative tools or users with appropriate
    /// permissions. It exposes endpoints to search log entries and to download the latest log file for diagnostic or
    /// auditing purposes.</remarks>
    [ApiController]
    [Route("admin/logs")]
    public class LogsController : ControllerBase
    {
        private readonly LogQueryService _logs;

        /// <summary>
        /// Initializes a new instance of the LogsController class using the specified log query service.
        /// </summary>
        /// <param name="logs">The service used to query and retrieve log data.</param>
        public LogsController(LogQueryService logs)
        {
            _logs = logs;
        }

        /// <summary>
        /// Retrieves a paginated list of log entries filtered by log level, service name, and search term.
        /// </summary>
        /// <param name="level">The log level to filter results by. If null, entries of all levels are included.</param>
        /// <param name="service">The name of the service to filter results by. If null, entries from all services are included.</param>
        /// <param name="search">A search term to filter log messages. If null or empty, no search filtering is applied.</param>
        /// <param name="page">The page number of results to return. Must be greater than or equal to 1.</param>
        /// <param name="pageSize">The number of log entries to include per page. Must be greater than 0.</param>
        /// <returns>An <see cref="IActionResult"/> containing the filtered and paginated list of log entries.</returns>
        [HttpGet("query")]
        public IActionResult Query(
            string? level = null,
            string? service = null,
            string? search = null,
            int page = 1,
            int pageSize = 50)
        {
            var results = _logs.QueryLogs(level, service, search, page, pageSize);
            return Ok(results);
        }

        /// <summary>
        /// Returns the most recent log file from the "logs" directory as a downloadable plain text file.
        /// </summary>
        /// <remarks>The file is served with a MIME type of "text/plain" and the original file name. If
        /// the "logs" directory does not exist or contains no .log files, an exception may be thrown.</remarks>
        /// <returns>A file result containing the contents of the latest .log file in the "logs" directory, or an error response
        /// if no log files are found.</returns>
        [HttpGet("download")]
        public IActionResult Download()
        {
            var file = Directory.GetFiles("Logs", "*.log")
                .OrderByDescending(f => f)
                .First();

            var bytes = System.IO.File.ReadAllBytes(file);
            return File(bytes, "text/plain", Path.GetFileName(file));
        }
    }
}
