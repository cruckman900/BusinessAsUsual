namespace Platform.Application.DTOs.Import;

/// <summary>
/// Preview of transformed data showing sample rows before final import
/// </summary>
public class TransformedDataPreview
{
    /// <summary>
    /// Column headers in the transformed data
    /// </summary>
    public List<string> Headers { get; set; } = new();

    /// <summary>
    /// Sample rows of transformed data (limited to prevent overwhelming UI)
    /// </summary>
    public List<Dictionary<string, string>> SampleRows { get; set; } = new();

    /// <summary>
    /// Total number of rows that will be imported
    /// </summary>
    public int TotalRowCount { get; set; }

    /// <summary>
    /// Number of sample rows shown in preview
    /// </summary>
    public int SampleRowCount => SampleRows.Count;

    /// <summary>
    /// Whether there are more rows beyond the sample
    /// </summary>
    public bool HasMoreRows => TotalRowCount > SampleRowCount;
}
