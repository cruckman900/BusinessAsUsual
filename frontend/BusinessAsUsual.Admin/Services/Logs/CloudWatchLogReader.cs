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
    /// Initializes a new instance of the CloudWatchLogReader class using the specified Amazon CloudWatch Logs client.
    /// </summary>
    /// <param name="client">The IAmazonCloudWatchLogs client used to interact with the Amazon CloudWatch Logs service. Cannot be null.</param>
    public CloudWatchLogReader(IAmazonCloudWatchLogs client)
    {
        _client = client;
    }

    /// <summary>
    /// Asynchronously retrieves log entries that match the specified query criteria.
    /// </summary>
    /// <param name="query">The query parameters used to filter and select log entries. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of log entries that
    /// match the query. The collection is empty if no entries are found.</returns>
    public async Task<IEnumerable<LogEntry>> GetLogsAsync(LogQuery query)
    {
        var request = new FilterLogEventsRequest
        {
            LogGroupName = "/bau/admin",
            Limit = query.Limit,
            FilterPattern = query.Level != null ? $"[{query.Level}]" : null
        };

        var response = await _client.FilterLogEventsAsync(new FilterLogEventsRequest
        {
            LogGroupName = "/bau/admin",
            Limit = query.Limit,
            FilterPattern = query.Level != null ? $"[{query.Level}]" : null
        });

        return response.Events.Select(e => new LogEntry
        {
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)e.Timestamp!).DateTime,
            Level = ExtractLevel(e.Message),
            Message = e.Message
        });
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