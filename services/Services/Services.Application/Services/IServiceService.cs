namespace Services.Application.Services;

public interface IServiceService
{
    Task<IEnumerable<DTOs.ServiceDto>> GetAllServicesAsync();
    Task<DTOs.ServiceDto?> GetServiceByIdAsync(int id);
}
