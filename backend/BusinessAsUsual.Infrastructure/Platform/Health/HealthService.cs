using BusinessAsUsual.Application.Platform.Health;
using BusinessAsUsual.Domain.Platform.Health;

namespace BusinessAsUsual.Infrastructure.Platform.Health
{
    public class HealthService : IHealthService
    {
        private readonly IHealthMetricsProvider _provider;

        public HealthService(IHealthMetricsProvider provider)
        {
            _provider = provider;
        }

        public HealthStats GetHealth() => _provider.GetStats();
    }
}
