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
        /// Initializes a new instance of the LogsController class with the specified log reader.
        /// </summary>
        /// <param name="logReader">The log reader used to retrieve log entries for this controller. Cannot be null.</param>
        public LogsController(ILogReader logReader)
        {
            _logReader = logReader;
        }

        /// <summary>
        /// Handles HTTP GET requests for the root URL and returns the default view for the controller.
        /// </summary>
        /// <returns>An <see cref="ViewResult"/> that renders the default view associated with the action.</returns>
        [HttpGet("")]
        public IActionResult Index() => View();

        /// <summary>
        /// Retrieves a paginated list of log entries filtered by log level and search criteria.
        /// </summary>
        /// <remarks>The response includes pagination tokens for navigating through large result sets. If
        /// both nextToken and prevToken are provided, only one is used depending on the pagination direction. The logs
        /// are filtered and paginated based on the specified parameters.</remarks>
        /// <param name="level">The log level to filter results by. Specify a value such as "Error", "Warning", or "Information" to limit
        /// results to that level. If null, logs of all levels are included.</param>
        /// <param name="search">A search string to filter log messages. Only logs containing this string in their message or exception
        /// details are returned. If null, no search filtering is applied.</param>
        /// <param name="startDate">The start date for filtering log entries. Only logs with a timestamp on or after this date are included. If null, no start date filtering is applied.</param>
        /// <param name="endDate">The end date for filtering log entries. Only logs with a timestamp on or before this date are included. If null, no end date filtering is applied.</param>
        /// <param name="limit">The maximum number of log entries to return in a single page. Must be a positive integer. The default is
        /// 200.</param>
        /// <param name="page">The page number of results to retrieve. Must be greater than or equal to 1. The default is 1.</param>
        /// <param name="nextToken">A token indicating the position to continue retrieving results for forward pagination. Use the value
        /// returned from a previous response to fetch the next page. If null, pagination starts from the beginning.</param>
        /// <param name="prevToken">A token indicating the position to continue retrieving results for backward pagination. Use the value
        /// returned from a previous response to fetch the previous page. If null, backward pagination is not performed.</param>
        /// <returns>An IActionResult containing a JSON object with the current page of log entries, pagination tokens, and
        /// associated metadata.</returns>
        [HttpGet("data")]
        public async Task<IActionResult> GetLogs(
            string? level,
            string? search,
            DateTime? startDate,
            DateTime? endDate,
            int limit = 200,
            int page = 1,
            string? nextToken = null,
            string? prevToken = null)
        {
            var query = new LogQuery
            {
                Level = level,
                Search = search,
                StartDate = startDate,
                EndDate = endDate,
                Limit = limit,
                Page = page,
                NextToken = nextToken,
                PrevToken = prevToken
            };

            var result = await _logReader.GetLogsAsync(query);

            return Json(new
            {
                mode = result.Mode,
                page = result.Page,
                nextToken = result.NextToken,
                prevToken = result.PrevToken,
                logs = result.Logs.Select(e => new
                {
                    timestamp = e.TimestampFormatted,
                    level = e.Level,
                    message = e.Message,
                    exception = e.Exception
                })
            });
        }

        /// <summary>
        /// Generates and returns a downloadable text file containing log entries filtered by the specified criteria.
        /// </summary>
        /// <remarks>The returned file contains all matching log entries, with each entry formatted to
        /// include its timestamp, level, message, and exception details if present.</remarks>
        /// <param name="level">The log level to filter by. If null, logs of all levels are included.</param>
        /// <param name="search">A search string to filter log messages. Only logs containing this string are included. If null, no search
        /// filtering is applied.</param>
        /// <param name="startDate">The earliest date and time for log entries to include. If null, no lower date bound is applied.</param>
        /// <param name="endDate">The latest date and time for log entries to include. If null, no upper date bound is applied.</param>
        /// <returns>A file result containing the filtered logs as a UTF-8 encoded text file named 'logs.txt'.</returns>
        [HttpGet("download")]
        public async Task<IActionResult> DownloadLogs(
            string? level,
            string? search,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = new LogQuery
            {
                Level = level,
                Search = search,
                StartDate = startDate,
                EndDate = endDate,
                Limit = int.MaxValue // get everything
            };

            var result = await _logReader.GetLogsAsync(query);

            var text = string.Join("\n", result.Logs.Select(l =>
                $"{l.TimestampFormatted} [{l.Level}] {l.Message}\n{l.Exception}"
            ));

            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return File(bytes, "text/plain", "logs.txt");
        }
    }
}
