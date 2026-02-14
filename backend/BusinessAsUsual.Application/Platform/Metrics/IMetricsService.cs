using BusinessAsUsual.Domain.Platform.Metrics;

namespace BusinessAsUsual.Application.Platform.Metrics
{
    public interface IMetricsService
    {
        Task<SystemMetrics> GetSystemMetricsAsync();
    }
}
