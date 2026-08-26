using Platform.Application.DTOs.Import;

namespace Platform.Application.Services;

/// <summary>
/// Service for executing batch imports with progress tracking
/// </summary>
public interface IBatchImportService
{
    /// <summary>
    /// Imports data with batch processing and progress callbacks
    /// </summary>
    Task<ImportResult> ImportDataAsync(
        ParsedData sourceData,
        ColumnMappingAnalysis mappings,
        string targetTable,
        string companyId,
        string userId,
        int batchSize = 1000,
        IProgress<ImportProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates import data before execution
    /// </summary>
    Task<(bool IsValid, List<string> Errors)> ValidateImportDataAsync(
        ParsedData sourceData,
        ColumnMappingAnalysis mappings,
        TableSchema targetSchema);

    /// <summary>
    /// Gets the status of an ongoing or completed import
    /// </summary>
    Task<ImportProgress?> GetImportStatusAsync(Guid importLogId);

    /// <summary>
    /// Cancels a running import
    /// </summary>
    Task CancelImportAsync(Guid importLogId);

    /// <summary>
    /// Generates a preview of transformed data showing sample rows
    /// </summary>
    Task<TransformedDataPreview> GeneratePreviewAsync(
        ParsedData sourceData,
        ColumnMappingAnalysis mappings,
        int sampleSize = 20);

    /// <summary>
    /// Gets import history for a company
    /// </summary>
    Task<List<ImportHistoryDto>> GetImportHistoryAsync(Guid companyId, int pageSize = 50, int pageNumber = 1);

    /// <summary>
    /// Gets detailed import history by ID
    /// </summary>
    Task<ImportHistoryDetailDto?> GetImportHistoryDetailAsync(Guid importId);

    /// <summary>
    /// Rolls back an import by marking all imported rows as deleted
    /// </summary>
    Task<bool> RollbackImportAsync(Guid importId, Guid userId);
}
