using System.ComponentModel.DataAnnotations;

namespace Platform.Domain.Entities;

/// <summary>
/// Tracks import operations for audit trail and rollback support
/// </summary>
public class ImportHistory
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Target table that was imported into
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Tenant/Company ID
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// User who initiated the import
    /// </summary>
    public Guid ImportedBy { get; set; }

    /// <summary>
    /// When the import started
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the import completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Import status: Pending, InProgress, Completed, Failed, Cancelled, RolledBack
    /// </summary>
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Total number of rows in source file
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Successfully imported rows
    /// </summary>
    public int SuccessfulRows { get; set; }

    /// <summary>
    /// Failed rows
    /// </summary>
    public int FailedRows { get; set; }

    /// <summary>
    /// Source filename
    /// </summary>
    [MaxLength(500)]
    public string? FileName { get; set; }

    /// <summary>
    /// Serialized column mapping configuration (JSON)
    /// </summary>
    public string? MappingConfiguration { get; set; }

    /// <summary>
    /// Error messages if import failed
    /// </summary>
    public string? ErrorMessages { get; set; }

    /// <summary>
    /// When the import was rolled back
    /// </summary>
    public DateTime? RolledBackAt { get; set; }

    /// <summary>
    /// User who performed rollback
    /// </summary>
    public Guid? RolledBackBy { get; set; }

    /// <summary>
    /// Duration of import operation
    /// </summary>
    public TimeSpan? Duration { get; set; }
}
