namespace HR.Domain.Entities;

/// <summary>
/// Employee domain entity
/// </summary>
public class Employee
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? PersonalEmail { get; set; }
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
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? TerminationReason { get; set; }

    // Work Location & Reporting
    public string? WorkLocation { get; set; }  // Office location or "Remote"
    public string? ManagerId { get; set; }  // Direct manager/supervisor
    public Employee? Manager { get; set; }  // Navigation property

    // Legacy: Keep for backward compatibility, will be replaced by many-to-many
    [Obsolete("Use EmployeeDepartments navigation property instead")]
    public string? Department { get; set; }

    // Compensation (consider security/access control in real apps)
    public string? SalaryGrade { get; set; }  // e.g., "L3", "Senior", "Grade 5"

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();
    public ICollection<DepartmentManager> ManagedDepartments { get; set; } = new List<DepartmentManager>();
    public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

    // Computed Properties
    public string FullName => $"{FirstName} {LastName}";
    public int? Age => DateOfBirth.HasValue ? (int)((DateTime.Now - DateOfBirth.Value).TotalDays / 365.25) : null;
}

public enum EmploymentStatus
{
    Active,
    OnLeave,
    Terminated,
    Retired
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Intern,
    Temporary
}
