namespace HR.Contracts.UIModels;

/// <summary>
/// Mobile UI view model for Employee
/// </summary>
public class EmployeeViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
