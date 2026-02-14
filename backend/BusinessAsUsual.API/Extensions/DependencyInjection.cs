using BusinessAsUsual.Application.Platform.Health;
using BusinessAsUsual.Application.Platform.Metrics;
using BusinessAsUsual.Infrastructure.Platform.Health;
using BusinessAsUsual.Infrastructure.Platform.Metrics;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace BusinessAsUsual.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPlatformMetrics(this IServiceCollection services)
        {
            services.AddSingleton<CpuCollector>();
            services.AddSingleton<MemoryCollector>();
            services.AddSingleton<DiskCollector>();
            services.AddSingleton<NetworkCollector>();
            services.AddSingleton<UptimeCollector>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.AddSingleton<IHealthMetricsProvider, WindowsHealthMetricsProvider>();
            }
            else
            {
                services.AddSingleton<IHealthMetricsProvider, LinuxHealthMetricsProvider>();
            }

            services.AddSingleton<IMetricsService, MetricsService>();
            services.AddSingleton<IHealthService, HealthService>();

            return services;
        }
    }
}
