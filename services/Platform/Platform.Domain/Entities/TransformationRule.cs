namespace Platform.Domain.Entities;

/// <summary>
/// Stores reusable transformation rules for data imports
/// </summary>
public class TransformationRule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant/company identifier
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Friendly name for this transformation
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Transformation type (e.g., "SplitFullName", "FormatDate", "TrimWhitespace")
    /// </summary>
    public string TransformationType { get; set; } = string.Empty;

    /// <summary>
    /// Transformation configuration (JSON)
    /// </summary>
    public string ConfigurationJson { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this transformation does
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// User who created this rule
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// When this rule was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this is a system-provided rule or user-created
    /// </summary>
    public bool IsSystemRule { get; set; } = false;
}
