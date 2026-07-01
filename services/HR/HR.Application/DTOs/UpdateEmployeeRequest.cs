namespace HR.Application.DTOs;

/// <summary>
/// Update Employee Request DTO
/// </summary>
public class UpdateEmployeeRequest
{
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PersonalEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // Address
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

    // Employment
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string EmploymentType { get; set; } = "FullTime";
    public DateTime HireDate { get; set; }
    public string Status { get; set; } = "Active";
    public string? WorkLocation { get; set; }
    public string? ManagerId { get; set; }
    public string? SalaryGrade { get; set; }
}
