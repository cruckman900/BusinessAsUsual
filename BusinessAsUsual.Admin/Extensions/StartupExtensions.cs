using BusinessAsUsual.Admin.Services;

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
            services.AddScoped<TenantMetadataService>();
            services.AddScoped<CompanyProvisioner>();
            return services;
        }
    }
}