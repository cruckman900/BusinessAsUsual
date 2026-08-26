namespace Platform.Application.DTOs.Import;

/// <summary>
/// Progress update from an import operation
/// </summary>
public class ImportProgress
{
    public Guid ImportLogId { get; set; }
    public int CurrentBatch { get; set; }
    public int TotalBatches { get; set; }
    public int ProcessedRows { get; set; }
    public int TotalRows { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CurrentMessage { get; set; }
    public double PercentComplete => TotalRows > 0 ? (double)ProcessedRows / TotalRows * 100 : 0;
}

/// <summary>
/// Result of an import operation
/// </summary>
public class ImportResult
{
    public Guid ImportLogId { get; set; }
    public bool Success { get; set; }
    public int TotalRows { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Error that occurred during import
/// </summary>
public class ImportError
{
    public int RowNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public Dictionary<string, string>? RowData { get; set; }
}
