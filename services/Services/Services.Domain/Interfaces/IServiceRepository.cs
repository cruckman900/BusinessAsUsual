using Services.Domain.Entities;

namespace Services.Domain.Interfaces;

public interface IServiceRepository
{
    Task<List<Service>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Service> CreateAsync(Service service, CancellationToken cancellationToken = default);
    Task UpdateAsync(Service service, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
