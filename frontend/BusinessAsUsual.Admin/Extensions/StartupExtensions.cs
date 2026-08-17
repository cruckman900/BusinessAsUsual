using BusinessAsUsual.Admin.Services;
using BusinessAsUsual.Admin.Services.Logs;
using HR.Infrastructure.Persistence;
using HR.Infrastructure.Repositories;
using HR.Infrastructure.Data;
using HR.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
        /// <param name="configuration">The configuration to use for database connections.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddBusinessAsUsualServices(this IServiceCollection services, IConfiguration configuration)
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
            services.AddSingleton<SystemSettingsService>();
            services.AddSignalR();
            services.AddHttpContextAccessor();

            // Add HR Infrastructure
            var hrConnectionString = configuration.GetConnectionString("HRDatabase") ?? "Server=(localdb)\\mssqllocaldb;Database=BAUAdmin_HR;Trusted_Connection=True;MultipleActiveResultSets=true";
            services.AddDbContext<HRDbContext>(options =>
                options.UseSqlServer(hrConnectionString));

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<HRSeedData>();

            return services;
        }
    }
}
