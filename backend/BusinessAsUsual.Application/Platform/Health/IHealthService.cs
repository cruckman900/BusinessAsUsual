using BusinessAsUsual.Domain.Platform.Health;

namespace BusinessAsUsual.Application.Platform.Health
{
    public interface IHealthService
    {
        HealthStats GetHealth();
    }
}
