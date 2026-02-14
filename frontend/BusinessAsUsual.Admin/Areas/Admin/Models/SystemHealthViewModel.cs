namespace BusinessAsUsual.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Represents a view model containing system health metrics, including status, resource usage, and uptime
    /// information.
    /// </summary>
    /// <remarks>This class is typically used to transfer system health data between the backend and user
    /// interface in monitoring or dashboard applications. All properties are read-write, allowing for both retrieval
    /// and assignment of system health values.</remarks>
    public class SystemHealthViewModel
    {
        /// <summary>
        /// Gets or sets the current status message associated with the object.
        /// </summary>
        public string Status { get; set; } = "";

        /// <summary>
        /// Gets or sets the percentage of CPU usage for the current process.
        /// </summary>
        public double Cpu { get; set; }

        /// <summary>
        /// Gets or sets the amount of memory currently used, in megabytes.
        /// </summary>
        public double MemoryUsed { get; set; }

        /// <summary>
        /// Gets or sets the total amount of memory available, in bytes.
        /// </summary>
        public double MemoryTotal { get; set; }

        /// <summary>
        /// Gets or sets the amount of disk space used, in bytes.
        /// </summary>
        public double DiskUsed { get; set; }

        /// <summary>
        /// Gets or sets the total disk space available, measured in gigabytes.
        /// </summary>
        public double DiskTotal { get; set; }

        /// <summary>
        /// Gets or sets the formatted uptime of the system as a human-readable string.
        /// </summary>
        public string Uptime { get; set; } = "";
    }
}
