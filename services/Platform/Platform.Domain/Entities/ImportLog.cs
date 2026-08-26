namespace Platform.Domain.Entities;

/// <summary>
/// Tracks a data import session for audit and rollback purposes
/// </summary>
public class ImportLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant/company identifier
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Target table/entity name (e.g., "Employee", "Department")
    /// </summary>
    public string TargetTable { get; set; } = string.Empty;

    /// <summary>
    /// User who performed the import
    /// </summary>
    public string ImportedBy { get; set; } = string.Empty;

    /// <summary>
    /// When the import started
    /// </summary>
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Total number of rows in the source file
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Number of rows successfully imported
    /// </summary>
    public int SuccessfulRows { get; set; }

    /// <summary>
    /// Number of rows that failed validation or import
    /// </summary>
    public int FailedRows { get; set; }

    /// <summary>
    /// Overall status of the import
    /// </summary>
    public ImportStatus Status { get; set; } = ImportStatus.InProgress;

    /// <summary>
    /// Source file name (if applicable)
    /// </summary>
    public string? SourceFileName { get; set; }

    /// <summary>
    /// JSON-serialized column mappings used for this import
    /// </summary>
    public string? ColumnMappingsJson { get; set; }

    /// <summary>
    /// Error messages or summary
    /// </summary>
    public string? ErrorSummary { get; set; }

    /// <summary>
    /// Individual batches that make up this import
    /// </summary>
    public ICollection<ImportBatch> Batches { get; set; } = new List<ImportBatch>();
}

/// <summary>
/// Import session status
/// </summary>
public enum ImportStatus
{
    InProgress,
    Completed,
    CompletedWithErrors,
    Failed,
    RolledBack
}
