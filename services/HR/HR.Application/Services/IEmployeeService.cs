using HR.Application.DTOs;

namespace HR.Application.Services;

/// <summary>
/// Employee service interface
/// </summary>
public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request);
    Task DeleteEmployeeAsync(Guid id);
    Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm);
}
