using BusinessAsUsual.Domain.Platform.Metrics;

namespace BusinessAsUsual.Domain.Platform.Health
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
}
