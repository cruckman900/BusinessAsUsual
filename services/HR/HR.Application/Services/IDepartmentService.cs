using HR.Application.DTOs;

namespace HR.Application.Services;

/// <summary>
/// Department service interface
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// Get all departments
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();

    /// <summary>
    /// Get a department by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DepartmentDto?> GetDepartmentByIdAsync(string id);

    /// <summary>
    /// Create a new department
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request);

    /// <summary>
    /// Update an existing department
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<DepartmentDto> UpdateDepartmentAsync(string id, UpdateDepartmentRequest request);

    /// <summary>
    /// Delete a department by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteDepartmentAsync(string id);

    /// <summary>
    /// Search departments by a search term
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    Task<IEnumerable<DepartmentDto>> SearchDepartmentsAsync(string searchTerm);
}
