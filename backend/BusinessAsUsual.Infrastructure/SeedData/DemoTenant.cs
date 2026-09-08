namespace BusinessAsUsual.Infrastructure.SeedData;

/// <summary>
/// Shared constants for the built-in "admin/password" demo tenant used by every module's
/// in-memory seed data. Centralizing this avoids each module API re-declaring its own copy of
/// the same GUID literal (a source of subtle seed-data drift across modules in the past).
/// </summary>
public static class DemoTenant
{
    /// <summary>
    /// Default demo tenant CompanyId, matching the Web shell's default tenant context
    /// (see TenantContextCircuitHandler.DefaultCompanyId) so cross-module demo data lines up.
    /// </summary>
    public static readonly Guid CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
}
