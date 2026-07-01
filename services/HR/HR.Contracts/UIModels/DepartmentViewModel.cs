namespace HR.Contracts.UIModels;

/// <summary>
/// Mobile UI view model for Department
/// </summary>
public class DepartmentViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public string? ManagerName { get; set; }
}
