using BusinessAsUsual.Admin.Areas.Admin.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BusinessAsUsual.Admin.Services.Metrics
{
    /// <summary>
    /// Provides functionality to retrieve system uptime statistics from the current environment.
    /// </summary>
    /// <remarks>This class is intended for environments where the system uptime can be read from the
    /// /proc/uptime file, such as Linux-based systems. It exposes methods to obtain uptime information in both raw and
    /// human-readable formats. Instances of this class are not thread-safe.</remarks>
    public class UptimeCollector
    {
        /// <summary>
        /// Asynchronously retrieves the system uptime statistics, including the total uptime in seconds and a
        /// human-readable representation.
        /// </summary>
        /// <remarks>This method reads uptime data from the /proc/uptime file, which is available on Linux
        /// systems. If called on a non-Linux platform or if the file is inaccessible, the method may throw an
        /// exception.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="UptimeStats"/>
        /// object with the system's uptime information.</returns>
        public Task<UptimeStats> GetUptimeAsync()
        {
            // If not Linux, return zeros
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Task.FromResult(new UptimeStats
                {
                    Seconds = 0,
                    HumanReadable = null
                });
            }

            // /proc/uptime returns: "<seconds> <idle_seconds>"
            var parts = File.ReadAllText("/proc/uptime")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            double uptimeSeconds = double.Parse(parts[0]);

            return Task.FromResult(new UptimeStats
            {
                Seconds = uptimeSeconds,
                HumanReadable = Format(uptimeSeconds)
            });
        }

        private string Format(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{(int)ts.TotalDays:00}.{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}
