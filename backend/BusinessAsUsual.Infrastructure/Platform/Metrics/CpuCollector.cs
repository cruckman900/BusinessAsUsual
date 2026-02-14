using System.Runtime.InteropServices;

namespace BusinessAsUsual.Infrastructure.Platform.Metrics
{
    /// <summary>
    /// Provides functionality to asynchronously collect CPU usage statistics from the system.
    /// </summary>
    /// <remarks>The CpuCollector class is designed for environments where CPU usage can be read from the
    /// /proc/stat file, such as Linux-based systems. This class is not supported on platforms that do not expose CPU
    /// statistics via /proc/stat.</remarks>
    public class CpuCollector
    {
        /// <summary>
        /// Asynchronously calculates the current CPU usage percentage of the system.
        /// </summary>
        /// <remarks>This method reads CPU statistics from the "/proc/stat" file and measures usage over a
        /// short interval. It is intended for use on Linux systems where "/proc/stat" is available. The method
        /// introduces a brief delay to sample CPU activity over time.</remarks>
        /// <returns>A double value representing the CPU usage as a percentage, from 0 to 100. Returns 0 if the usage cannot be
        /// determined.</returns>
        public async Task<double> GetCpuUsageAsync()
        {
            // If not Linux, return empty values
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return 0;
            }

            var cpuLine1 = File.ReadLines("/proc/stat")
                .First(l => l.StartsWith("cpu "));

            await Task.Delay(200);

            var cpuLine2 = File.ReadLines("/proc/stat")
                .First(l => l.StartsWith("cpu "));

            var parts1 = cpuLine1.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(long.Parse)
                .ToArray();

            var parts2 = cpuLine2.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(long.Parse)
                .ToArray();

            long idle1 = parts1[3];
            long idle2 = parts2[3];

            long total1 = parts1.Sum();
            long total2 = parts2.Sum();

            double idleDelta = idle2 - idle1;
            double totalDelta = total2 - total1;

            if (totalDelta == 0)
                return 0;

            return 100.0 * (1.0 - idleDelta / totalDelta);
        }
    }
}
