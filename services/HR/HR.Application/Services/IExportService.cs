using HR.Application.DTOs;

namespace HR.Application.Services;

/// <summary>
/// Service for exporting data to various formats (CSV, PDF, Excel)
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Generate CSV content from employee data
    /// </summary>
    /// <param name="employees">Collection of employees to export</param>
    /// <returns>Tuple of (base64-encoded CSV content, suggested filename)</returns>
    (string Base64Content, string FileName) GenerateEmployeeCsv(IEnumerable<EmployeeDto> employees);

    /// <summary>
    /// Generate CSV content from department data
    /// </summary>
    /// <param name="departments">Collection of departments to export</param>
    /// <returns>Tuple of (base64-encoded CSV content, suggested filename)</returns>
    (string Base64Content, string FileName) GenerateDepartmentCsv(IEnumerable<DepartmentDto> departments);
}
