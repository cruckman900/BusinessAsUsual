using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.Application.Services.Provisioning;

/// <summary>
/// Executes structured SQL provisioning scripts from schema folders.
/// </summary>
public class SqlScriptExecutor
{
    private readonly string _schemaRoot;

    public SqlScriptExecutor(string schemaRoot)
    {
        _schemaRoot = schemaRoot;
    }

    /// <summary>
    /// Executes all SQL scripts for tables, indexes, and constraints in the schema folder.
    /// </summary>
    /// <param name="connection">An open SqlConnection to the target tenant database.</param>
    public async Task ExecuteAllAsync(SqlConnection connection)
    {
        var tableScripts = Directory.GetFiles(Path.Combine(_schemaRoot, "tables"), "*.sql", SearchOption.AllDirectories);

        foreach (var tableScriptPath in tableScripts)
        {
            var fileName = Path.GetFileNameWithoutExtension(tableScriptPath); // e.g., CompanySettings
            var indexScriptPath = Path.Combine(_schemaRoot, "indexes", $"{fileName}.Indexes.sql");
            var constraintScriptPath = Path.Combine(_schemaRoot, "constraints", $"{fileName}.Constraints.sql");

            await ExecuteScriptAsync(connection, tableScriptPath);

            if (File.Exists(indexScriptPath))
                await ExecuteScriptAsync(connection, indexScriptPath);

            if (File.Exists(constraintScriptPath))
                await ExecuteScriptAsync(connection, constraintScriptPath);
        }
    }

    /// <summary>
    /// Executes a single SQL script file.
    /// </summary>
    /// <param name="connection">An open SqlConnection.</param>
    /// <param name="scriptPath">Path to the SQL file.</param>
    private async Task ExecuteScriptAsync(SqlConnection connection, string scriptPath)
    {
        var script = await File.ReadAllTextAsync(scriptPath);
        using var command = new SqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }
}