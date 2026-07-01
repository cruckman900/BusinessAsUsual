using Microsoft.EntityFrameworkCore;
using ModuleRegistry.Domain.Entities;

namespace ModuleRegistry.Infrastructure.Persistence;

public class ModuleRegistryDbContext : DbContext
{
    public ModuleRegistryDbContext(DbContextOptions<ModuleRegistryDbContext> options)
        : base(options)
    {
    }

    public DbSet<ModuleMetadata> Modules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ModuleMetadata>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ModuleId).IsUnique();
            entity.Property(e => e.ModuleId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Version).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ApiBaseUrl).IsRequired().HasMaxLength(500);

            // Store NavigationItems as JSON
            entity.OwnsMany(e => e.NavigationItems, nav =>
            {
                nav.ToJson();
                nav.Property(n => n.Label).IsRequired();
                nav.Property(n => n.Route).IsRequired();
            });
        });
    }
}
