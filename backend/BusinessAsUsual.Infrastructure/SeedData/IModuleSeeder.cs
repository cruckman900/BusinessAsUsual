namespace BusinessAsUsual.Infrastructure.SeedData;

/// <summary>
/// Common contract for a module's dev/demo-only in-memory database seeder.
/// <para>
/// Pattern: each module API (Sales.API, Inventory.API, CRM.API, HR.API, ...) should implement
/// this in its own <c>SeedData/{Module}SeedData.cs</c> static or instance class, keeping all
/// fixture/dummy data out of <c>Program.cs</c>. <c>Program.cs</c> should only reference the
/// seeder type and call <see cref="SeedAsync"/> during startup and from the module's
/// <c>/api/{module}/tenant-reset</c> demo endpoint.
/// </para>
/// </summary>
/// <typeparam name="TContext">The module's EF Core DbContext type.</typeparam>
public interface IModuleSeeder<in TContext>
{
    /// <summary>
    /// Seeds the given context with demo data for <see cref="DemoTenant.CompanyId"/>.
    /// Implementations must be idempotent (no-op if data already exists) so this can safely
    /// run on every startup.
    /// </summary>
    Task SeedAsync(TContext context);
}
