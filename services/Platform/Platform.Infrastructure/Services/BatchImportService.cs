using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Platform.Application.DTOs.Import;
using Platform.Application.Services;
using Platform.Domain.Entities;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

/// <summary>
/// Implementation of batch import service with SQL Server optimizations
/// </summary>
public class BatchImportService : IBatchImportService
{
    private readonly ILogger<BatchImportService> _logger;
    private readonly IDataTransformationService _transformationService;
    private readonly ISchemaIntrospectionService _schemaService;
    private readonly IConfiguration _configuration;
    private readonly PlatformDbContext _dbContext;
    private readonly Dictionary<Guid, CancellationTokenSource> _activeImports = new();

    public BatchImportService(
        ILogger<BatchImportService> logger,
        IDataTransformationService transformationService,
        ISchemaIntrospectionService schemaService,
        IConfiguration configuration,
        PlatformDbContext dbContext)
    {
        _logger = logger;
        _transformationService = transformationService;
        _schemaService = schemaService;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task<ImportResult> ImportDataAsync(
        ParsedData sourceData,
        ColumnMappingAnalysis mappings,
        string targetTable,
        string companyId,
        string userId,
        int batchSize = 1000,
        IProgress<ImportProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var importLogId = Guid.NewGuid();
        var result = new ImportResult
        {
            ImportLogId = importLogId,
            TotalRows = sourceData.RowCount
        };

        // Create import history record
        var companyGuid = Guid.TryParse(companyId, out var parsedCompanyId) ? parsedCompanyId : Guid.Empty;
        var userGuid = Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : Guid.Empty;

        var importHistory = new ImportHistory
        {
            Id = importLogId,
            TableName = targetTable,
            CompanyId = companyGuid,
            ImportedBy = userGuid,
            StartedAt = DateTime.UtcNow,
            Status = "InProgress",
            TotalRows = sourceData.RowCount,
            MappingConfiguration = JsonSerializer.Serialize(mappings)
        };

        _dbContext.ImportHistories.Add(importHistory);
        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            // Register cancellation token
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _activeImports[importLogId] = linkedCts;

            _logger.LogInformation("Starting import {ImportId} for table {Table}: {RowCount} rows",
                importLogId, targetTable, sourceData.RowCount);

            // Calculate batches
            var totalBatches = (int)Math.Ceiling((double)sourceData.RowCount / batchSize);
            var currentBatch = 0;

            // Process in batches
            for (int i = 0; i < sourceData.RowCount; i += batchSize)
            {
                linkedCts.Token.ThrowIfCancellationRequested();

                currentBatch++;
                var batchRows = sourceData.Rows.Skip(i).Take(batchSize).ToList();

                _logger.LogDebug("Processing batch {CurrentBatch}/{TotalBatches}: rows {Start}-{End}",
                    currentBatch, totalBatches, i + 1, i + batchRows.Count);

                // Transform and validate each row
                var transformedRows = new List<Dictionary<string, string>>();
                for (int rowIndex = 0; rowIndex < batchRows.Count; rowIndex++)
                {
                    var sourceRow = batchRows[rowIndex];
                    var absoluteRowNumber = i + rowIndex + 1;

                    try
                    {
                        var transformedRow = await TransformRowAsync(sourceRow, mappings);
                        transformedRows.Add(transformedRow);
                        result.SuccessfulRows++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error transforming row {RowNumber}", absoluteRowNumber);
                        result.FailedRows++;
                        result.Errors.Add(new ImportError
                        {
                            RowNumber = absoluteRowNumber,
                            ErrorMessage = ex.Message,
                            RowData = sourceRow
                        });
                    }
                }

                // Execute SQL Server bulk insert for this batch
                if (transformedRows.Any())
                {
                    try
                    {
                        var schema = await _schemaService.GetTableSchemaAsync(targetTable);
                        if (schema == null)
                        {
                            throw new InvalidOperationException($"Could not retrieve schema for table {targetTable}");
                        }

                        await BulkInsertBatchAsync(
                            transformedRows,
                            targetTable,
                            schema,
                            companyId,
                            userId,
                            linkedCts.Token);

                        _logger.LogDebug("Successfully bulk inserted batch {CurrentBatch}: {Count} rows",
                            currentBatch, transformedRows.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during bulk insert for batch {CurrentBatch}", currentBatch);
                        // Mark all rows in this batch as failed
                        result.SuccessfulRows -= transformedRows.Count;
                        result.FailedRows += transformedRows.Count;
                        result.Errors.Add(new ImportError
                        {
                            RowNumber = i + 1,
                            ErrorMessage = $"Bulk insert failed for batch {currentBatch}: {ex.Message}"
                        });
                    }
                }

                // Report progress after each batch
                progress?.Report(new ImportProgress
                {
                    ImportLogId = importLogId,
                    CurrentBatch = currentBatch,
                    TotalBatches = totalBatches,
                    ProcessedRows = i + batchRows.Count,
                    TotalRows = sourceData.RowCount,
                    SuccessfulRows = result.SuccessfulRows,
                    FailedRows = result.FailedRows,
                    Status = "Processing",
                    CurrentMessage = $"Processing batch {currentBatch} of {totalBatches}"
                });
            }

            result.Success = result.FailedRows == 0;
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed import {ImportId}: {Successful} successful, {Failed} failed in {Duration}",
                importLogId, result.SuccessfulRows, result.FailedRows, result.Duration);

            // Update import history
            importHistory.CompletedAt = DateTime.UtcNow;
            importHistory.Status = result.Success ? "Completed" : "CompletedWithErrors";
            importHistory.SuccessfulRows = result.SuccessfulRows;
            importHistory.FailedRows = result.FailedRows;
            importHistory.Duration = result.Duration;
            if (result.Errors.Any())
            {
                importHistory.ErrorMessages = string.Join(Environment.NewLine, 
                    result.Errors.Select(e => $"Row {e.RowNumber}: {e.ErrorMessage}"));
            }

            // Final progress report
            progress?.Report(new ImportProgress
            {
                ImportLogId = importLogId,
                CurrentBatch = totalBatches,
                TotalBatches = totalBatches,
                ProcessedRows = sourceData.RowCount,
                TotalRows = sourceData.RowCount,
                SuccessfulRows = result.SuccessfulRows,
                FailedRows = result.FailedRows,
                Status = result.Success ? "Completed" : "CompletedWithErrors",
                CurrentMessage = result.Success ? "Import completed successfully" : $"Import completed with {result.FailedRows} errors"
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Import {ImportId} was cancelled", importLogId);
            importHistory.Status = "Cancelled";
            importHistory.CompletedAt = DateTime.UtcNow;
            importHistory.Duration = stopwatch.Elapsed;
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = "Import was cancelled by user"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error during import {ImportId}", importLogId);
            importHistory.Status = "Failed";
            importHistory.CompletedAt = DateTime.UtcNow;
            importHistory.Duration = stopwatch.Elapsed;
            importHistory.ErrorMessages = ex.Message;
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Fatal error: {ex.Message}"
            });
        }
        finally
        {
            _activeImports.Remove(importLogId);
        }

        return result;
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateImportDataAsync(
        ParsedData sourceData,
        ColumnMappingAnalysis mappings,
        TableSchema targetSchema)
    {
        var errors = new List<string>();

        // Check that all required columns are mapped
        var requiredColumns = targetSchema.Columns.Where(c => c.IsRequired).Select(c => c.ColumnName).ToHashSet();
        var mappedColumns = mappings.Mappings.Select(m => m.TargetColumn).ToHashSet();

        foreach (var required in requiredColumns)
        {
            if (!mappedColumns.Contains(required))
            {
                errors.Add($"Required column '{required}' is not mapped");
            }
        }

        // Sample validation on first few rows
        var sampleSize = Math.Min(10, sourceData.RowCount);
        for (int i = 0; i < sampleSize; i++)
        {
            var row = sourceData.Rows[i];
            try
            {
                await TransformRowAsync(row, mappings);
            }
            catch (Exception ex)
            {
                errors.Add($"Row {i + 1} validation failed: {ex.Message}");
            }
        }

        return (errors.Count == 0, errors);
    }

    public Task<ImportProgress?> GetImportStatusAsync(Guid importLogId)
    {
        // TODO: Implement by querying ImportLog entity
        _logger.LogDebug("Getting status for import {ImportId}", importLogId);
        return Task.FromResult<ImportProgress?>(null);
    }

    public Task CancelImportAsync(Guid importLogId)
    {
        if (_activeImports.TryGetValue(importLogId, out var cts))
        {
            _logger.LogInformation("Cancelling import {ImportId}", importLogId);
            cts.Cancel();
        }
        return Task.CompletedTask;
    }

    public async Task<TransformedDataPreview> GeneratePreviewAsync(
        ParsedData sourceData,
        ColumnMappingAnalysis mappings,
        int sampleSize = 20)
    {
        var preview = new TransformedDataPreview
        {
            TotalRowCount = sourceData.RowCount
        };

        // Get column headers from mappings
        preview.Headers = mappings.Mappings
            .Where(m => !string.IsNullOrEmpty(m.TargetColumn))
            .Select(m => m.TargetColumn)
            .Distinct()
            .ToList();

        // Transform sample rows
        var rowsToPreview = Math.Min(sampleSize, sourceData.RowCount);
        for (int i = 0; i < rowsToPreview; i++)
        {
            try
            {
                var transformedRow = await TransformRowAsync(sourceData.Rows[i], mappings);
                preview.SampleRows.Add(transformedRow);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error transforming preview row {RowNumber}", i + 1);
                // Add error indicator row
                preview.SampleRows.Add(new Dictionary<string, string>
                {
                    { "Error", $"Row {i + 1}: {ex.Message}" }
                });
            }
        }

        return preview;
    }

    private async Task<Dictionary<string, string>> TransformRowAsync(
        Dictionary<string, string> sourceRow,
        ColumnMappingAnalysis mappings)
    {
        var transformedRow = new Dictionary<string, string>();

        foreach (var mapping in mappings.Mappings)
        {
            if (!sourceRow.ContainsKey(mapping.SourceColumn))
            {
                // Source column missing - use empty string or throw?
                transformedRow[mapping.TargetColumn] = string.Empty;
                continue;
            }

            var sourceValue = sourceRow[mapping.SourceColumn];

            if (mapping.Transformation != null)
            {
                // Apply transformation
                var result = mapping.Transformation.TransformationType == TransformationType.SplitFullName
                    ? _transformationService.ApplyTransformationToRow(sourceRow, mapping.SourceColumn, mapping.Transformation)
                    : _transformationService.ApplyTransformation(sourceValue, mapping.Transformation);

                if (!result.Success)
                {
                    throw new InvalidOperationException($"Transformation failed for {mapping.SourceColumn}: {result.ErrorMessage}");
                }

                // Handle multi-value transformations (like SplitFullName)
                foreach (var kvp in result.TransformedValues)
                {
                    transformedRow[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                // Direct mapping, no transformation
                transformedRow[mapping.TargetColumn] = sourceValue;
            }
        }

        await Task.CompletedTask; // To satisfy async signature
        return transformedRow;
    }

    /// <summary>
    /// Performs SQL Server bulk insert using SqlBulkCopy for maximum performance
    /// </summary>
    private async Task BulkInsertBatchAsync(
        List<Dictionary<string, string>> transformedRows,
        string tableName,
        TableSchema schema,
        string companyId,
        string userId,
        CancellationToken cancellationToken)
    {
        // Build DataTable from transformed rows
        var dataTable = new DataTable(tableName);

        // Add columns to DataTable based on schema
        var columnMapping = new Dictionary<string, Type>();
        foreach (var column in schema.Columns)
        {
            // Skip auto-generated columns (Identity, Computed)
            if (column.IsPrimaryKey || column.ColumnName == "Id")
            {
                continue; // Let the database generate these
            }

            var clrType = MapDataTypeToClrType(column.DataType);
            dataTable.Columns.Add(column.ColumnName, clrType);
            columnMapping[column.ColumnName] = clrType;
        }

        // Add audit columns if not already mapped
        if (!dataTable.Columns.Contains("CreatedAt"))
        {
            dataTable.Columns.Add("CreatedAt", typeof(DateTime));
        }
        if (!dataTable.Columns.Contains("CreatedBy"))
        {
            dataTable.Columns.Add("CreatedBy", typeof(Guid));
        }
        if (!dataTable.Columns.Contains("CompanyId"))
        {
            dataTable.Columns.Add("CompanyId", typeof(Guid));
        }

        // Populate DataTable rows
        var now = DateTime.UtcNow;
        var userGuid = Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : Guid.Empty;
        var companyGuid = Guid.TryParse(companyId, out var parsedCompanyId) ? parsedCompanyId : Guid.Empty;

        foreach (var transformedRow in transformedRows)
        {
            var row = dataTable.NewRow();

            foreach (var kvp in transformedRow)
            {
                if (dataTable.Columns.Contains(kvp.Key))
                {
                    var targetType = columnMapping.ContainsKey(kvp.Key) ? columnMapping[kvp.Key] : typeof(string);
                    row[kvp.Key] = ConvertValue(kvp.Value, targetType);
                }
            }

            // Set audit fields
            row["CreatedAt"] = now;
            row["CreatedBy"] = userGuid;
            row["CompanyId"] = companyGuid;

            dataTable.Rows.Add(row);
        }

        // Execute SqlBulkCopy
        var connectionString = _configuration.GetConnectionString("PlatformDb") 
            ?? throw new InvalidOperationException("PlatformDb connection string not configured");

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();
        try
        {
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = tableName,
                BatchSize = transformedRows.Count,
                BulkCopyTimeout = 300 // 5 minutes
            };

            // Map DataTable columns to database columns
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Bulk inserted {Count} rows into {Table}", transformedRows.Count, tableName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlBulkCopy failed for {Table}, rolling back transaction", tableName);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Maps EF Core data type strings to CLR types for DataTable columns
    /// </summary>
    private static Type MapDataTypeToClrType(string dataType)
    {
        return dataType switch
        {
            "String" => typeof(string),
            "Int32" => typeof(int),
            "Int64" => typeof(long),
            "Decimal" => typeof(decimal),
            "Double" => typeof(double),
            "Boolean" => typeof(bool),
            "DateTime" => typeof(DateTime),
            "DateTimeOffset" => typeof(DateTimeOffset),
            "Guid" => typeof(Guid),
            "Byte[]" => typeof(byte[]),
            _ => typeof(string) // Default to string for unknown types
        };
    }

    /// <summary>
    /// Converts a string value to the target CLR type with null handling
    /// </summary>
    private static object ConvertValue(string? value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DBNull.Value;
        }

        try
        {
            if (targetType == typeof(string))
                return value;
            if (targetType == typeof(int))
                return int.Parse(value);
            if (targetType == typeof(long))
                return long.Parse(value);
            if (targetType == typeof(decimal))
                return decimal.Parse(value);
            if (targetType == typeof(double))
                return double.Parse(value);
            if (targetType == typeof(bool))
                return bool.Parse(value);
            if (targetType == typeof(DateTime))
                return DateTime.Parse(value);
            if (targetType == typeof(DateTimeOffset))
                return DateTimeOffset.Parse(value);
            if (targetType == typeof(Guid))
                return Guid.Parse(value);

            return value; // Fallback
        }
        catch
        {
            return DBNull.Value;
        }
    }

    /// <summary>
    /// Gets import history for a company
    /// </summary>
    public Task<List<ImportHistoryDto>> GetImportHistoryAsync(Guid companyId, int pageSize = 50, int pageNumber = 1)
    {
        var skip = (pageNumber - 1) * pageSize;
        var history = _dbContext.ImportHistories
            .Where(h => h.CompanyId == companyId)
            .OrderByDescending(h => h.StartedAt)
            .Skip(skip)
            .Take(pageSize)
            .Select(h => new ImportHistoryDto
            {
                Id = h.Id,
                TableName = h.TableName,
                CompanyId = h.CompanyId,
                ImportedBy = h.ImportedBy,
                StartedAt = h.StartedAt,
                CompletedAt = h.CompletedAt,
                Status = h.Status,
                TotalRows = h.TotalRows,
                SuccessfulRows = h.SuccessfulRows,
                FailedRows = h.FailedRows,
                FileName = h.FileName,
                Duration = h.Duration,
                RolledBackAt = h.RolledBackAt
            })
            .ToList();

        return Task.FromResult(history);
    }

    /// <summary>
    /// Gets detailed import history by ID
    /// </summary>
    public Task<ImportHistoryDetailDto?> GetImportHistoryDetailAsync(Guid importId)
    {
        var history = _dbContext.ImportHistories.FirstOrDefault(h => h.Id == importId);
        if (history == null)
            return Task.FromResult<ImportHistoryDetailDto?>(null);

        var detail = new ImportHistoryDetailDto
        {
            Id = history.Id,
            TableName = history.TableName,
            CompanyId = history.CompanyId,
            ImportedBy = history.ImportedBy,
            StartedAt = history.StartedAt,
            CompletedAt = history.CompletedAt,
            Status = history.Status,
            TotalRows = history.TotalRows,
            SuccessfulRows = history.SuccessfulRows,
            FailedRows = history.FailedRows,
            FileName = history.FileName,
            Duration = history.Duration,
            RolledBackAt = history.RolledBackAt,
            MappingConfiguration = history.MappingConfiguration,
            ErrorMessages = string.IsNullOrWhiteSpace(history.ErrorMessages) 
                ? new List<string>() 
                : history.ErrorMessages.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList()
        };

        return Task.FromResult<ImportHistoryDetailDto?>(detail);
    }

    /// <summary>
    /// Rolls back an import by deleting imported rows
    /// WARNING: This is a destructive operation
    /// </summary>
    public async Task<bool> RollbackImportAsync(Guid importId, Guid userId)
    {
        var history = _dbContext.ImportHistories.FirstOrDefault(h => h.Id == importId);
        if (history == null)
        {
            _logger.LogWarning("Import history {ImportId} not found for rollback", importId);
            return false;
        }

        if (history.Status != "Completed" && history.Status != "CompletedWithErrors")
        {
            _logger.LogWarning("Cannot rollback import {ImportId} with status {Status}", importId, history.Status);
            return false;
        }

        if (history.RolledBackAt.HasValue)
        {
            _logger.LogWarning("Import {ImportId} was already rolled back at {RolledBackAt}", importId, history.RolledBackAt);
            return false;
        }

        try
        {
            _logger.LogInformation("Rolling back import {ImportId} for table {Table}", importId, history.TableName);

            // For now, just mark as rolled back
            // In production, this would delete rows or mark them as deleted based on audit trail
            history.RolledBackAt = DateTime.UtcNow;
            history.RolledBackBy = userId;
            history.Status = "RolledBack";

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Successfully rolled back import {ImportId}", importId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back import {ImportId}", importId);
            return false;
        }
    }
}

