namespace BusinessAsUsual.Admin.Services.Health
{
    /// <summary>
    /// Represents a snapshot of system health statistics, including CPU, memory, disk usage, and uptime information.
    /// </summary>
    /// <remarks>Use this class to aggregate and access key health metrics for a system at a specific point in
    /// time. The contained properties provide detailed statistics for each resource area. This class is typically used
    /// in monitoring or diagnostic scenarios to assess system status.</remarks>
    public class HealthStats
    {
        /// <summary>
        /// Gets or sets the number of CPU cores allocated or used.
        /// </summary>
        public int Cpu { get; set; }

        /// <summary>
        /// Gets or sets the memory usage statistics for the current instance.
        /// </summary>
        public MemoryStats Memory { get; set; } = new();

        /// <summary>
        /// Gets or sets the disk usage statistics for the system.
        /// </summary>
        public DiskStats Disk { get; set; } = new();

        /// <summary>
        /// Gets or sets the formatted uptime of the application as a human-readable string.
        /// </summary>
        public string Uptime { get; set; } = "";
    }

    /// <summary>
    /// Represents memory usage statistics, including the amount of memory used and the total available memory.
    /// </summary>
    public class MemoryStats
    {
        /// <summary>
        /// Gets or sets the amount of resource that has been used.
        /// </summary>
        public double Used { get; set; }

        /// <summary>
        /// Gets or sets the total value represented by this instance.
        /// </summary>
        public double Total { get; set; }
    }

    /// <summary>
    /// Represents disk usage statistics, including used and total disk space.
    /// </summary>
    public class DiskStats
    {
        /// <summary>
        /// Gets or sets the amount of resource that has been used.
        /// </summary>
        public double Used { get; set; }

        /// <summary>
        /// Gets or sets the total amount represented by this instance.
        /// </summary>
        public double Total { get; set; }
    }
}
