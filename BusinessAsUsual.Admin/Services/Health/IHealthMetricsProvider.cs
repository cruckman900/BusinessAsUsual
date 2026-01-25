using BusinessAsUsual.Infrastructure.System.Metrics.Telemetry;

namespace BusinessAsUsual.Admin.Services.Health
{
    /// <summary>
    /// Defines a contract for retrieving health metrics for a system or component.
    /// </summary>
    /// <remarks>Implementations of this interface provide access to health-related statistics, which can be
    /// used for monitoring, diagnostics, or reporting purposes. The specific metrics and their interpretation may vary
    /// depending on the implementation.</remarks>
    public interface IHealthMetricsProvider
    {
        /// <summary>
        /// Retrieves the current health statistics for the system.
        /// </summary>
        /// <returns>A <see cref="HealthStats"/> object containing the latest health metrics. The returned object provides
        /// detailed information about the system's health status.</returns>
        HealthStats GetStats();
    }
}
