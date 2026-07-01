namespace HR.Domain.Entities;

/// <summary>
/// Department domain entity supporting hierarchical structure and multiple managers
/// </summary>
public class Department
{
    /// <summary>
    /// Gets or sets the unique identifier for the department.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the name of the department.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the department.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the department code (e.g., "ENG", "HR", "FIN")
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the location/office of the department
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the cost center code for budgeting
    /// </summary>
    public string? CostCenter { get; set; }

    /// <summary>
    /// Gets or sets the parent department ID for hierarchical structure (pods/sub-departments)
    /// </summary>
    public string? ParentDepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the parent department navigation property
    /// </summary>
    public Department? ParentDepartment { get; set; }

    /// <summary>
    /// Gets or sets child departments/pods
    /// </summary>
    public ICollection<Department> SubDepartments { get; set; } = new List<Department>();

    /// <summary>
    /// Legacy: Gets or sets the unique identifier of the single manager (for backward compatibility)
    /// </summary>
    [Obsolete("Use DepartmentManagers navigation property instead")]
    public string? ManagerEmployeeId { get; set; }

    /// <summary>
    /// Legacy: Gets or sets the static employee count (for backward compatibility)
    /// </summary>
    [Obsolete("Calculate dynamically from EmployeeDepartments.Count")]
    public int EmployeeCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the department is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the department was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the department was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// Legacy: Gets or sets the single manager employee (for backward compatibility)
    /// </summary>
    [Obsolete("Use DepartmentManagers navigation property instead")]
    public Employee? Manager { get; set; }

    /// <summary>
    /// Gets or sets the many-to-many relationship with employees
    /// </summary>
    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();

    /// <summary>
    /// Gets or sets the many-to-many relationship with managers
    /// </summary>
    public ICollection<DepartmentManager> DepartmentManagers { get; set; } = new List<DepartmentManager>();

    // Helper properties
    public bool IsSubDepartment => ParentDepartmentId != null;
    public string FullPath => ParentDepartment != null ? $"{ParentDepartment.Name} > {Name}" : Name;
}
