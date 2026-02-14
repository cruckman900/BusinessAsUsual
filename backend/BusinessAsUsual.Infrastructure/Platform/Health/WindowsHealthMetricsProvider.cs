using BusinessAsUsual.Domain.Platform.Health;
using BusinessAsUsual.Domain.Platform.Metrics;
using System.Diagnostics;

namespace BusinessAsUsual.Infrastructure.Platform.Health
{
    /// <summary>
    /// Provides health metrics for a Windows system, including CPU, memory, disk usage, and system uptime.
    /// </summary>
    /// <remarks>This provider is intended for use on Windows platforms and relies on Windows-specific
    /// performance counters and APIs. It implements the IHealthMetricsProvider interface to supply system health
    /// statistics that can be used for monitoring or diagnostics. This class is not thread safe.</remarks>
    public class WindowsHealthMetricsProvider : IHealthMetricsProvider
    {
        /// <summary>
        /// Retrieves the current system health statistics, including CPU usage, memory usage, disk usage, and system
        /// uptime.
        /// </summary>
        /// <returns>A <see cref="HealthStats"/> object containing the latest CPU, memory, disk, and uptime information for the
        /// system.</returns>
        public HealthStats GetStats()
        {
            var cpu = GetCpuUsage();
            var memory = GetMemory();
            var disk = GetDisk();
            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

            return new HealthStats
            {
                Cpu = cpu,
                Memory = memory,
                Disk = disk,
                Uptime = uptime.ToString(@"dd\.hh\:mm\:ss")
            };
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private int GetCpuUsage()
        {
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(200);
            return (int)cpuCounter.NextValue();
        }

        private MemoryStats GetMemory()
        {
            var total = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024d / 1024d;
            var used = GC.GetTotalMemory(false) / 1024d / 1024d;
            return new MemoryStats { Used = used, Total = total };
        }

        private DiskStats GetDisk()
        {
            var drive = DriveInfo.GetDrives().First(d => d.IsReady);
            var total = drive.TotalSize / 1024d / 1024d / 1024d;
            var used = (drive.TotalSize - drive.TotalFreeSpace) / 1024d / 1024d / 1024d;
            return new DiskStats { Used = used, Total = total };
        }
    }
}
