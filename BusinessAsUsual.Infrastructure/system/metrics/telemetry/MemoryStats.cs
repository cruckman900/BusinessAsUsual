namespace BusinessAsUsual.Infrastructure.System.Metrics.Telemetry
{
    /// <summary>
    /// Represents memory usage statistics, including total and used memory values.
    /// </summary>
    /// <remarks>Use this class to track or report memory consumption metrics, such as in monitoring or diagnostic
    /// scenarios. All values are expressed in the same units, typically bytes or megabytes, depending on the context in
    /// which the class is used.</remarks>
    public class MemoryStats
    {
        /// <summary>
        /// Gets or sets the total value represented by this instance.
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// Gets or sets the amount of resource that has been used.
        /// </summary>
        public double Used { get; set; }
    }
}