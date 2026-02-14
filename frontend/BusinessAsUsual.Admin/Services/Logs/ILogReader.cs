namespace BusinessAsUsual.Admin.Services.Logs
{
    /// <summary>
    /// Defines a contract for asynchronously retrieving log entries that match specified query criteria.
    /// </summary>
    public interface ILogReader
    {
        /// <summary>
        /// Asynchronously retrieves log entries that match the specified query criteria.
        /// </summary>
        /// <param name="query">The query parameters used to filter and select log entries. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
        /// cref="LogEntry"/> objects that match the query. The collection is empty if no log entries are found.</returns>
        Task<IEnumerable<LogEntry>> GetLogsAsync(LogQuery query);
    }
}
