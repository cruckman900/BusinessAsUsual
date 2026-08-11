using Services.Application.DTOs;

namespace Services.Application.Services;

/// <summary>
/// Mock service implementation for Services module
/// Returns empty collections to prevent errors when API is unavailable
/// </summary>
public class MockServiceService : IServiceService
{
    public Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        return Task.FromResult(Enumerable.Empty<ServiceDto>());
    }

    public Task<ServiceDto?> GetServiceByIdAsync(Guid id)
    {
        return Task.FromResult<ServiceDto?>(null);
    }

    public Task<ServiceDto> CreateServiceAsync(ServiceDto service)
    {
        // Return the service with a new ID
        service.Id = Guid.NewGuid();
        return Task.FromResult(service);
    }

    public Task<ServiceDto> UpdateServiceAsync(Guid id, ServiceDto service)
    {
        // Return the updated service
        service.Id = id;
        return Task.FromResult(service);
    }

    public Task<bool> DeleteServiceAsync(Guid id)
    {
        // Return success
        return Task.FromResult(true);
    }
}
