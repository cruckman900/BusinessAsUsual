using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Linq;

namespace BusinessAsUsual.Admin.Services;
/// <summary>
/// 
/// </summary>
public class SchemaExecutor
{
    /// <summary>
    /// Runs the provided sql script on the provided database.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="script"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "<Pending>")]
    public async Task ExecuteScriptAsync(string connectionString, string script)
    {
        await Task.Run(() =>
        {
            var sqlConn = new SqlConnection(connectionString);
            var serverConn = new ServerConnection(sqlConn);
            var server = new Server(serverConn);

            var sqlScript = new StringCollection();
            sqlScript.Add(script);

            Console.WriteLine("🚀 Executing schema script via SMO...");

            var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (var batch in batches.Where(batch => !string.IsNullOrWhiteSpace(batch)))
            {
                server.ConnectionContext.ExecuteNonQuery(batch);
                Console.WriteLine($"✅ Executed batch: {batch.Substring(0, Math.Min(80, batch.Length))}...");
            }

            Console.WriteLine("✅ Schema execution complete.");
        });
    }
}