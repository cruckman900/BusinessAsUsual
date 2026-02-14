using BusinessAsUsual.Application.Platform.Metrics;
using BusinessAsUsual.Domain.Platform.Metrics;
using System.Threading;

namespace BusinessAsUsual.Infrastructure.Platform.Metrics
{
    public class MetricsService : IMetricsService
    {
        private readonly CpuCollector _cpu;
        private readonly MemoryCollector _memory;
        private readonly DiskCollector _disk;
        private readonly NetworkCollector _network;
        private readonly UptimeCollector _uptime;

        public MetricsService(
            CpuCollector cpu,
            MemoryCollector memory,
            DiskCollector disk,
            NetworkCollector network,
            UptimeCollector uptime)
        {
            _cpu = cpu;
            _memory = memory;
            _disk = disk;
            _network = network;
            _uptime = uptime;
        }

        public async Task<SystemMetrics> GetSystemMetricsAsync()
        {
            var cpuTask = _cpu.GetCpuUsageAsync();
            var memoryTask = _memory.GetMemoryAsync();
            var diskTask = _disk.GetDiskAsync();
            var networkTask = _network.GetNetworkAsync();
            var uptimeTask = _uptime.GetUptimeAsync();

            await Task.WhenAll(cpuTask, memoryTask, diskTask, networkTask, uptimeTask);

            return new SystemMetrics
            {
                Cpu = await cpuTask,
                Memory = await memoryTask,
                Disk = await diskTask,
                Network = await networkTask,
                Uptime = await uptimeTask
            };
        }
    }
}
