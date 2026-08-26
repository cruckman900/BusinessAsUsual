using Platform.Application.DTOs.Import;

namespace Platform.Application.Services;

/// <summary>
/// Service for intelligent column mapping between source and target schemas
/// </summary>
public interface IColumnMappingService
{
    /// <summary>
    /// Analyzes source columns and suggests mappings to target schema
    /// </summary>
    Task<ColumnMappingAnalysis> AnalyzeAndMapColumnsAsync(
        List<string> sourceColumns,
        TableSchema targetSchema,
        ColumnMatchingOptions? options = null);

    /// <summary>
    /// Validates that a set of column mappings is complete and correct
    /// </summary>
    Task<(bool IsValid, List<string> Errors)> ValidateMappingsAsync(
        ColumnMappingAnalysis mappings,
        TableSchema targetSchema);

    /// <summary>
    /// Calculates similarity score between two column names (0-100)
    /// </summary>
    int CalculateSimilarity(string source, string target, ColumnMatchingOptions options);

    /// <summary>
    /// Saves a mapping template for reuse
    /// </summary>
    Task SaveMappingTemplateAsync(string templateName, string tableName, ColumnMappingAnalysis mappings, string userId);

    /// <summary>
    /// Loads a previously saved mapping template
    /// </summary>
    Task<ColumnMappingAnalysis?> LoadMappingTemplateAsync(string templateName, string tableName);
}
