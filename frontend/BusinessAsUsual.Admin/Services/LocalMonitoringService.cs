using BusinessAsUsual.Admin.Dtos;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Provides a local implementation of platform health monitoring services, supplying simulated metrics and alarm
    /// data for development and testing purposes.
    /// </summary>
    /// <remarks>This service generates realistic but artificial health metrics and alarms for backend, admin,
    /// and web services, as well as infrastructure alarms. It is intended for use in local or non-production
    /// environments where real monitoring data is unavailable or unnecessary. The generated data mimics typical service
    /// behavior to facilitate UI development and integration testing without requiring live infrastructure.</remarks>
    public class LocalMonitoringService : IMonitoringService
    {
        private readonly Random _rand = new();

        /// <summary>
        /// Asynchronously retrieves a snapshot of the platform's current health, including service metrics and
        /// infrastructure alarms.
        /// </summary>
        /// <remarks>The returned health data includes metrics and alarms for various services and
        /// infrastructure components. The values are simulated and intended for demonstration or testing purposes
        /// only.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="PlatformHealthDto"/> object with the platform's health information at the time of the request.</returns>
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