using BusinessAsUsual.Domain.Entities;
using BusinessAsUsual.Infrastructure.Data;
using BusinessAsUsual.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessAsUsual.Application.Services.Provisioning;

/// <summary>
/// Handles creation of per-tenant databases and delegates company provisioning.
/// </summary>
public class TenantProvisioningService
{
    private readonly IConfiguration _config;
    private readonly MasterDbContext _masterDbContext;

    /// <summary>
    /// inject configuration
    /// </summary>
    /// <param name="config"></param>
    public TenantProvisioningService(IConfiguration config, MasterDbContext masterDbContext)
    {
        _config = config;
        _masterDbContext = masterDbContext;
    }

    /// <summary>
    /// Provisions a new tenant database and seeds default company data.
    /// </summary>
    /// <param name="companyName">The name of the company to provision.</param>
    /// <param name="description">Optional description of the company.</param>
    public async Task ProvisionTenantAsync(string companyName, string description = "")
    {
        // ─────────────────────────────────────────────
        // Step 1: Create tenant database
        // ─────────────────────────────────────────────
        var dbName = $"bau_{companyName.ToLower().Replace(" ", "_")}";
        var masterConnectionString = _config.GetConnectionString("MasterDb");

        using var masterConnection = new SqlConnection(masterConnectionString);
        await masterConnection.OpenAsync();

        var createDbSql = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}]";
        using var createCommand = new SqlCommand(createDbSql, masterConnection);
        await createCommand.ExecuteNonQueryAsync();

        // ─────────────────────────────────────────────
        // Step 2: Apply migrations to tenant database
        // ─────────────────────────────────────────────
        var builder = new SqlConnectionStringBuilder(masterConnectionString)
        {
            InitialCatalog = dbName
        };

        var tenantConnectionString = builder.ConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<BusinessDbContext>();
        optionsBuilder.UseSqlServer(tenantConnectionString);

        using var tenantDb = new BusinessDbContext(optionsBuilder.Options);
        await tenantDb.Database.MigrateAsync();

        // ─────────────────────────────────────────────
        // Step 3: Create schema objects in tenant database
        // ─────────────────────────────────────────────
        using var sqlConnection = new SqlConnection(tenantConnectionString);
        await sqlConnection.OpenAsync();

        var schemaPath = Path.Combine(AppContext.BaseDirectory, "schema");
        var executor = new SqlScriptExecutor(schemaPath);
        await executor.ExecuteAllAsync(sqlConnection);

        // ─────────────────────────────────────────────
        // Step 4: Seed default company data
        // ─────────────────────────────────────────────
        var provisioningService = new CompanyProvisioningService(tenantDb);
        await provisioningService.ProvisionCompanyAsync(companyName, description);

        // ─────────────────────────────────────────────
        // Step 5: (Optional) Store tenant metadata
        // ─────────────────────────────────────────────
        var metadata = new TenantMetadata
        {
            TenantMetadataID = Guid.NewGuid(),
            CompanyName = companyName,
            DatabaseName = dbName,
            ConnectionString = tenantConnectionString,
            CreatedAt = DateTime.UtcNow
        };

        await _masterDbContext.Tenants.AddAsync(metadata);
        await _masterDbContext.SaveChangesAsync();
    }
}