using BusinessAsUsual.Admin.Dtos;

namespace BusinessAsUsual.Admin.Services
{
    public class LocalMonitoringService : IMonitoringService
    {
        private readonly Random _rand = new();

        public Task<PlatformHealthDto> GetPlatformHealthAsync()
        {
            return Task.FromResult(new PlatformHealthDto
            {
                Timestamp = DateTime.UtcNow,

                // ------------------------------------------------------------
                // Fake but realistic service metrics
                // ------------------------------------------------------------
                Metrics = new Dictionary<string, ServiceMetricsDto>
                {
                    ["Backend"] = FakeMetrics(100, 600, 0.02),
                    ["Admin"] = FakeMetrics(50, 300, 0.01),
                    ["Web"] = FakeMetrics(120, 900, 0.03)
                },

                // ------------------------------------------------------------
                // Fake but realistic service alarms
                // ------------------------------------------------------------
                Alarms = new Dictionary<string, ServiceAlarmDto>
                {
                    ["Backend"] = FakeServiceAlarms(),
                    ["Admin"] = FakeServiceAlarms(),
                    ["Web"] = FakeServiceAlarms()
                },

                // ------------------------------------------------------------
                // Fake but realistic infrastructure alarms
                // ------------------------------------------------------------
                Infrastructure = new InfrastructureAlarmDto
                {
                    RdsFreeStorageLow = Flip(),
                    RdsCpuHigh = Flip(),
                    RdsFreeableMemoryLow = Flip(),
                    RdsConnectionsHigh = Flip(),
                    Ec2CpuHigh = Flip(),
                    Ec2StatusCheckFailed = Flip()
                }
            });
        }

        // ------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------
        private ServiceMetricsDto FakeMetrics(double baseLatency, double baseRpm, double baseError)
        {
            return new ServiceMetricsDto
            {
                LatencyMs = baseLatency + _rand.Next(-20, 20),
                RequestsPerMinute = baseRpm + _rand.Next(-50, 50),
                ErrorRate = Math.Round(baseError + (_rand.NextDouble() * 0.01 - 0.005), 4)
            };
        }

        private ServiceAlarmDto FakeServiceAlarms()
        {
            return new ServiceAlarmDto
            {
                HighErrorRate = Flip(),
                NoTraffic = Flip()
            };
        }

        private string Flip()
        {
            // 80% OK, 20% ALARM — realistic
            return _rand.NextDouble() < 0.8 ? "OK" : "ALARM";
        }
    }
}