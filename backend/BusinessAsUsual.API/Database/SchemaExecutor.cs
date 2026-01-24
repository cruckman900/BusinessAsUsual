namespace BusinessAsUsual.API.Database
{
    /// <summary>
    /// Executes SQL schema scripts against a SQL Server database.
    /// </summary>
    public class SchemaExecutor
    {
        /// <summary>
        /// Executes the provided SQL script against the specified connection string asynchronously.
        /// </summary>
        /// <param name="connectionString">The connection string to the target database.</param>
        /// <param name="script">The SQL script to execute.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteScriptAsync(string connectionString, string script)
        {
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = script;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}