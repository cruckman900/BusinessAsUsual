namespace Platform.Domain.Entities;

/// <summary>
/// Represents a fine-grained permission that can be assigned to roles.
/// </summary>
public class Permission
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "users.create", "roles.delete"
    public string? Description { get; set; }
    public string Module { get; set; } = "Platform"; // Which module this permission belongs to
    public string Resource { get; set; } = string.Empty; // e.g., "users", "roles", "audit_logs"
    public string Action { get; set; } = string.Empty; // e.g., "create", "read", "update", "delete"
    public bool IsSystemPermission { get; set; } = false; // Cannot be deleted if true
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
