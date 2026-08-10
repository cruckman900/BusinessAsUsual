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

    public Task<ServiceDto?> GetServiceByIdAsync(int id)
    {
        return Task.FromResult<ServiceDto?>(null);
    }
}
