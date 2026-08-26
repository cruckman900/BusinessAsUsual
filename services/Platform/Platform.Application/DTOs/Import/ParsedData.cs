namespace Platform.Application.DTOs.Import;

/// <summary>
/// Represents parsed data from an import file
/// </summary>
public class ParsedData
{
    /// <summary>
    /// Column headers from the file
    /// </summary>
    public List<string> Headers { get; set; } = new();

    /// <summary>
    /// Rows of data (each row is a dictionary of column name -> value)
    /// </summary>
    public List<Dictionary<string, string>> Rows { get; set; } = new();

    /// <summary>
    /// Total number of rows parsed
    /// </summary>
    public int RowCount => Rows.Count;

    /// <summary>
    /// Parse errors encountered
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Original file name
    /// </summary>
    public string? FileName { get; set; }
}

/// <summary>
/// Supported file formats for import
/// </summary>
public enum ImportFileType
{
    Csv,
    Excel,
    TabDelimited,
    PipeSeparated
}
