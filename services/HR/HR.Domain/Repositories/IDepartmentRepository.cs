using HR.Domain.Entities;

namespace HR.Domain.Repositories;

/// <summary>
/// Repository interface for Department operations
/// </summary>
public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(string id);
    Task<Department> CreateAsync(Department department);
    Task<Department> UpdateAsync(Department department);
    Task DeleteAsync(string id);
}
