using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BusinessAsUsual.Admin.Controllers
{

    /// <summary>
    /// Provides administrative endpoints for retrieving system resource statistics, including CPU, memory, disk usage, and
    /// server uptime.
    /// </summary>
    /// <remarks>This controller is intended for use by system administrators to monitor server health and resource
    /// utilization. All endpoints are accessible under the 'admin/system' route. The statistics returned reflect the
    /// current state of the server at the time of the request.</remarks>
    [ApiController]
    [Route("admin/system")]
    public class SystemStatsController : ControllerBase
    {
        private static readonly DateTime _startTime = DateTime.UtcNow;

        /// <summary>
        /// Retrieves current system statistics, including CPU usage, memory usage, disk usage, and application uptime.
        /// </summary>
        /// <remarks>This endpoint is typically used for monitoring the application's resource consumption and
        /// operational status. The uptime value is calculated from the time the application started and is formatted as
        /// "dd.hh:mm:ss". All statistics reflect the state at the time of the request.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a JSON object with the current CPU, memory, and disk usage, as well as
        /// the application's uptime formatted as days, hours, minutes, and seconds.</returns>
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var cpu = GetCpuUsage();
            var memory = GetMemoryUsage();
            var disk = GetDiskUsage();
            var uptime = DateTime.UtcNow - _startTime;

            return Ok(new
            {
                cpu,
                memory,
                disk,
                uptime = uptime.ToString(@"dd\.hh\:mm\:ss")
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private double GetCpuUsage()
        {
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(250);
            return Math.Round(cpuCounter.NextValue(), 2);
        }

        private object GetMemoryUsage()
        {
            var info = GC.GetGCMemoryInfo();
            return new
            {
                used = info.HeapSizeBytes / 1024 / 1024,
                total = info.TotalAvailableMemoryBytes / 1024 / 1024
            };
        }

        private object GetDiskUsage()
        {
            var drive = new DriveInfo("C");
            return new
            {
                used = (drive.TotalSize - drive.TotalFreeSpace) / 1024 / 1024 / 1024,
                total = drive.TotalSize / 1024 / 1024 / 1024
            };
        }
    }
}