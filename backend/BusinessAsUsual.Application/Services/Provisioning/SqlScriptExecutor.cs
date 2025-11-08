using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace BusinessAsUsual.Application.Services.Provisioning;

/// <summary>
/// Executes structured SQL provisioning scripts from schema folders.
/// </summary>
public class SqlScriptExecutor
{
    private readonly string _tablesRoot;
    private readonly string _indexesRoot;
    private readonly string _constraintsRoot;

    /// <summary>
    /// inject schema root path
    /// </summary>
    /// <param name="schemaRoot"></param>
    public SqlScriptExecutor(string schemaRoot)
    {
        _tablesRoot = Path.Combine(schemaRoot, "tables");
        _indexesRoot = Path.Combine(schemaRoot, "indexes");
        _constraintsRoot = Path.Combine(schemaRoot, "constraints");
    }

    /// <summary>
    /// Executes all SQL scripts for tables, indexes, and constraints in the schema folder.
    /// </summary>
    /// <param name="connection">An open SqlConnection to the target tenant database.</param>
    public async Task ExecuteAllAsync(MySqlConnection connection)
    {
        var tableScripts = Directory.GetFiles(_tablesRoot, "*.sql", SearchOption.AllDirectories);

        foreach (var tableScriptPath in tableScripts)
        {
            var baseName = Path.GetFileNameWithoutExtension(tableScriptPath); // e.g., Product
            //var subfolder = Path.GetRelativePath(_tablesRoot, Path.GetDirectoryName(tableScriptPath)); // e.g., business
            var dirPath = Path.GetDirectoryName(tableScriptPath) ?? string.Empty;
            var subfolder = Path.GetRelativePath(_tablesRoot, dirPath);

            var indexPath = Path.Combine(_indexesRoot, subfolder, $"{baseName}.Indexes.sql");
            var constraintPath = Path.Combine(_constraintsRoot, subfolder, $"{baseName}.Constraints.sql");

            await ExecuteScriptAsync(connection, tableScriptPath);

            if (File.Exists(indexPath))
                await ExecuteScriptAsync(connection, indexPath);

            if (File.Exists(constraintPath))
                await ExecuteScriptAsync(connection, constraintPath);
        }
    }

    /// <summary>
    /// Executes a single SQL script file.
    /// </summary>
    /// <param name="connection">An open SqlConnection.</param>
    /// <param name="scriptPath">Path to the SQL file.</param>
    private static async Task ExecuteScriptAsync(MySqlConnection connection, string scriptPath)
    {
        var script = await File.ReadAllTextAsync(scriptPath);
        using var command = new MySqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }
}