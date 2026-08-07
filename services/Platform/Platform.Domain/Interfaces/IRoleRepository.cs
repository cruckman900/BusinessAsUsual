using Platform.Domain.Entities;

namespace Platform.Domain.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<Role> AddAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Role>> GetRolesByUserAsync(Guid userId);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
}
