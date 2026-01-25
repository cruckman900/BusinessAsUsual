using BusinessAsUsual.Infrastructure.System.Metrics.Telemetry;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BusinessAsUsual.Admin.Services.Metrics
{
    /// <summary>
    /// Provides functionality to collect and report network statistics, such as inbound and outbound throughput, from
    /// the current Linux system.
    /// </summary>
    /// <remarks>This class is designed to read network interface statistics from the Linux /proc/net/dev
    /// file, typically for interfaces named eth0 or ensX. It maintains internal state to calculate throughput over
    /// time. The class is intended for use on Linux environments and may not function correctly on other operating
    /// systems. Instances of this class are not thread-safe.</remarks>
    public class NetworkCollector
    {
        private long _lastBytesIn = 0;
        private long _lastBytesOut = 0;
        private DateTime _lastSampleTime = DateTime.UtcNow;

        /// <summary>
        /// Asynchronously retrieves the current network throughput statistics for the primary network interface.
        /// </summary>
        /// <remarks>This method reads network counters from the Linux /proc/net/dev file and calculates
        /// throughput based on the difference between consecutive samples. It supports interfaces named "eth0" or
        /// starting with "ens", which are common on EC2 and many Linux systems. The returned values reflect the rate
        /// since the previous call to this method. This method is not thread-safe; concurrent calls may result in
        /// inaccurate statistics.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="NetworkStats"/>
        /// object with the number of bytes received and sent per second. If no supported network interface is found,
        /// both values are zero.</returns>
        public Task<NetworkStats> GetNetworkAsync()
        {
            // If not Linux, return zeros
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Task.FromResult(new NetworkStats
                {
                    BytesIn = 0,
                    BytesOut = 0
                });
            }

            // Read from /proc/net/dev - Linux network counters
            var lines = File.ReadAllLines("/proc/net/dev")
                .Skip(2) // Skip headers
                .Where(l => l.Contains("eth0") || l.Contains("ens"))    // EC2 uses eth0 or ensX
                .ToList();

            if (!lines.Any())
            {
                return Task.FromResult(new NetworkStats
                {
                    BytesIn = 0,
                    BytesOut = 0
                });
            }

            var parts = lines.First()
                .Split(':')[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            long bytesIn = long.Parse(parts[0]);
            long bytesOut = long.Parse(parts[8]);

            // Calculate throughput (bytes per second)
            var now = DateTime.UtcNow;
            var seconds = (now - _lastSampleTime).TotalSeconds;

            long deltaIn = _lastBytesIn == 0 ? 0 : bytesIn - _lastBytesIn;
            long deltaOut = _lastBytesOut == 0 ? 0 : bytesOut - _lastBytesOut;

            long inPerSecond = seconds > 0 ? (long)(deltaIn / seconds) : 0;
            long outPerSecond = seconds > 0 ? (long)(deltaOut / seconds) : 0;

            // Update state
            _lastBytesIn = bytesIn;
            _lastBytesOut = bytesOut;
            _lastSampleTime = now;

            return Task.FromResult(new NetworkStats
            {
                BytesIn = inPerSecond,
                BytesOut = outPerSecond
            });
        }
    }
}
