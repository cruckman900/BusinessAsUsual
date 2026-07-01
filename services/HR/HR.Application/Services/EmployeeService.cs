using HR.Application.DTOs;
using HR.Domain.Entities;
using HR.Domain.Repositories;

namespace HR.Application.Services;

/// <summary>
/// Employee service implementing business logic
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _repository.GetAllAsync();
        return employees.Select(MapToDto);
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(string id)
    {
        var employee = await _repository.GetByIdAsync(id);
        return employee == null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        // Parse status string to enum
        if (!Enum.TryParse<EmploymentStatus>(request.Status, out var status))
        {
            status = EmploymentStatus.Active;
        }

        // Parse employment type string to enum
        if (!Enum.TryParse<EmploymentType>(request.EmploymentType, out var employmentType))
        {
            employmentType = EmploymentType.FullTime;
        }

#pragma warning disable CS0618 // Type or member is obsolete - setting legacy Department field for backward compatibility
        var employee = new Employee
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PersonalEmail = request.PersonalEmail,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Department = request.Department,
            JobTitle = request.JobTitle,
            EmploymentType = employmentType,
            HireDate = request.HireDate,
            Status = status,
            WorkLocation = request.WorkLocation,
            ManagerId = request.ManagerId,
            SalaryGrade = request.SalaryGrade,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            EmergencyContactRelationship = request.EmergencyContactRelationship,
            CreatedAt = DateTime.UtcNow
        };
#pragma warning restore CS0618

        var created = await _repository.CreateAsync(employee);
        return MapToDto(created);
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(string id, UpdateEmployeeRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} not found");
        }

        // Parse status string to enum
        if (!Enum.TryParse<EmploymentStatus>(request.Status, out var status))
        {
            status = EmploymentStatus.Active;
        }

        // Parse employment type string to enum
        if (!Enum.TryParse<EmploymentType>(request.EmploymentType, out var employmentType))
        {
            employmentType = EmploymentType.FullTime;
        }

#pragma warning disable CS0618 // Type or member is obsolete - setting legacy Department field for backward compatibility
        existing.FirstName = request.FirstName;
        existing.LastName = request.LastName;
        existing.Email = request.Email;
        existing.PersonalEmail = request.PersonalEmail;
        existing.PhoneNumber = request.PhoneNumber;
        existing.DateOfBirth = request.DateOfBirth;
        existing.Department = request.Department;
        existing.JobTitle = request.JobTitle;
        existing.EmploymentType = employmentType;
        existing.HireDate = request.HireDate;
        existing.Status = status;
        existing.WorkLocation = request.WorkLocation;
        existing.ManagerId = request.ManagerId;
        existing.SalaryGrade = request.SalaryGrade;
        existing.AddressLine1 = request.AddressLine1;
        existing.AddressLine2 = request.AddressLine2;
        existing.City = request.City;
        existing.State = request.State;
        existing.PostalCode = request.PostalCode;
        existing.Country = request.Country;
        existing.EmergencyContactName = request.EmergencyContactName;
        existing.EmergencyContactPhone = request.EmergencyContactPhone;
        existing.EmergencyContactRelationship = request.EmergencyContactRelationship;
        existing.UpdatedAt = DateTime.UtcNow;
#pragma warning restore CS0618

        var updated = await _repository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    public async Task DeleteEmployeeAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm)
    {
        var employees = await _repository.SearchAsync(searchTerm);
        return employees.Select(MapToDto);
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PersonalEmail = employee.PersonalEmail,
            PhoneNumber = employee.PhoneNumber,
            DateOfBirth = employee.DateOfBirth,
            PhotoUrl = employee.PhotoUrl,

            // Address
            AddressLine1 = employee.AddressLine1,
            AddressLine2 = employee.AddressLine2,
            City = employee.City,
            State = employee.State,
            PostalCode = employee.PostalCode,
            Country = employee.Country,

            // Emergency Contact
            EmergencyContactName = employee.EmergencyContactName,
            EmergencyContactPhone = employee.EmergencyContactPhone,
            EmergencyContactRelationship = employee.EmergencyContactRelationship,

            // Employment
            JobTitle = employee.JobTitle,
            EmploymentType = employee.EmploymentType.ToString(),
            Status = employee.Status.ToString(),
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            TerminationReason = employee.TerminationReason,

            // Work
            WorkLocation = employee.WorkLocation,
            ManagerId = employee.ManagerId,
            ManagerName = employee.Manager != null ? $"{employee.Manager.FirstName} {employee.Manager.LastName}" : null,

#pragma warning disable CS0618 // Type or member is obsolete - using legacy Department field for fallback compatibility
            // Legacy
            Department = employee.Department ?? employee.EmployeeDepartments
                .Where(ed => ed.IsPrimary)
                .Select(ed => ed.Department?.Name)
                .FirstOrDefault(),
#pragma warning restore CS0618

            // Compensation
            SalaryGrade = employee.SalaryGrade,

            // Department memberships
            Departments = employee.EmployeeDepartments
                .Where(ed => ed.LeftDate == null)
                .Select(ed => new EmployeeDepartmentDto
                {
                    DepartmentId = ed.DepartmentId,
                    DepartmentName = ed.Department?.Name ?? "Unknown",
                    IsPrimary = ed.IsPrimary,
                    AllocationPercentage = ed.AllocationPercentage,
                    JoinedDate = ed.JoinedDate
                })
                .ToList(),

            // Managed departments
            ManagedDepartmentNames = employee.ManagedDepartments
                .Where(dm => dm.EndDate == null)
                .Select(dm => dm.Department?.Name ?? "Unknown")
                .ToList()
        };
    }
}
