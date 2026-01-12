namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Represents uptime statistics, including the total duration in seconds and a human-readable representation.
    /// </summary>
    public class UptimeStats
    {
        /// <summary>
        /// Gets or sets the time interval, in seconds.
        /// </summary>
        public double Seconds { get; set; }

        /// <summary>
        /// Gets or sets the human-readable representation of the value.
        /// </summary>
        public string? HumanReadable { get; set; }
    }
}
