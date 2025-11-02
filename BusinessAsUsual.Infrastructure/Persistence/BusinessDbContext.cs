using BusinessAsUsual.Domain.Entities.Company;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BusinessAsUsual.Infrastructure.Persistence
{
    /// <summary>
    /// DbContext for company provisioning and business logic.
    /// </summary>
    public class BusinessDbContext : DbContext
    {
        public BusinessDbContext(DbContextOptions<BusinessDbContext> options)
            : base(options)
        {
        }

        // ────────────────────────────────
        // DbSets for Company Provisioning
        // ────────────────────────────────

        public DbSet<CompanyInfo> Companies { get; set; }
        public DbSet<CompanySetting> CompanySettings { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<CompanyUserSetting> CompanyUserSettings { get; set; }
        public DbSet<CompanyUserRole> CompanyUserRoles { get; set; }
        public DbSet<CompanyUserRoleSetting> CompanyUserRoleSettings { get; set; }
        public DbSet<CompanyUserRolePermission> CompanyUserRolePermissions { get; set; }
        public DbSet<CompanyUserRolePermissionSetting> CompanyUserRolePermissionSettings { get; set; }
        public DbSet<CompanyUserRolePermissionOverride> CompanyUserRolePermissionOverrides { get; set; }
        public DbSet<CompanyUserRolePermissionGroup> CompanyUserRolePermissionGroups { get; set; }
        public DbSet<CompanyUserRolePermissionGroupMember> CompanyUserRolePermissionGroupMembers { get; set; }
        public DbSet<CompanyUserRolePermissionGroupAssignment> CompanyUserRolePermissionGroupAssignments { get; set; }
        public DbSet<CompanyUserRolePermissionGroupAudit> CompanyUserRolePermissionGroupAudits { get; set; }
        public DbSet<CompanyUserRoleAssignment> CompanyUserRoleAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Fluent API configurations go here
        }
    }
}