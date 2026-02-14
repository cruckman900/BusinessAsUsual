namespace BusinessAsUsual.Admin.Services.Logs;

/// <summary>
/// Represents the parameters used to filter and limit log query results.
/// </summary>
/// <remarks>Use this class to specify criteria such as log level, search keywords, and the maximum number of
/// results when querying logs. All properties are optional; if a property is not set, the corresponding filter is not
/// applied.</remarks>
public class LogQuery
{
    /// <summary>
    /// Gets or sets the log level for the current entry.
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// Gets or sets the search query used to filter results.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items to return in a single operation.
    /// </summary>
    public int Limit { get; set; } = 200;

    /// <summary>
    /// Gets or sets the current page number for paginated results.
    /// </summary>
    public int Page { get; set; } = 1;
}