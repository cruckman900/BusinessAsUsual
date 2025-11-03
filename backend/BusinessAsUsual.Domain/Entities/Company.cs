using BusinessAsUsual.Domain.Entities.ProvisionedCompany;

namespace BusinessAsUsual.Domain.Entities;

/// <summary>
/// Represents a company in the system.
/// </summary>
public class Company
{
    /// <summary>
    /// Unique identifier for the company.
    /// </summary>
    public Guid CompanyID { get; set; }

    /// <summary>
    /// Display name of the company.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the company.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the company is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Timestamp when the company was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the company was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // ─────────────────────────────────────────────
    // Navigation properties
    // ─────────────────────────────────────────────

    /// <summary>
    /// Collection of users associated with the company.
    /// </summary>
    public ICollection<CompanyUser>? Users { get; set; }

    /// <summary>
    /// Collection of roles defined for the company.
    /// </summary>
    public ICollection<CompanyUserRole>? Roles { get; set; }

    /// <summary>
    /// Collection of settings configured for the company.
    /// </summary>
    public ICollection<CompanySetting>? Settings { get; set; }

    /// <summary>
    /// Collection of permission groups available to the company.
    /// </summary>
    public ICollection<CompanyUserRolePermissionGroup>? PermissionGroups { get; set; }
}