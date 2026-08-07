using Platform.Domain.Entities;

namespace Platform.Domain.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<Permission?> GetByIdAsync(Guid id);
    Task<Permission?> GetByNameAsync(string name);
    Task<Permission> AddAsync(Permission permission);
    Task<Permission> UpdateAsync(Permission permission);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(Guid roleId);
    Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(string module);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
}
