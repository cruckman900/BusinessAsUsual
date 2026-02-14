using System.Text.Json;
using System.Text.RegularExpressions;

namespace BusinessAsUsual.Admin.Services.Logs;

/// <summary>
/// Provides functionality to read log entries from local log files stored in the application's log directory.
/// </summary>
/// <remarks>LocalLogReader retrieves log data from files located in the 'logs' folder under the application's
/// content root path. This class is intended for use in environments where logs are stored locally and accessible via
/// the file system. Ensure that the application has appropriate permissions to read from the log directory.
/// LocalLogReader implements the ILogReader interface, allowing it to be used interchangeably with other log reader
/// implementations.</remarks>
public class LocalLogReader : ILogReader
{
    private readonly string _logDirectory;

    /// <summary>
    /// Initializes a new instance of the LocalLogReader class using the specified web host environment.
    /// </summary>
    /// <remarks>The log directory is set to the 'logs' folder located under the application's content root
    /// path. Ensure that the environment provides a valid content root path and that the application has permission to
    /// access the log directory.</remarks>
    /// <param name="env">The web host environment that provides access to the application's content root path.</param>
    public LocalLogReader(IWebHostEnvironment env)
    {
        _logDirectory = Path.Combine(env.ContentRootPath, "Logs");
    }

    /// <summary>
    /// Asynchronously retrieves log entries that match the specified query criteria.
    /// </summary>
    /// <remarks>Only the most recent log files are searched, and results are returned in reverse
    /// chronological order. The method stops searching once the specified limit is reached.</remarks>
    /// <param name="query">An object containing filter parameters for the log search, such as log level, search text, and result limit.
    /// Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of log entries matching
    /// the query. The collection may be empty if no entries are found.</returns>
    public async Task<IEnumerable<LogEntry>> GetLogsAsync(LogQuery query)
    {
        Console.WriteLine("LocalLogReader: Reading logs from " + _logDirectory);
        var files = Directory.GetFiles(_logDirectory, "*.log")
                             .OrderByDescending(f => f)
                             .Take(3); // read last 3 files

        var entries = new List<LogEntry>();
        LogEntry? entry = null;

        foreach (var file in files)
        {
            var lines = await ReadAllLinesSharedAsync(file);

            foreach (var line in lines)
            {
                LogEntry? newEntry = null;

                if (IsJsonLine(line))
                {
                    newEntry = ParseJson(line);
                }
                else
                {
                    var match = Regex.Match(line,
                        @"^(?<ts>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) (?<offset>[-+]\d{2}:\d{2}) \[(?<lvl>[A-Z]+)\] (?<msg>.*)$");

                    if (match.Success)
                    {
                        newEntry = new LogEntry
                        {
                            Timestamp = DateTime.Parse(match.Groups["ts"].Value),
                            Level = NormalizeLevel(match.Groups["lvl"].Value),
                            Message = match.Groups["msg"].Value,
                            Exception = ""
                        };
                    }
                }

                if (newEntry != null)
                {
                    // Apply filters BEFORE adding
                    if (query.Level != null && newEntry.Level != query.Level)
                        continue;

                    if (query.Search != null &&
                        !newEntry.Message.Contains(query.Search, StringComparison.OrdinalIgnoreCase) &&
                        (newEntry.Exception == null || !newEntry.Exception.Contains(query.Search, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    entries.Add(newEntry);
                    entry = newEntry; // set current entry
                    continue;
                }

                // Continuation line (stack trace)
                if (entry != null && !IsJsonLine(line))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        entry.Exception += line.TrimEnd() + "\n";
                    }
                }

                if (entries.Count >= query.Limit)
                {
                    var skip = (query.Page - 1) * query.Limit;
                    return entries.Skip(skip).Take(query.Limit);
                }
            }

            entries.Reverse();
        }

        return entries;
    }

    private static bool IsJsonLine(string line)
    {
        return line.StartsWith("{") && line.EndsWith("}");
    }

    private LogEntry? ParseJson(string line)
    {
        try
        {
            var raw = JsonSerializer.Deserialize<SerilogJson>(line);
            if (raw == null) return null;

            return new LogEntry
            {
                Timestamp = raw.@t,
                Level = NormalizeLevel(raw.@l ?? "INFO"),
                Message = raw.@m,
                Exception = raw.@x
            };
        }
        catch
        {
            return null;
        }
    }

    private async Task<string[]> ReadAllLinesSharedAsync(string path)
    {
        var lines = new List<string>();

        using var fs = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite); // ← key change

        using var sr = new StreamReader(fs);

        string? line;
        while ((line = await sr.ReadLineAsync()) != null)
        {
            lines.Add(line);
        }

        return lines.ToArray();
    }

    private static string NormalizeLevel(string lvl)
    {
        return lvl switch
        {
            "ERR" => "ERROR",
            "INF" => "INFO",
            "WRN" => "WARN",
            "DBG" => "DEBUG",
            "FTL" => "FATAL",
            _ => lvl
        };
    }
}