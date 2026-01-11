namespace BusinessAsUsual.Admin.Services.Health
{
    /// <summary>
    /// Provides health metrics for a Linux system, including CPU, memory, disk usage, and uptime information.
    /// </summary>
    /// <remarks>This provider retrieves system health statistics by reading from Linux-specific files such as
    /// /proc/stat, /proc/meminfo, and /proc/uptime. It is intended for use on Linux-based operating systems and may not
    /// function correctly on other platforms.</remarks>
    public class LinuxHealthMetricsProvider : IHealthMetricsProvider
    {
        /// <summary>
        /// Retrieves the current system health statistics, including CPU usage, memory usage, disk usage, and uptime.
        /// </summary>
        /// <returns>A <see cref="HealthStats"/> object containing the latest values for CPU, memory, disk usage, and system
        /// uptime.</returns>
        public HealthStats GetStats()
        {
            return new HealthStats
            {
                Cpu = GetCpuUsage(),
                Memory = GetMemory(),
                Disk = GetDisk(),
                Uptime = GetUptime()
            };
        }

        private int GetCpuUsage()
        {
            var cpuLine1 = File.ReadLines("/proc/stat").First();
            Thread.Sleep(200);
            var cpuLine2 = File.ReadLines("/proc/stat").First();

            var v1 = cpuLine1.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToArray();
            var v2 = cpuLine2.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToArray();

            var idle1 = v1[3];
            var idle2 = v2[3];

            var total1 = v1.Sum();
            var total2 = v2.Sum();

            var idleDelta = idle2 - idle1;
            var totalDelta = total2 - total1;

            return (int)(100 * (1.0 - (idleDelta / (double)totalDelta)));
        }

        private MemoryStats GetMemory()
        {
            var lines = File.ReadAllLines("/proc/meminfo");
            var total = double.Parse(lines[0].Split(':')[1].Trim().Split(' ')[0]) / 1024;
            var free = double.Parse(lines[1].Split(':')[1].Trim().Split(' ')[0]) / 1024;
            return new MemoryStats { Used = total - free, Total = total };
        }

        private DiskStats GetDisk()
        {
            var drive = DriveInfo.GetDrives()
                .First(d => d.Name == "/");

            var total = drive.TotalSize / 1024d / 1024d / 1024d;
            var used = (drive.TotalSize - drive.TotalFreeSpace) / 1024d / 1024d / 1024d;

            return new DiskStats { Used = used, Total = total };
        }

        private string GetUptime()
        {
            var uptimeSeconds = double.Parse(File.ReadAllText("/proc/uptime").Split(' ')[0]);
            return TimeSpan.FromSeconds(uptimeSeconds).ToString(@"dd\.hh\:mm\:ss");
        }
    }
}
