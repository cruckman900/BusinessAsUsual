using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using BusinessAsUsual.Admin.Dtos;
using System.Collections.Concurrent;

namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Provides monitoring functionality for platform health by retrieving metrics and alarm states from Amazon
    /// CloudWatch.
    /// </summary>
    /// <remarks>This service caches platform health data for a short period to minimize the number of
    /// requests made to CloudWatch. It aggregates key metrics such as latency, request rates, and error rates, as well
    /// as the current state of relevant alarms for multiple platform services. Intended for use in scenarios where
    /// up-to-date platform health information is required with reduced overhead.</remarks>
    public class MonitoringService : IMonitoringService
    {
        private readonly IAmazonCloudWatch _cloudWatch;
        private readonly IWebHostEnvironment _env;

        // Optional 5-second cache to reduce CloudWatch calls
        private static readonly ConcurrentDictionary<string, (DateTime Timestamp, PlatformHealthDto Data)> _cache
            = new();

        private const int CacheSeconds = 5;

        /// <summary>
        /// Initializes a new instance of the MonitoringService class using the specified Amazon CloudWatch client.
        /// </summary>
        /// <param name="cloudWatch">The IAmazonCloudWatch instance used to interact with Amazon CloudWatch services. This parameter cannot be null.</param>
        /// <param name="env">The IWebHostEnvironment instance providing information about the hosting environment. This parameter cannot be null</param>
        public MonitoringService(IAmazonCloudWatch cloudWatch, IWebHostEnvironment env)
        {
            _cloudWatch = cloudWatch;
            _env = env;
        }

        /// <summary>
        /// Asynchronously retrieves the current health status of the platform, including relevant metrics and active
        /// alarms.
        /// </summary>
        /// <remarks>If a recent health status is available in the cache, the cached data is returned.
        /// Otherwise, the method fetches new metrics and alarms before updating the cache. The returned data reflects
        /// the state of the platform at the time of the request.</remarks>
        /// <returns>A <see cref="PlatformHealthDto"/> object that contains the latest platform health information, including
        /// metrics and alarms.</returns>
        public async Task<PlatformHealthDto> GetPlatformHealthAsync()
        {
            if (!_env.IsProduction())
            {
                return FakePlatformHealth();
            }

            var cacheKey = "platform-health";

            if (_cache.TryGetValue(cacheKey, out var entry))
            {
                if ((DateTime.UtcNow - entry.Timestamp).TotalSeconds < CacheSeconds)
                    return entry.Data;
            }

            var metrics = await FetchMetricsAsync();
            var alarms = await FetchAlarmsAsync();

            var dto = new PlatformHealthDto
            {
                Timestamp = DateTime.UtcNow,
                Metrics = metrics,
                Alarms = alarms.Services,
                Infrastructure = alarms.Infra
            };

            _cache[cacheKey] = (DateTime.UtcNow, dto);

            return dto;
        }

        // ------------------------------------------------------------
        // 0. IF IS DEV, RETURN FAKE METRICS
        // ------------------------------------------------------------

        private PlatformHealthDto FakePlatformHealth()
        {
            return new PlatformHealthDto
            {
                Timestamp = DateTime.UtcNow,
                Metrics = new Dictionary<string, ServiceMetricsDto>
                {
                    ["Backend"] = new ServiceMetricsDto { LatencyMs = 120, RequestsPerMinute = 500, ErrorRate = 0.01 },
                    ["Admin"] = new ServiceMetricsDto { LatencyMs = 80, RequestsPerMinute = 200, ErrorRate = 0.005 },
                    ["Web"] = new ServiceMetricsDto { LatencyMs = 150, RequestsPerMinute = 1000, ErrorRate = 0.02 }
                },
                Alarms = new Dictionary<string, ServiceAlarmDto>
                {
                    ["Backend"] = new ServiceAlarmDto { HighErrorRate = "OK", NoTraffic = "ALARM" },
                    ["Admin"] = new ServiceAlarmDto { HighErrorRate = "OK", NoTraffic = "OK" },
                    ["Web"] = new ServiceAlarmDto { HighErrorRate = "ALARM", NoTraffic = "OK" }
                },
                Infrastructure = new InfrastructureAlarmDto
                {
                    RdsFreeStorageLow = "OK",
                    RdsCpuHigh = "ALARM",
                    RdsFreeableMemoryLow = "OK",
                    RdsConnectionsHigh = "OK",
                    Ec2CpuHigh = "ALARM",
                    Ec2StatusCheckFailed = "OK"
                }
            };
        }

        // ------------------------------------------------------------
        // 1. FETCH METRICS (LatencyMs, RequestsPerMinute, ErrorRate)
        // ------------------------------------------------------------
        private async Task<Dictionary<string, ServiceMetricsDto>> FetchMetricsAsync()
        {
            var end = DateTime.UtcNow;
            var start = end.AddMinutes(-2);

            var request = new GetMetricDataRequest
            {
                StartTime = start,
                EndTime = end,
                ScanBy = ScanBy.TimestampDescending,
                MetricDataQueries = BuildMetricQueries()
            };

            var response = await _cloudWatch.GetMetricDataAsync(request);

            return NormalizeMetrics(response);
        }

        private List<MetricDataQuery> BuildMetricQueries()
        {
            var services = new[] { "Backend", "Admin", "Web" };
            var metrics = new[] { "LatencyMs", "RequestsPerMinute", "ErrorRate" };

            var queries = new List<MetricDataQuery>();
            int id = 0;

            foreach (var service in services)
            {
                foreach (var metric in metrics)
                {
                    queries.Add(new MetricDataQuery
                    {
                        Id = $"m{id++}",
                        MetricStat = new MetricStat
                        {
                            Metric = new Metric
                            {
                                Namespace = "BAU/Platform",
                                MetricName = metric,
                                Dimensions = new List<Dimension>
                            {
                                new Dimension { Name = "ServiceName", Value = service },
                                new Dimension { Name = "Environment", Value = "Production" }
                            }
                            },
                            Period = 60,
                            Stat = "Sum"
                        },
                        ReturnData = true
                    });
                }
            }

            return queries;
        }

        private Dictionary<string, ServiceMetricsDto> NormalizeMetrics(GetMetricDataResponse response)
        {
            var result = new Dictionary<string, ServiceMetricsDto>
            {
                ["Backend"] = new ServiceMetricsDto(),
                ["Admin"] = new ServiceMetricsDto(),
                ["Web"] = new ServiceMetricsDto()
            };

            int index = 0;
            var services = new[] { "Backend", "Admin", "Web" };
            var metrics = new[] { "LatencyMs", "RequestsPerMinute", "ErrorRate" };

            foreach (var service in services)
            {
                foreach (var metric in metrics)
                {
                    var data = response.MetricDataResults[index++];

                    double value = data.Values?.FirstOrDefault() ?? 0;

                    switch (metric)
                    {
                        case "LatencyMs":
                            result[service].LatencyMs = value;
                            break;
                        case "RequestsPerMinute":
                            result[service].RequestsPerMinute = value;
                            break;
                        case "ErrorRate":
                            result[service].ErrorRate = value;
                            break;
                    }
                }
            }

            return result;
        }

        // ------------------------------------------------------------
        // 2. FETCH ALARMS (HighErrorRate + NoTraffic)
        // ------------------------------------------------------------
        private async Task<(Dictionary<string, ServiceAlarmDto> Services, InfrastructureAlarmDto Infra)> FetchAlarmsAsync()
        {
            var response = await _cloudWatch.DescribeAlarmsAsync(new DescribeAlarmsRequest
            {
                AlarmNamePrefix = "BAU-"
            });

            // Service-level alarms
            var services = new Dictionary<string, ServiceAlarmDto>
            {
                ["Backend"] = new ServiceAlarmDto(),
                ["Admin"] = new ServiceAlarmDto(),
                ["Web"] = new ServiceAlarmDto()
            };

            // Infrastructure-level alarms
            var infra = new InfrastructureAlarmDto();

            foreach (var alarm in response.MetricAlarms)
            {
                var name = alarm.AlarmName;
                var state = alarm.StateValue.Value;

                // -------------------------
                // SERVICE-LEVEL ALARMS
                // -------------------------
                if (name.Contains("Backend"))
                    MapAlarm(services["Backend"], name, state);

                if (name.Contains("Admin"))
                    MapAlarm(services["Admin"], name, state);

                if (name.Contains("Web"))
                    MapAlarm(services["Web"], name, state);

                // -------------------------
                // INFRASTRUCTURE ALARMS
                // -------------------------
                if (name.Contains("RDS-FreeStorage-Low"))
                    infra.RdsFreeStorageLow = state;

                if (name.Contains("RDS-CPU-High"))
                    infra.RdsCpuHigh = state;

                if (name.Contains("RDS-FreeableMemory-Low"))
                    infra.RdsFreeableMemoryLow = state;

                if (name.Contains("RDS-Connections-High"))
                    infra.RdsConnectionsHigh = state;

                if (name.Contains("EC2-CPU-High"))
                    infra.Ec2CpuHigh = state;

                if (name.Contains("EC2-StatusCheckFailed"))
                    infra.Ec2StatusCheckFailed = state;
            }

            return (services, infra);
        }

        private void MapAlarm(ServiceAlarmDto dto, string name, StateValue state)
        {
            if (name.Contains("HighErrorRate"))
                dto.HighErrorRate = state.Value;

            if (name.Contains("NoTraffic"))
                dto.NoTraffic = state.Value;
        }
    }
}
