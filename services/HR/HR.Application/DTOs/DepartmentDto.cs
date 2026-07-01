namespace HR.Application.DTOs;

/// <summary>
/// Department Data Transfer Object
/// </summary>
public class DepartmentDto
{
    /// <summary>
    /// The unique identifier of the department
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the department
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the department
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The department code (e.g., "ENG", "HR", "FIN")
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// The location/office of the department
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// The cost center code for budgeting
    /// </summary>
    public string? CostCenter { get; set; }

    /// <summary>
    /// The parent department ID (for sub-departments/pods)
    /// </summary>
    public string? ParentDepartmentId { get; set; }

    /// <summary>
    /// The parent department name
    /// </summary>
    public string? ParentDepartmentName { get; set; }

    /// <summary>
    /// Full hierarchical path (e.g., "Engineering > Team A")
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// Legacy: The employee ID of the primary manager (for backward compatibility)
    /// </summary>
    public string? ManagerEmployeeId { get; set; }

    /// <summary>
    /// Legacy: The name of the primary manager (for backward compatibility)
    /// </summary>
    public string? ManagerName { get; set; }

    /// <summary>
    /// List of all managers for this department
    /// </summary>
    public List<DepartmentManagerDto> Managers { get; set; } = new();

    /// <summary>
    /// The computed number of active employees in the department
    /// </summary>
    public int EmployeeCount { get; set; }

    /// <summary>
    /// The number of sub-departments/pods under this department
    /// </summary>
    public int SubDepartmentCount { get; set; }

    /// <summary>
    /// Indicates whether the department is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates whether this is a sub-department/pod
    /// </summary>
    public bool IsSubDepartment { get; set; }

    /// <summary>
    /// The date and time when the department was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Represents a department manager
/// </summary>
public class DepartmentManagerDto
{
    public string ManagerId { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string? ManagerRole { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime StartDate { get; set; }
}

/// <summary>
/// Request model for creating a department
/// </summary>
public class CreateDepartmentRequest
{
    /// <summary>
    /// The name of the department
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the department
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The department code
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// The location/office
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// The cost center code
    /// </summary>
    public string? CostCenter { get; set; }

    /// <summary>
    /// The parent department ID (for sub-departments)
    /// </summary>
    public string? ParentDepartmentId { get; set; }

    /// <summary>
    /// Legacy: The employee ID of the primary manager (for backward compatibility)
    /// </summary>
    public string? ManagerEmployeeId { get; set; }

    /// <summary>
    /// List of manager IDs to assign to this department
    /// </summary>
    public List<string>? ManagerIds { get; set; }

    /// <summary>
    /// Indicates whether the department is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Request model for updating a department
/// </summary>
public class UpdateDepartmentRequest
{
    /// <summary>
    /// The name of the department
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the department
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The department code
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// The location/office
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// The cost center code
    /// </summary>
    public string? CostCenter { get; set; }

    /// <summary>
    /// The parent department ID
    /// </summary>
    public string? ParentDepartmentId { get; set; }

    /// <summary>
    /// Legacy: The employee ID of the primary manager
    /// </summary>
    public string? ManagerEmployeeId { get; set; }

    /// <summary>
    /// List of manager IDs (replaces existing managers)
    /// </summary>
    public List<string>? ManagerIds { get; set; }

    /// <summary>
    /// Indicates whether the department is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
