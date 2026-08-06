using Microsoft.EntityFrameworkCore;
using Services.Domain.Entities;

namespace Services.Infrastructure.Data;

public class ServicesDbContext : DbContext
{
    public ServicesDbContext(DbContextOptions<ServicesDbContext> options) : base(options)
    {
    }

    public DbSet<Service> Services { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Service>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
        });
    }
}
