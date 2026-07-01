using HR.Domain.Entities;

namespace HR.Application.DTOs;

/// <summary>
/// Employee Data Transfer Object
/// </summary>
public class EmployeeDto
{
    public string Id { get; set; } = string.Empty;

    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PersonalEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhotoUrl { get; set; }

    // Address Information
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Emergency Contact
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }

    // Employment Information
    public string? JobTitle { get; set; }
    public string EmploymentType { get; set; } = nameof(Domain.Entities.EmploymentType.FullTime);
    public string Status { get; set; } = nameof(EmploymentStatus.Active);
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? TerminationReason { get; set; }

    // Work Location & Reporting
    public string? WorkLocation { get; set; }
    public string? ManagerId { get; set; }
    public string? ManagerName { get; set; }  // Computed for display

    // Legacy: Keep for backward compatibility
    public string? Department { get; set; }  // Will hold primary department name

    // Compensation
    public string? SalaryGrade { get; set; }

    // Department Memberships (new)
    public List<EmployeeDepartmentDto> Departments { get; set; } = new();

    // Managed Departments (new)
    public List<string> ManagedDepartmentNames { get; set; } = new();

    // Computed Properties
    public string FullName => $"{FirstName} {LastName}";
    public int? Age => DateOfBirth.HasValue ? (int)((DateTime.Now - DateOfBirth.Value).TotalDays / 365.25) : null;
}

/// <summary>
/// Represents an employee's department membership
/// </summary>
public class EmployeeDepartmentDto
{
    public string DepartmentId { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int? AllocationPercentage { get; set; }
    public DateTime JoinedDate { get; set; }
}
