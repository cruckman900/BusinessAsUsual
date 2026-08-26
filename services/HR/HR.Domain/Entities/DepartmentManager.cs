namespace HR.Domain.Entities;

/// <summary>
/// Junction entity for Department-Manager many-to-many relationship
/// Allows multiple managers per department with different roles
/// </summary>
public class DepartmentManager
{
    /// <summary>
    /// Tenant/company identifier for multi-tenant isolation
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the department ID
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the employee ID of the manager
    /// </summary>
    public Guid ManagerId { get; set; }

    /// <summary>
    /// Gets or sets the management role/title for this department
    /// (e.g., "Department Head", "Team Lead", "Assistant Manager")
    /// </summary>
    public string? ManagerRole { get; set; }

    /// <summary>
    /// Gets or sets whether this is the primary/head manager for the department
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Gets or sets the date this manager started managing this department
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date this manager stopped managing this department (null if still active)
    /// </summary>
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public Department Department { get; set; } = null!;
    public Employee Manager { get; set; } = null!;
}
