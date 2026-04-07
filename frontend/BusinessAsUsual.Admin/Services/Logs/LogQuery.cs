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

    // Local pagination
    /// <summary>
    /// Gets or sets the current page number for paginated results.
    /// </summary>
    public int Page { get; set; } = 1;

    // CloudWatch pagination
    /// <summary>
    /// Gets or sets the pagination token used to retrieve the next set of results from a paginated CloudWatch
    /// operation.
    /// </summary>
    /// <remarks>If the response from a CloudWatch API call is truncated, this property contains a token that
    /// can be used in a subsequent request to continue retrieving results. If there are no more results, this property
    /// is null.</remarks>
    public string? NextToken { get; set; }

    /// <summary>
    /// Gets or sets the token that was used in the previous operation, if any.
    /// </summary>
    public string? PrevToken { get; set; }

    /// <summary>
    /// Gets or sets the start date for the associated event or operation.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date for the associated period or event.
    /// </summary>
    public DateTime? EndDate { get; set; }
}