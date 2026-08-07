namespace Platform.Application.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsSystemPermission { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePermissionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
