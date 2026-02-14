using BusinessAsUsual.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Infrastructure.Data;

/// <summary>
/// DbContext for storing tenant metadata in the master BusinessAsUsual database.
/// </summary>
public class MasterDbContext : DbContext
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="options"></param>
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

    /// <summary>
    /// Collection of tenant metadata records
    /// </summary>
    public DbSet<TenantMetadata> Tenants { get; set; }
}