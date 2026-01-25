using BusinessAsUsual.Admin.Services.Metrics;
using BusinessAsUsual.Infrastructure.System.Metrics.Telemetry;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.Controllers
{
    /// <summary>
    /// Provides API endpoints for retrieving system metrics such as CPU, memory, disk, network, and uptime statistics.
    /// </summary>
    /// <remarks>This controller is intended for use in monitoring and diagnostics scenarios where real-time
    /// system performance data is required. All endpoints are grouped under the "metrics" area.</remarks>
    [ApiController]
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class MetricsApiController : ControllerBase
    {
        private readonly CpuCollector _cpu;
        private readonly MemoryCollector _memory;
        private readonly DiskCollector _disk;
        private readonly NetworkCollector _network;
        private readonly UptimeCollector _uptime;

        /// <summary>
        /// Initializes a new instance of the MetricsController class with the specified collectors for CPU, memory,
        /// disk, network, and uptime metrics.
        /// </summary>
        /// <param name="cpu">The CPU metrics collector used to gather processor usage data. Cannot be null.</param>
        /// <param name="memory">The memory metrics collector used to gather memory usage data. Cannot be null.</param>
        /// <param name="disk">The disk metrics collector used to gather disk usage data. Cannot be null.</param>
        /// <param name="network">The network metrics collector used to gather network usage data. Cannot be null.</param>
        /// <param name="uptime">The uptime metrics collector used to gather system uptime data. Cannot be null.</param>
        public MetricsApiController(CpuCollector cpu, MemoryCollector memory, DiskCollector disk, NetworkCollector network, UptimeCollector uptime)
        {
            _cpu = cpu;
            _memory = memory;
            _disk = disk;
            _network = network;
            _uptime = uptime;
        }

        /// <summary>
        /// Retrieves current system metrics, including CPU, memory, disk, network, and uptime statistics.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing a <see cref="SystemMetrics"/> object with the latest system
        /// statistics.</returns>
        [HttpGet("system")]
        public async Task<IActionResult> GetSystemMetrics()
        {
            var cpuPercent = await _cpu.GetCpuUsageAsync();
            var memory = await _memory.GetMemoryAsync();
            var disk = await _disk.GetDiskAsync();
            var network = await _network.GetNetworkAsync();
            var uptime = await _uptime.GetUptimeAsync();

            return Ok(new SystemMetrics
            {
                Cpu = new CpuStats { Percent = cpuPercent },
                Memory = memory,
                Disk = disk,
                Network = network,
                Uptime = uptime
            });
        }
    }
}
