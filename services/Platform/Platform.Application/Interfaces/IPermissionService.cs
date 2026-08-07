using Platform.Application.DTOs;

namespace Platform.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
    Task<PermissionDto?> GetPermissionByIdAsync(Guid id);
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createPermissionDto);
    Task DeletePermissionAsync(Guid id);
    Task<IEnumerable<PermissionDto>> GetPermissionsByModuleAsync(string module);
}
