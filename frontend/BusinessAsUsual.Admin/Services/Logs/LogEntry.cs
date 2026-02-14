namespace BusinessAsUsual.Admin.Services.Logs;

/// <summary>
/// Represents a single log entry containing information about an event, error, or message recorded by the application.
/// </summary>
/// <remarks>A log entry typically includes a timestamp, severity level, message text, and optional details such
/// as exception information or the source of the log. This class can be used to capture and store log data for
/// diagnostics, auditing, or monitoring purposes.</remarks>
public class LogEntry
{
    /// <summary>
    /// Gets or sets the date and time associated with the current event or record.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the level associated with the current instance.
    /// </summary>
    public string Level { get; set; } = "";

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Gets or sets the exception message associated with the current operation, if any.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Gets or sets the source identifier associated with the object.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets the timestamp as a formatted string in the "yyyy-MM-dd HH:mm:ss.fff" pattern.
    /// </summary>
    public string TimestampFormatted => Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
}