using Platform.Application.DTOs.Import;

namespace Platform.Application.Services;

/// <summary>
/// Service for introspecting database schemas
/// </summary>
public interface ISchemaIntrospectionService
{
    /// <summary>
    /// Gets a list of all importable tables
    /// </summary>
    Task<List<string>> GetImportableTablesAsync();

    /// <summary>
    /// Gets the schema for a specific table
    /// </summary>
    Task<TableSchema?> GetTableSchemaAsync(string tableName);

    /// <summary>
    /// Gets schemas for multiple tables
    /// </summary>
    Task<Dictionary<string, TableSchema>> GetTableSchemasAsync(IEnumerable<string> tableNames);
}
