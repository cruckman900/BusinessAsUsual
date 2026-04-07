namespace BusinessAsUsual.Admin.Services.Logs
{
    /// <summary>
    /// Represents the result of a paged log retrieval operation, including log entries and pagination information for
    /// both local and CloudWatch sources.
    /// </summary>
    /// <remarks>Use this class to access a page of log entries along with the necessary pagination tokens or
    /// page numbers to retrieve additional pages. The pagination mechanism depends on the log source: local pagination
    /// uses the Page property, while CloudWatch pagination uses NextToken and PreviousToken.</remarks>
    public class LogPageResult
    {
        /// <summary>
        /// Gets or sets the logging mode used by the application.
        /// </summary>
        /// <remarks>Valid values are "local" and "cloudwatch". The mode determines whether logs are
        /// written to local storage or sent to AWS CloudWatch. The default value is "local".</remarks>
        public string Mode { get; set; } = "local"; // "local" or "cloudwatch"

        // Local pagination
        /// <summary>
        /// Gets or sets the current page number for pagination.
        /// </summary>
        /// <remarks>The first page is typically 1. Changing this value affects which subset of items is
        /// retrieved or displayed when paging through a collection.</remarks>
        public int Page { get; set; } = 1;

        // CloudWatch pagination
        /// <summary>
        /// Gets or sets the token for retrieving the next page of results from CloudWatch.
        /// </summary>
        /// <remarks>This token is provided by CloudWatch when there are additional pages of results available.
        /// It should be used in subsequent requests to fetch the next page.</remarks>
        public string? NextToken { get; set; }

        /// <summary>
        /// Gets or sets the token that was used to retrieve the previous page of results in a paginated operation.
        /// </summary>
        public string? PrevToken { get; set; }

        /// <summary>
        /// Gets or sets the collection of log entries associated with the current instance.
        /// </summary>
        public List<LogEntry> Logs { get; set; } = new();
    }
}
