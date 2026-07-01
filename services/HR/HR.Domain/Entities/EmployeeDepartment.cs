namespace HR.Domain.Entities;

/// <summary>
/// Junction entity for Employee-Department many-to-many relationship
/// </summary>
public class EmployeeDepartment
{
    /// <summary>
    /// Gets or sets the employee ID
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department ID
    /// </summary>
    public string DepartmentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this is the employee's primary department
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Gets or sets the percentage of time allocated to this department (e.g., 50 for 50%)
    /// </summary>
    public int? AllocationPercentage { get; set; }

    /// <summary>
    /// Gets or sets the date the employee joined this department
    /// </summary>
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date the employee left this department (null if still active)
    /// </summary>
    public DateTime? LeftDate { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;
    public Department Department { get; set; } = null!;
}
