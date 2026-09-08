# Module Demo Seed-Data Pattern

## Why this exists

Several module APIs (`Sales.API`, `Inventory.API`, `Services.API`) run against an
in-memory EF Core database for local development and demos, and need to be pre-populated
with realistic "admin/password" demo-tenant data on startup.

Early on, this seed data was written directly inline in each module's `Program.cs` as a
large local function (in one case, ~350 lines of entity literals embedded in the host
bootstrap file). This caused two concrete problems:

1. **Readability/maintainability** — `Program.cs` should be a thin, scannable description
   of how the host is wired up (DI, middleware, endpoints). Burying hundreds of lines of
   demo fixtures in the same file made it hard to see what actually configures the app.
2. **Fragility** — because the file was so large, targeted edits to `Program.cs` (e.g.
   fixing a DI registration) had a much higher chance of accidentally corrupting or
   truncating the seed-data section, or vice versa.

## The pattern

Each module that needs demo seed data should have:

- A dedicated `Seeding/` folder under the module's API project
  (e.g. `services\Sales\Sales.API\Seeding\`, `services\Inventory\Inventory.API\Seeding\`).
- A single instantiable class named `{Module}Seeder` (e.g. `SalesSeeder`, `InventorySeeder`),
  matching the original reference implementation `Services.Infrastructure.Seeding.DataSeeder`.
- The seeder's constructor takes the module's `DbContext` (and any other dependency it
  genuinely needs, such as `IHttpClientFactory` for cross-module demo data fetches).
- A parameterless `public async Task SeedAsync()` method that is idempotent — it should
  check whether data already exists and return immediately if so, so it's safe to call on
  every startup.
- The seeder additionally implements `BusinessAsUsual.Infrastructure.SeedData.IModuleSeeder<TContext>`
  (an explicit interface implementation delegating to the parameterless `SeedAsync()`) purely
  for contract consistency and future testability — callers should keep using the
  parameterless `SeedAsync()` directly.
- Any demo tenant/company id used by the seed data should come from
  `BusinessAsUsual.Infrastructure.SeedData.DemoTenant.CompanyId` rather than a locally
  declared GUID literal, so cross-module demo data (Sales, Inventory, CRM, etc.) always
  lines up under the same "tenant" — this also matches the Web shell's default tenant
  context (see `TenantContextCircuitHandler.DefaultCompanyId`).

`Program.cs` should only ever reference the seeder type, e.g.:

```csharp
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
	var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
	await new SalesSeeder(context, httpClientFactory).SeedAsync();
}
```

and, for modules that expose a dev/demo-only `tenant-reset` endpoint, the same seeder is
reused after clearing the tables:

```csharp
app.MapPost("/api/sales/tenant-reset", async (SalesDbContext context, IHttpClientFactory httpClientFactory) =>
{
	// ...remove existing rows...
	await new SalesSeeder(context, httpClientFactory).SeedAsync();
	return Results.Ok(new { message = "Sales tenant database reset and reseeded." });
}).AllowAnonymousTenant();
```

## Current adopters

| Module        | Seeder class      | Location                              |
|---------------|--------------------|----------------------------------------|
| Services.API  | `DataSeeder`        | `Services.Infrastructure.Seeding`      |
| Sales.API     | `SalesSeeder`       | `Sales.API.Seeding`                    |
| Inventory.API | `InventorySeeder`   | `Inventory.API.Seeding`                |
| HR.API        | `HRSeeder`          | `HR.API.Seeding`                       |
| LMS.API       | `LMSSeedData`       | `LMS.Infrastructure.Data` (pre-existing, now actually invoked from `Program.cs`) |

CRM.API and Finance.API don't use EF Core persistence yet — their `Mock*Service`
classes and `FinanceDataStore` already carry inline in-memory demo data (leads,
opportunities, invoices, payments, etc.), so this pattern doesn't apply to them until
they move to real persistence. Platform.API seeds system Roles/Permissions via EF Core
`HasData` migrations (static reference data, not demo business data), so it also doesn't
need this pattern. AI.Api has no persistence and no demo dataset to seed.

## Adding this pattern to a new module

1. Create `services\{Module}\{Module}.API\Seeding\{Module}Seeder.cs`.
2. Implement the class per the shape described above, implementing
   `IModuleSeeder<{Module}DbContext>` explicitly.
3. Reference `BusinessAsUsual.Infrastructure.SeedData.DemoTenant.CompanyId` for the demo
   tenant id instead of a new GUID literal.
4. Update `Program.cs` to call `new {Module}Seeder(...).SeedAsync()` from the startup seed
   block and (if applicable) the tenant-reset endpoint — and nothing else related to seed
   data should live in `Program.cs`.
