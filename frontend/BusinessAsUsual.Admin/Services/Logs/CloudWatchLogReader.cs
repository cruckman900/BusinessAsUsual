using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using System.Text.RegularExpressions;

namespace BusinessAsUsual.Admin.Services.Logs;

/// <summary>
/// Provides functionality to retrieve log entries from AWS CloudWatch Logs based on specified queries.
/// </summary>
/// <remarks>This class implements the ILogReader interface to enable reading logs from AWS CloudWatch. Use this
/// class when you need to access or analyze log data stored in CloudWatch Logs within your application. Thread safety
/// and performance characteristics depend on the underlying AWS SDK implementation.</remarks>
public class CloudWatchLogReader : ILogReader
{
    private readonly IAmazonCloudWatchLogs _client;

    /// <summary>
    /// Initializes a new instance of the CloudWatchLogReader class using the specified CloudWatch Logs client.
    /// </summary>
    /// <remarks>Use this constructor to provide a custom or preconfigured CloudWatch Logs client, such as one
    /// with specific credentials or region settings.</remarks>
    /// <param name="client">The IAmazonCloudWatchLogs client used to interact with AWS CloudWatch Logs. Cannot be null.</param>
    public CloudWatchLogReader(IAmazonCloudWatchLogs client)
    {
        _client = client;
    }

    /// <summary>
    /// Asynchronously retrieves a filtered page of log entries from the specified log group based on the provided query
    /// parameters.
    /// </summary>
    /// <remarks>Filtering by log level and search term is performed client-side after retrieving log events
    /// from the log group. The returned page number is always 1, as server-side paging is not supported; use the
    /// provided tokens for pagination. This method is not thread-safe.</remarks>
    /// <param name="query">The query parameters used to filter, search, and page through log entries. Must not be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a page of log entries and pagination
    /// tokens for retrieving additional results.</returns>
    public async Task<LogPageResult> GetLogsAsync(LogQuery query)
    {
        var request = new FilterLogEventsRequest
        {
            LogGroupName = "/bau/admin",
            Limit = query.Limit,
            NextToken = query.NextToken
            // We’ll filter level/search client-side for simplicity
        };

        if (query.StartDate.HasValue)
            request.StartTime = new DateTimeOffset(query.StartDate.Value).ToUnixTimeMilliseconds();

        if (query.EndDate.HasValue)
            request.EndTime = new DateTimeOffset(query.EndDate.Value).ToUnixTimeMilliseconds();

        var response = await _client.FilterLogEventsAsync(request);

        var entries = response.Events
            .Select(e => new LogEntry
            {
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)e.Timestamp!).DateTime,
                Level = ExtractLevel(e.Message),
                Message = e.Message
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(query.Level))
        {
            entries = entries
                .Where(e => string.Equals(e.Level, query.Level, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            entries = entries
                .Where(e =>
                    e.Message.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    (e.Exception != null && e.Exception.Contains(query.Search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return new LogPageResult
        {
            Mode = "cloudwatch",
            Page = 1, // page is meaningless here
            Logs = entries,
            NextToken = response.NextToken,
            PrevToken = query.NextToken // minimal backward hint; real back requires client history
        };
    }

    private static string ExtractLevel(string message)
    {
        var match = Regex.Match(message, @"\[(?<lvl>[A-Z]+)\]");
        if (!match.Success) return "INFO";

        return match.Groups["lvl"].Value switch
        {
            "ERR" => "ERROR",
            "INF" => "INFO",
            "WRN" => "WARN",
            "DBG" => "DEBUG",
            "FAT" => "FATAL",
            "FTL" => "FATAL",
            _ => match.Groups["lvl"].Value
        };
    }
}