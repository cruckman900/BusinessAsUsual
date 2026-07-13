# BAU Module Blueprint

This document defines the **canonical pattern** every BusinessAsUsual (BAU) module follows so a new
module can be brought online quickly and consistently across the **web (Blazor)**, the **.NET service
API**, the **Module Registry**, and the **Android** contract-driven client.

**HR** is the reference implementation. **CRM** was the first module built to parity using this blueprint.
When in doubt, copy HR.

---

## 1. Guiding principles

1. **Contract-first.** The backend defines *what* to render (a JSON UI specification). The clients decide
   *how* to render it. Grow the backend contract first, then let each client map and render the new shape.
2. **One module = one vertical slice.** Domain → Application → Infrastructure → API → Contracts, plus Web
   and Android surfaces that consume the shared contract.
3. **Self-registration.** Every module registers itself with the Module Registry on startup and keeps a
   heartbeat so it survives a registry restart (the registry uses an in-memory store in development).
4. **Never show an empty flagship screen in a demo.** Live data first; fall back to representative sample
   rows when the data source is empty.

---

## 2. Project layout

```
services/<Module>/
  <Module>.Domain/          # Entities, enums, repository interfaces (no external deps)
  <Module>.Application/     # DTOs, service interfaces + implementations, ModuleRegistrationService
  <Module>.Infrastructure/  # EF Core DbContext, repositories (optional for early mock modules)
  <Module>.Contracts/       # Mobile UI contract types (Specifications + Navigation)  ← shared shape
  <Module>.API/             # ASP.NET Core host: controllers + hosted registration service
  <Module>.Web/             # Blazor UI (MudBlazor)
```

`<Module>.Contracts` is a **plain SDK-style .NET 9 class library** with `Nullable` and `ImplicitUsings`
enabled — no package references. It is referenced by `<Module>.API` (and can be shared with clients).

---

## 3. The mobile UI contract (`<Module>.Contracts`)

These types define the JSON the Android client consumes. Mirror them exactly (names may vary per module,
but shapes must match what the Android renderer expects).

| Type | Purpose |
| --- | --- |
| `MobileUISpecification` | Root object: `ModuleId`, `ModuleName`, `Version`, `Navigation`, `Screens` (dictionary of screenKey → spec). |
| `ModuleNavigationMap` / `NavigationItem` | Sidebar structure; each item maps a label to a `Screen` key + web `Route`. |
| `ListScreenSpec` | List screen: `Columns`, `Actions`, `Filters`, search + empty-state text. |
| `ColumnDefinition` | Column `Name` (data key), `Label`, `Type` (`text`/`date`/`badge`/`image`), `Width`, `Sortable`. |
| `ActionButton` | List-level (`Id="add"`) or per-row action (any other `Id`); `Action` = `navigate`/`api-call`; optional confirmation. |
| `FilterOption` / `FilterValue` | Optional select/checkbox/date-range filters. |
| `DetailScreenSpec` / `SectionDefinition` / `FieldDefinition` | Read-only detail screen grouped into sections. |
| `FormScreenSpec` / `FormSectionDefinition` / `FormFieldDefinition` / `SelectOption` / `ValidationRules` | Create/edit form with validation. |
| `ChartScreenSpec` / `ChartSpec` / `ChartSeries` / `ChartDataPoint` | Analytics screen; `ChartType` = `line`/`bar`/`pie`/`donut`/`sparkline`. |

**Screen kinds** (the `Type` discriminator on each spec):
`list` · `detail` · `form` · `chart`.

---

## 4. The mobile controller (`<Module>.API/Controllers/MobileUIController.cs`)

Route: **`[Route("api/<module>/mobile")]`** (e.g. `api/hr/mobile`, `api/crm/mobile`).

Required endpoints:

| Verb + Route | Returns |
| --- | --- |
| `GET ui-spec` | Full `MobileUISpecification` (navigation + all screen specs). |
| `GET navigation` | Just the `ModuleNavigationMap`. |
| `GET ui-spec/{screen}` | A single screen spec (optional convenience endpoints). |
| `GET data/{screenKey}` | `List<Dictionary<string,string>>` — rows keyed by column `Name`. |

**Conventions that keep controllers small:**

- `ListSpec(title, searchPlaceholder, addLabel, addRoute, emptyMessage, rowActions, columns...)` factory.
- `Col(name, label, type, width, sortable)` factory for columns.
- Row-action factories: `RowView`, `RowEdit`, `RowDelete`, plus domain actions (`RowApprove`/`RowReject`
  for HR, `RowConvert` for CRM). Any action whose `Id != "add"` is treated as a per-row overflow action.
- `StdRowActions(baseRoute, noun)` = View / Edit / Delete. Compose richer sets per screen as needed.
- Data rows come from the module's services (`GetAll...Async`) with a **sample-row fallback** when empty.

---

## 5. Self-registration with the Module Registry

Two pieces, mirrored from HR:

1. **`<Module>.Application/Services/ModuleRegistrationService.cs`** — builds a `RegisterModuleRequest`
   (`ModuleId`, `Key`, `DisplayName`, URLs, permissions, capabilities, `SupportsMobile = true`,
   `MobileUISpecUrl = {apiBaseUrl}/api/<module>/mobile/ui-spec`, navigation items) and POSTs it to
   `{registryUrl}/api/modules/register`. Config keys: `ModuleRegistry:Url`, `<Module>:ApiBaseUrl`,
   `<Module>:UiEntryPoint`.
2. **`<Module>.API/Services/ModuleRegistrationHostedService.cs`** — a `BackgroundService` that retries
   registration on startup (5s) until the registry accepts it, then re-registers on a heartbeat (30s).

Wire-up in `Program.cs`:

```csharp
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();
builder.Services.AddHostedService<<Module>.API.Services.ModuleRegistrationHostedService>();
builder.Services.AddHealthChecks();
...
app.MapHealthChecks("/health");
```

Register `<Module>.Contracts` as a project reference from `<Module>.API`.

---

## 6. Android discovery flow (client side)

1. Android reads the registry: `GET {registry}/api/modules/mobile` → list of modules with their
   `MobileUISpecUrl`.
2. For a selected module it fetches `.../api/<module>/mobile/ui-spec` → navigation + screen specs.
3. For each list screen it fetches `.../api/<module>/mobile/data/{screenKey}` → rows.
4. The dynamic renderer (`DynamicUi.kt`) branches on the screen `Type` (`list`/`detail`/`form`/`chart`),
   renders columns/fields, and resolves row actions into in-place navigation or API calls. Charts render
   via `ChartRenderers.kt`. Breadcrumbs show `Dashboard > Module > Screen`.

Because the client is contract-driven, **a new module usually requires no Android code** — it appears
automatically once its `ui-spec` is reachable and it is registered.

---

## 7. New-module checklist

- [ ] Create `Domain`, `Application`, (`Infrastructure`), `Contracts`, `API`, `Web` projects.
- [ ] Add `<Module>.Contracts` (Specifications + Navigation) mirroring this blueprint; add to the solution.
- [ ] Reference `<Module>.Contracts` from `<Module>.API`.
- [ ] Implement `MobileUIController` under `api/<module>/mobile` with `ui-spec`, `navigation`, `data/{screenKey}`.
- [ ] Define navigation items and one spec per screen (list/detail/form/chart) using the factory helpers.
- [ ] Wire live data rows from the module services with a sample-row fallback.
- [ ] Add `ModuleRegistrationService` (+ `RegisterModuleRequest` DTO) and the hosted registration service.
- [ ] Add HTTP client + hosted service + health checks in `Program.cs`; map `/health`.
- [ ] Build `<Module>.API`; verify `GET /api/<module>/mobile/ui-spec` returns 200.
- [ ] Start the module; confirm it appears in `GET {registry}/api/modules/mobile`.
- [ ] Launch Android; confirm the module and its screens render without any client changes.

---

## 8. Reference files

| Concern | HR (reference) | CRM (parity) |
| --- | --- | --- |
| Mobile controller | `services/HR/HR.API/Controllers/MobileUIController.cs` | `services/CRM/CRM.API/Controllers/MobileUIController.cs` |
| Contract types | `services/HR/HR.Contracts/**` | `services/CRM/CRM.Contracts/**` |
| Registration service | `services/HR/HR.Application/Services/ModuleRegistrationService.cs` | `services/CRM/CRM.Application/Services/ModuleRegistrationService.cs` |
| Hosted registration | `services/HR/HR.API/Services/ModuleRegistrationHostedService.cs` | `services/CRM/CRM.API/Services/ModuleRegistrationHostedService.cs` |
| Program wire-up | `services/HR/HR.API/Program.cs` | `services/CRM/CRM.API/Program.cs` |
| Android renderer | `ui/mobileui/DynamicUi.kt`, `ui/mobileui/ChartRenderers.kt` | *(shared — no per-module code)* |
