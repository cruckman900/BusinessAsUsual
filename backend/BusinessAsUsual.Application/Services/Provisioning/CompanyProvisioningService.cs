using BusinessAsUsual.Domain.Entities;
using BusinessAsUsual.Domain.Entities.ProvisionedCompany;
using BusinessAsUsual.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Application.Services.Provisioning;

/// <summary>
/// Handles initial provisioning of a company and its default entities.
/// </summary>
public class CompanyProvisioningService
{
    private readonly BusinessDbContext _db;

    /// <summary>
    /// db context injection
    /// </summary>
    /// <param name="db"></param>
    public CompanyProvisioningService(BusinessDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Provisions a new company with default roles, users, settings, and permission groups.
    /// </summary>
    /// <param name="companyName">The name of the company to create.</param>
    /// <param name="description">Optional description of the company.</param>
    /// <returns>The provisioned Company entity.</returns>
    public async Task<Company> ProvisionCompanyAsync(string companyName, string description = "")
    {
        var company = new Company
        {
            CompanyID = Guid.NewGuid(),
            CompanyName = companyName,
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Companies.AddAsync(company);

        // ─────────────────────────────────────────────
        // Seed default roles
        // ─────────────────────────────────────────────
        var adminRole = new CompanyUserRole
        {
            RoleID = Guid.NewGuid(),
            CompanyID = company.CompanyID,
            RoleName = "Administrator",
            Description = "Full access to all company features",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _db.CompanyUserRoles.AddAsync(adminRole);

        // ─────────────────────────────────────────────
        // Seed default user
        // ─────────────────────────────────────────────
        var defaultUser = new CompanyUser
        {
            UserID = Guid.NewGuid(),
            CompanyID = company.CompanyID,
            UserName = "admin",
            Email = "admin@" + companyName.ToLower().Replace(" ", "") + ".com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _db.CompanyUsers.AddAsync(defaultUser);

        // ─────────────────────────────────────────────
        // Assign role to user
        // ─────────────────────────────────────────────
        var roleAssignment = new CompanyUserRoleAssignment
        {
            AssignmentID = Guid.NewGuid(),
            CompanyID = company.CompanyID,
            RoleID = adminRole.RoleID,
            UserID = defaultUser.UserID,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _db.CompanyUserRoleAssignments.AddAsync(roleAssignment);

        // ─────────────────────────────────────────────
        // Seed default settings
        // ─────────────────────────────────────────────
        await _db.CompanySettings.AddAsync(new CompanySetting
        {
            SettingID = Guid.NewGuid(),
            CompanyID = company.CompanyID,
            SettingKey = "Theme",
            SettingValue = "Default",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // ─────────────────────────────────────────────
        // Seed default permission group
        // ─────────────────────────────────────────────
        var defaultGroup = new CompanyUserRolePermissionGroup
        {
            PermissionGroupID = Guid.NewGuid(),
            CompanyID = company.CompanyID,
            GroupName = "Default Group",
            Description = "Default permission group",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _db.CompanyUserRolePermissionGroups.AddAsync(defaultGroup);

        // ─────────────────────────────────────────────
        // Assign group to role
        // ─────────────────────────────────────────────
        await _db.CompanyUserRolePermissionGroupAssignments.AddAsync(new CompanyUserRolePermissionGroupAssignment
        {
            AssignmentID = Guid.NewGuid(),
            RoleID = adminRole.RoleID,
            PermissionGroupID = defaultGroup.PermissionGroupID,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return company;
    }
}