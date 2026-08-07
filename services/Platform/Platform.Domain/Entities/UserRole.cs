namespace Platform.Domain.Entities;

/// <summary>
/// Junction table linking Users to Roles (many-to-many relationship).
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedBy { get; set; } // UserId of admin who assigned this role
}
