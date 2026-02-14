using OpenQA.Selenium.Internal;
using System.Linq.Expressions;
using System.Text.Json;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Provides methods for querying and retrieving log entries from log files stored in the application's log
    /// directory.
    /// </summary>
    /// <remarks>This service enables filtering and paging of log entries based on log level, service name,
    /// and search terms. It also supports retrieving the most recent log entries. All log data is expected to be in
    /// JSON format within the log files. This class is not thread-safe; concurrent access should be managed externally
    /// if required.</remarks>
    public class LogQueryService
    {
        private readonly string _logDirectory = "Logs";

        /// <summary>
        /// Queries log entries from the log directory, applying optional filters for log level, service name, and
        /// search text, and returns a paged collection of matching entries as JSON elements.
        /// </summary>
        /// <remarks>Log entries are read from all files in the log directory with a ".log" extension,
        /// ordered by file name descending. Invalid JSON lines are skipped. Paging is applied after filtering. This
        /// method is not thread-safe and may be affected by concurrent modifications to log files.</remarks>
        /// <param name="level">The log level to filter by. If not null, only log entries with a matching level (case-insensitive) are
        /// included.</param>
        /// <param name="service">The service name to filter by. If not null, only log entries with a matching service name (case-insensitive)
        /// are included.</param>
        /// <param name="search">An optional search string to filter log entries by their raw text content. If not null, only entries
        /// containing this text (case-insensitive) are included.</param>
        /// <param name="page">The page number of results to return. Must be greater than or equal to 1.</param>
        /// <param name="pageSize">The number of log entries to include in each page. Must be greater than 0.</param>
        /// <returns>An enumerable collection of JSON elements representing log entries that match the specified filters and
        /// paging parameters. The collection may be empty if no entries match.</returns>
        public IEnumerable<JsonElement> QueryLogs(
            string? level,
            string? service,
            string? search,
            int page,
            int pageSize)
        {
            var files = Directory.GetFiles(_logDirectory, "*.log")
                .OrderByDescending(f => f);

            var results = new List<JsonElement>();

            foreach (var file in files)
            {
                foreach (var line in File.ReadLines(file))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    JsonElement json;
                    try
                    {
                        json = JsonSerializer.Deserialize<JsonElement>(line);
                    }
                    catch
                    {
                        continue; // Skip lines that are not valid JSON
                    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (level != null &&
                        json.TryGetProperty("@l", out var lvl) &&
                        !lvl.GetString().Equals(level, StringComparison.OrdinalIgnoreCase))
                        continue;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (service != null &&
                        json.TryGetProperty("Service", out var svc) &&
                        !svc.GetString().Equals(service, StringComparison.OrdinalIgnoreCase))
                        continue;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    if (search != null &&
                        !line.Contains(search, StringComparison.OrdinalIgnoreCase))
                        continue;

                    results.Add(json);
                }
            }

            return results
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Returns the most recent log entries from the latest log file as a sequence of JSON elements.
        /// </summary>
        /// <remarks>Entries are read from the latest log file in reverse order, starting from the most
        /// recent. Invalid or empty lines are skipped. This method does not guarantee that all returned elements are
        /// valid or complete log records; callers should validate the contents as needed.</remarks>
        /// <param name="count">The maximum number of log entries to return. Must be greater than zero.</param>
        /// <returns>An enumerable collection of <see cref="JsonElement"/> objects representing the most recent log entries. The
        /// collection may contain fewer than <paramref name="count"/> elements if the log file has fewer entries.</returns>
        public IEnumerable<JsonElement> Tail(int count)
        {
            var file = Directory.GetFiles(_logDirectory, "*.log")
                .OrderByDescending(f => f)
                .First();

            var lines = File.ReadLines(file).Reverse().Take(count);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                JsonElement json;
                try
                {
                    json = JsonSerializer.Deserialize<JsonElement>(line);
                }
                catch
                {
                    continue;
                }
                yield return json;
            }
        }
    }
}
