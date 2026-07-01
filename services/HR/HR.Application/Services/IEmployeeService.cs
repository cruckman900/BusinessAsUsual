using HR.Application.DTOs;

namespace HR.Application.Services;

/// <summary>
/// Employee service interface
/// </summary>
public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeByIdAsync(string id);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<EmployeeDto> UpdateEmployeeAsync(string id, UpdateEmployeeRequest request);
    Task DeleteEmployeeAsync(string id);
    Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm);
}
