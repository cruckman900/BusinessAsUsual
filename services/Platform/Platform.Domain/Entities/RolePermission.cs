namespace Platform.Domain.Entities;

/// <summary>
/// Junction table linking Roles to Permissions (many-to-many relationship).
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedBy { get; set; } // UserId of admin who assigned this permission
}
