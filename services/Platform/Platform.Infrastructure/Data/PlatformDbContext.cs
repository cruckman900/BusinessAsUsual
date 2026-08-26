using Microsoft.EntityFrameworkCore;
using Platform.Domain.Entities;

namespace Platform.Infrastructure.Data;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<ImportHistory> ImportHistories => Set<ImportHistory>();
    public DbSet<MappingTemplate> MappingTemplates => Set<MappingTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Ignore(e => e.FullName); // Computed property
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Module).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
        });

        // UserRole junction configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RolePermission junction configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });

            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ImportHistory configuration
        modelBuilder.Entity<ImportHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CompanyId, e.StartedAt });
            entity.HasIndex(e => e.TableName);
            entity.Property(e => e.TableName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileName).HasMaxLength(500);
        });

        // MappingTemplate configuration
        modelBuilder.Entity<MappingTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CompanyId, e.TableName });
            entity.HasIndex(e => e.TemplateName);
            entity.Property(e => e.TableName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TemplateName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ConfigurationJson).IsRequired();
        });

        // Seed system roles
        SeedSystemRoles(modelBuilder);

        // Seed system permissions
        SeedSystemPermissions(modelBuilder);
    }

    private static void SeedSystemRoles(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var managerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var userRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                Name = "Administrator",
                Description = "Full system access",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = managerRoleId,
                Name = "Manager",
                Description = "Departmental management access",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = userRoleId,
                Name = "User",
                Description = "Standard user access",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    private static void SeedSystemPermissions(ModelBuilder modelBuilder)
    {
        var permissions = new List<Permission>
        {
            // User Management Permissions
            new Permission
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000001"),
                Name = "users.create",
                Description = "Create new users",
                Module = "Platform",
                Resource = "users",
                Action = "create",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000002"),
                Name = "users.read",
                Description = "View users",
                Module = "Platform",
                Resource = "users",
                Action = "read",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000003"),
                Name = "users.update",
                Description = "Update users",
                Module = "Platform",
                Resource = "users",
                Action = "update",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000004"),
                Name = "users.delete",
                Description = "Delete users",
                Module = "Platform",
                Resource = "users",
                Action = "delete",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            },
            // Role Management Permissions
            new Permission
            {
                Id = Guid.Parse("00000002-0000-0000-0000-000000000001"),
                Name = "roles.create",
                Description = "Create new roles",
                Module = "Platform",
                Resource = "roles",
                Action = "create",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("00000002-0000-0000-0000-000000000002"),
                Name = "roles.read",
                Description = "View roles",
                Module = "Platform",
                Resource = "roles",
                Action = "read",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("00000002-0000-0000-0000-000000000003"),
                Name = "roles.update",
                Description = "Update roles",
                Module = "Platform",
                Resource = "roles",
                Action = "update",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("00000002-0000-0000-0000-000000000004"),
                Name = "roles.delete",
                Description = "Delete roles",
                Module = "Platform",
                Resource = "roles",
                Action = "delete",
                IsSystemPermission = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<Permission>().HasData(permissions);
    }
}
