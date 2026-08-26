using System.ComponentModel.DataAnnotations;

namespace Platform.Domain.Entities;

/// <summary>
/// Stores reusable column mapping templates for import operations
/// </summary>
public class MappingTemplate
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant/Company ID that owns this template
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Target table name this template is for
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// User-friendly name for this template
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Serialized JSON of ColumnMappingAnalysis (mappings + transformations)
    /// </summary>
    [Required]
    public string ConfigurationJson { get; set; } = string.Empty;

    /// <summary>
    /// User who created this template
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// When the template was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last user who modified the template
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// When the template was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether this template is shared across the company or private to creator
    /// </summary>
    public bool IsShared { get; set; } = false;
}
