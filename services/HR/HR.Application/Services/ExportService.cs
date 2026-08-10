using System.Text;
using HR.Application.DTOs;

namespace HR.Application.Services;

/// <summary>
/// Service for exporting data to various formats (CSV, PDF, Excel)
/// Extracted from Razor components to enable unit testing
/// </summary>
public class ExportService : IExportService
{
    /// <summary>
    /// Generate CSV content from employee data
    /// </summary>
    /// <param name="employees">Collection of employees to export</param>
    /// <returns>Tuple of (base64-encoded CSV content, suggested filename)</returns>
    public (string Base64Content, string FileName) GenerateEmployeeCsv(IEnumerable<EmployeeDto> employees)
    {
        var employeeList = employees?.ToList() ?? new List<EmployeeDto>();

        var csv = new StringBuilder();
        csv.AppendLine("Full Name,Email,Department,Hire Date");

        foreach (var employee in employeeList)
        {
            var dept = employee.Department ?? "";
            csv.AppendLine($"\"{employee.FirstName} {employee.LastName}\",\"{employee.Email}\",\"{dept}\",\"{employee.HireDate:yyyy-MM-dd}\"");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var base64 = Convert.ToBase64String(bytes);
        var fileName = $"employees-export-{DateTime.Now:yyyy-MM-dd}.csv";

        return (base64, fileName);
    }

    /// <summary>
    /// Generate CSV content from department data
    /// </summary>
    /// <param name="departments">Collection of departments to export</param>
    /// <returns>Tuple of (base64-encoded CSV content, suggested filename)</returns>
    public (string Base64Content, string FileName) GenerateDepartmentCsv(IEnumerable<DepartmentDto> departments)
    {
        var departmentList = departments?.ToList() ?? new List<DepartmentDto>();

        var csv = new StringBuilder();
        csv.AppendLine("Name,Employee Count,Description");

        foreach (var dept in departmentList)
        {
            var description = dept.Description ?? "";
            csv.AppendLine($"\"{dept.Name}\",\"{dept.EmployeeCount}\",\"{description}\"");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var base64 = Convert.ToBase64String(bytes);
        var fileName = $"departments-export-{DateTime.Now:yyyy-MM-dd}.csv";

        return (base64, fileName);
    }
}
