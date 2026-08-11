namespace Services.Application.Services;

public interface IServiceService
{
    Task<IEnumerable<DTOs.ServiceDto>> GetAllServicesAsync();
    Task<DTOs.ServiceDto?> GetServiceByIdAsync(Guid id);
    Task<DTOs.ServiceDto> CreateServiceAsync(DTOs.ServiceDto service);
    Task<DTOs.ServiceDto> UpdateServiceAsync(Guid id, DTOs.ServiceDto service);
    Task<bool> DeleteServiceAsync(Guid id);
}
