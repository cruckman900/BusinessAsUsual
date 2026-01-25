namespace BusinessAsUsual.Infrastructure.System.Metrics.Telemetry
{
    /// <summary>
    /// Represents a collection of system resource metrics, including CPU, memory, disk, network, and uptime statistics.
    /// </summary>
    /// <remarks>Use this class to access current system performance data in a structured format. Each
    /// property provides detailed statistics for a specific aspect of system health. The availability and accuracy of
    /// individual metrics may depend on the underlying platform or environment.</remarks>
    public class SystemMetrics
    {
        /// <summary>
        /// Gets or sets the CPU usage statistics for the system.
        /// </summary>
        public CpuStats? Cpu { get; set; }

        /// <summary>
        /// Gets or sets the memory usage statistics for the current instance.
        /// </summary>
        public MemoryStats? Memory { get; set; }

        /// <summary>
        /// Gets or sets the disk usage statistics for the system.
        /// </summary>
        public DiskStats? Disk { get; set; }

        /// <summary>
        /// Gets or sets the current network statistics for the device.
        /// </summary>
        public NetworkStats? Network { get; set; }

        /// <summary>
        /// Gets or sets the uptime statistics for the current instance.
        /// </summary>
        public UptimeStats? Uptime { get; set; }
    }
}
