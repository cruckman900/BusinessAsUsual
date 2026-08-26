namespace Platform.Application.DTOs.Import;

/// <summary>
/// Represents a mapping between source and target columns
/// </summary>
public class ColumnMappingDefinition
{
    public string SourceColumn { get; set; } = string.Empty;
    public string TargetColumn { get; set; } = string.Empty;
    public DataTransformation? Transformation { get; set; }
    public bool IsRequired { get; set; }
    public bool IsMapped => !string.IsNullOrEmpty(TargetColumn);
}

/// <summary>
/// Result of column mapping analysis
/// </summary>
public class ColumnMappingAnalysis
{
    public List<ColumnMappingDefinition> Mappings { get; set; } = new();
    public List<string> UnmappedSourceColumns { get; set; } = new();
    public List<string> UnmappedTargetColumns { get; set; } = new();
    public List<string> ValidationErrors { get; set; } = new();
    public double ConfidenceScore { get; set; } // 0-1, how confident the auto-mapping is
}

/// <summary>
/// Options for fuzzy column name matching
/// </summary>
public class ColumnMatchingOptions
{
    public bool IgnoreCase { get; set; } = true;
    public bool IgnoreUnderscores { get; set; } = true;
    public bool IgnoreSpaces { get; set; } = true;
    public int MinimumSimilarityScore { get; set; } = 80; // 0-100
}
