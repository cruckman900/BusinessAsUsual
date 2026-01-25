using BusinessAsUsual.Infrastructure.System.Metrics.Telemetry;
using Sprache;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BusinessAsUsual.Admin.Services.Metrics
{
    /// <summary>
    /// Provides functionality to retrieve memory usage statistics from the current system.
    /// </summary>
    /// <remarks>This class is intended for use on systems that expose memory information via the
    /// "/proc/meminfo" file, such as Linux-based operating systems. It is not supported on platforms that do not
    /// provide this file.</remarks>
    public class MemoryCollector
    {
        /// <summary>
        /// Asynchronously retrieves the current system memory usage statistics.
        /// </summary>
        /// <remarks>This method reads memory information from the "/proc/meminfo" file, which is
        /// available on Linux-based systems. It may not be supported on other operating systems.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="MemoryStats"/>
        /// object with the total and used memory, measured in gigabytes.</returns>
        public Task<MemoryStats> GetMemoryAsync()
        {
            // If not Linux, return empty values
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Task.FromResult(new MemoryStats
                {
                    Total = 0,
                    Used = 0
                });
            }

            var lines = File.ReadAllLines("/proc/meminfo");

            long total = Parse(lines, "MemTotal");
            long available = Parse(lines, "MemAvailable");

            long used = total - available;

            return Task.FromResult(new MemoryStats
            {
                Total = Math.Round(total / 1024.0 / 1024.0, 2), // GB
                Used = Math.Round(used / 1024.0 / 1024.0, 2)    // GB
            });
        }

        private long Parse(string[] lines, string key)
        {
            return long.Parse(
                lines.First(l => l.StartsWith(key))
                     .Split(':')[1]
                     .Trim()
                     .Split(' ')[0]
            ) * 1024; // Convert from KB to Bytes
        }
    }
}
