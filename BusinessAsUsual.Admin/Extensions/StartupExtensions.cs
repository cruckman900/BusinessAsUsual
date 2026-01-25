using BusinessAsUsual.Admin.Services;
using BusinessAsUsual.Admin.Services.Health;
using BusinessAsUsual.Admin.Services.Logs;
using BusinessAsUsual.Admin.Services.Metrics;

namespace BusinessAsUsual.Admin.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring services in the Business As Usual application.
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Registers core services required by the Business As Usual admin backend.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddBusinessAsUsualServices(this IServiceCollection services)
        {
            //if (builder.Environment.IsDevelopment())
            //{
            //    builder.Services.AddSingleton<ILogReader, LocalLogReader>();
            //}
            //else
            //{
            //    builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            //    builder.Services.AddAWSService<IAmazonCloudWatchLogs>();
            //    builder.Services.AddSingleton<ILogReader, CloudWatchLogReader>();
            //}
            // TEMPORARY: Always use LocalLogReader
            //builder.Services.AddSingleton<ILogReader, CloudWatchLogReader>();

            services.AddScoped<TenantMetadataService>();
            services.AddScoped<ISmartCommitLogger, SmartCommitLogger>();
            services.AddSingleton<LogQueryService>();
            services.AddSingleton<ILogReader, LocalLogReader>();
            services.AddSingleton<EnvironmentService>();
            services.AddSingleton<SystemSettingsService>();
            services.AddSingleton<CpuCollector>();
            services.AddSingleton<MemoryCollector>();
            services.AddSingleton<DiskCollector>();
            services.AddSingleton<NetworkCollector>();
            services.AddSingleton<UptimeCollector>();
            services.AddSignalR();
            services.AddHttpContextAccessor();

            if (OperatingSystem.IsWindows())
                services.AddSingleton<IHealthMetricsProvider, WindowsHealthMetricsProvider>();
            else
                services.AddSingleton<IHealthMetricsProvider, LinuxHealthMetricsProvider>();
            return services;
        }
    }
}