namespace Platform.Domain.Entities;

/// <summary>
/// Tracks an individual batch within an import session
/// </summary>
public class ImportBatch
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Parent import log
    /// </summary>
    public Guid ImportLogId { get; set; }
    public ImportLog? ImportLog { get; set; }

    /// <summary>
    /// Batch number (1-based)
    /// </summary>
    public int BatchNumber { get; set; }

    /// <summary>
    /// Starting row index in the source data (0-based)
    /// </summary>
    public int StartRow { get; set; }

    /// <summary>
    /// Ending row index in the source data (exclusive)
    /// </summary>
    public int EndRow { get; set; }

    /// <summary>
    /// Number of rows successfully imported in this batch
    /// </summary>
    public int SuccessfulRows { get; set; }

    /// <summary>
    /// Number of rows that failed in this batch
    /// </summary>
    public int FailedRows { get; set; }

    /// <summary>
    /// Batch status
    /// </summary>
    public BatchStatus Status { get; set; } = BatchStatus.Pending;

    /// <summary>
    /// When this batch started processing
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When this batch completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Error messages for failed rows (JSON array)
    /// </summary>
    public string? ErrorMessages { get; set; }
}

/// <summary>
/// Batch processing status
/// </summary>
public enum BatchStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}
