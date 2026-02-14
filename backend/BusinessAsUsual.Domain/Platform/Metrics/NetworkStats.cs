namespace BusinessAsUsual.Domain.Platform.Metrics
{
    /// <summary>
    /// Represents network usage statistics, including the total number of bytes received and sent.
    /// </summary>
    public class NetworkStats
    {
        /// <summary>
        /// Gets or sets the total number of bytes received.
        /// </summary>
        public long BytesIn { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes sent.
        /// </summary>
        public long BytesOut { get; set; }
    }
}
