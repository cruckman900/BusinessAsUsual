using Platform.Application.DTOs;

namespace Platform.Application.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(Guid id);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto);
    Task DeleteRoleAsync(Guid id);
    Task<IEnumerable<RoleDto>> GetRolesByUserAsync(Guid userId);
}
