using Microsoft.Data.SqlClient;

namespace AI.Api.Services;

/// <summary>
/// Validates a company id against the provisioning master database ("BusinessAsUsual").
/// Self-contained (uses Microsoft.Data.SqlClient directly) so AI.Api stays decoupled from
/// the backend Infrastructure project while querying the same source of truth.
/// </summary>
public sealed class SqlCompanyDirectory : ICompanyDirectory
{
    private const string ConnectionEnvVar = "AWS_SQL_CONNECTION_STRING";
    private const string MasterCatalog = "BusinessAsUsual";

    private readonly ILogger<SqlCompanyDirectory> _logger;

    public SqlCompanyDirectory(ILogger<SqlCompanyDirectory> logger)
    {
        _logger = logger;
    }

    public async Task<CompanyRecord?> FindCompanyAsync(Guid companyId, CancellationToken ct = default)
    {
        if (companyId == Guid.Empty)
        {
            return null;
        }

        var raw = Environment.GetEnvironmentVariable(ConnectionEnvVar);
        if (string.IsNullOrWhiteSpace(raw))
        {
            // No DB configured (e.g. local demo-only run) => cannot validate => treat as not provisioned.
            _logger.LogWarning("{EnvVar} is not set; company validation is unavailable and paid tier is disabled.", ConnectionEnvVar);
            return null;
        }

        try
        {
            var connectionString = new SqlConnectionStringBuilder(raw)
            {
                InitialCatalog = MasterCatalog
            }.ConnectionString;

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(ct);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 Id, Name, BillingPlan FROM Companies WHERE Id = @Id";
            cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.UniqueIdentifier) { Value = companyId });

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (!await reader.ReadAsync(ct))
            {
                return null;
            }

            var id = reader.GetGuid(0);
            var name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var plan = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            return new CompanyRecord(id, name, plan);
        }
        catch (Exception ex)
        {
            // Fail closed: if we cannot verify the company, do not grant paid access.
            _logger.LogError(ex, "Failed to validate company {CompanyId} against master DB.", companyId);
            return null;
        }
    }
}
