using HR.Domain.Entities;

namespace HR.Domain.Repositories;

/// <summary>
/// Repository interface for Employee operations
/// </summary>
public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(string id);
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task DeleteAsync(string id);
    Task<IEnumerable<Employee>> SearchAsync(string searchTerm);
}
