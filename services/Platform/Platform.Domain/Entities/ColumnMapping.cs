namespace Platform.Domain.Entities;

/// <summary>
/// Stores user-defined column mappings for reuse in future imports
/// </summary>
public class ColumnMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant/company identifier
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Target table name
    /// </summary>
    public string TargetTable { get; set; } = string.Empty;

    /// <summary>
    /// Friendly name for this mapping template
    /// </summary>
    public string MappingName { get; set; } = string.Empty;

    /// <summary>
    /// Source column name from the import file
    /// </summary>
    public string SourceColumn { get; set; } = string.Empty;

    /// <summary>
    /// Target column name in the database table
    /// </summary>
    public string TargetColumn { get; set; } = string.Empty;

    /// <summary>
    /// Optional transformation to apply (e.g., "SplitFullName", "TrimWhitespace", "FormatDate")
    /// </summary>
    public string? Transformation { get; set; }

    /// <summary>
    /// Transformation parameters (JSON)
    /// </summary>
    public string? TransformationParams { get; set; }

    /// <summary>
    /// User who created this mapping
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// When this mapping was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
