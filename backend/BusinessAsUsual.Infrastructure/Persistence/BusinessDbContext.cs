using BusinessAsUsual.Domain.Entities;
using BusinessAsUsual.Domain.Entities.ProvisionedCompany;
using Microsoft.EntityFrameworkCore;
using CompanyInfo = BusinessAsUsual.Domain.Entities.ProvisionedCompany.CompanyInfo;

namespace BusinessAsUsual.Infrastructure.Persistence
{
    /// <summary>
    /// Represents the EF Core context for company provisioning and business logic.
    /// </summary>
    public class BusinessDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public BusinessDbContext(DbContextOptions<BusinessDbContext> options)
            : base(options)
        {
        }

        // ────────────────────────────────
        // DbSets for Company Provisioning
        // ────────────────────────────────

        /// <summary>Represents the companies registered in the system.</summary>
        public DbSet<Company> Companies { get; set; }

        /// <summary>Represents the companies registered in the system.</summary>
        public DbSet<CompanyInfo> CompanyInfo { get; set; }

        /// <summary>Represents global settings for each company.</summary>
        public DbSet<CompanySetting> CompanySettings { get; set; }

        /// <summary>Represents users associated with a company.</summary>
        public DbSet<CompanyUser> CompanyUsers { get; set; }

        /// <summary>Represents user-specific settings within a company.</summary>
        public DbSet<CompanyUserSetting> CompanyUserSettings { get; set; }

        /// <summary>Represents roles defined within a company.</summary>
        public DbSet<CompanyUserRole> CompanyUserRoles { get; set; }

        /// <summary>Represents settings tied to specific roles.</summary>
        public DbSet<CompanyUserRoleSetting> CompanyUserRoleSettings { get; set; }

        /// <summary>Represents permissions assigned to roles.</summary>
        public DbSet<CompanyUserRolePermission> CompanyUserRolePermissions { get; set; }

        /// <summary>Represents settings tied to specific permissions.</summary>
        public DbSet<CompanyUserRolePermissionSetting> CompanyUserRolePermissionSettings { get; set; }

        /// <summary>Represents permission overrides for specific users or roles.</summary>
        public DbSet<CompanyUserRolePermissionOverride> CompanyUserRolePermissionOverrides { get; set; }

        /// <summary>Represents reusable permission groups.</summary>
        public DbSet<CompanyUserRolePermissionGroup> CompanyUserRolePermissionGroups { get; set; }

        /// <summary>Represents individual permissions included in a group.</summary>
        public DbSet<CompanyUserRolePermissionGroupMember> CompanyUserRolePermissionGroupMembers { get; set; }

        /// <summary>Represents assignments of permission groups to roles.</summary>
        public DbSet<CompanyUserRolePermissionGroupAssignment> CompanyUserRolePermissionGroupAssignments { get; set; }

        /// <summary>Represents audit logs for permission group changes.</summary>
        public DbSet<CompanyUserRolePermissionGroupAudit> CompanyUserRolePermissionGroupAudits { get; set; }

        /// <summary>Represents assignments of roles to users.</summary>
        public DbSet<CompanyUserRoleAssignment> CompanyUserRoleAssignments { get; set; }

        /// <summary>
        /// onModelCreating override to configure the model.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─────────────────────────────────────────────
            // Disable cascade deletes across all relationships
            // ─────────────────────────────────────────────
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}