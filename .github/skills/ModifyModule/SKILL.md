# ModifyModule Skill

## Objective

### Main Module Dashboard
Transform the basic module dashboard into a comprehensive, user-friendly hub with the following structure (organized as rows):

1. **(Optional) Small Insight Cards**
   - Quick glanceable metrics at the top
   - Example: Total active items, pending actions, recent activity count
   - Only include if the module has meaningful real-time metrics

2. **Submodule Cards**
   - Primary navigation to major functional areas
   - Each card represents a key capability (e.g., Service Catalog, Appointments, Providers)
   - Cards should have icons, titles, descriptions, and clear CTAs

3. **(Optional) Quick Stats**
   - Deeper analytics or trends
   - Example: Charts, graphs, or comparative data
   - Only include if the module benefits from visual data representation

4. **Quick Actions + Notifications and Alerts**
   - Common actions users need immediate access to
   - System notifications, warnings, or important alerts
   - Keep actions contextual to the module's purpose

5. **About [Module] Module + Module Info**
   - Brief description of the module's purpose
   - Key features, version info, or helpful links
   - Module-specific configuration or settings access

### Submodule Dashboard
*(To be defined based on specific module needs)*

### Reference Modules
Use **Finance** and **Inventory** module dashboards as the gold standard for understanding the big picture of what a module dashboard should be like.

---

## Prerequisites

- **Required**: Module must already exist and be created using the `.github/skills/CreateModule/SKILL.md` playbook
- Module should be registered in BOTH module catalogs (see Dual-Catalog Maintenance below)
- Basic CRUD pages should be functional
- Module should follow the established project structure (API, Web, Application, Domain, Infrastructure, Contracts, Tests)

### Dual-Catalog Maintenance (CRITICAL)
When modifying a module's structure or navigation, update BOTH catalogs:

1. ✅ **ModuleCatalog.cs** (`BusinessAsUsual.Core/Modules/ModuleCatalog.cs`) - Update submodules if new areas added
2. ✅ **GetFallbackModules()** (`frontend/BusinessAsUsual.Web/Services/ModuleDiscoveryService.cs`) - Update NavigationItems
3. ✅ **Reference**: See `docs/MODULE_CATALOG_UNIFIED_REFERENCE.md` for protocol

---

## Steps

### Step 1: Analyze Reference Dashboards
- Examine Finance module dashboard structure
- Examine Inventory module dashboard structure
- Identify common patterns and layout conventions

### Step 2: Design Module-Specific Dashboard Layout
- Determine which optional components (insight cards, quick stats) make sense for the module
- Map out submodule cards and their navigation targets
- Identify key quick actions relevant to the module

### Step 3: Implement Dashboard Components
- Create insight card components (if applicable)
- Build submodule navigation cards
- Implement quick stats visualizations (if applicable)
- Add quick actions section
- Create notifications/alerts area
- Build About module section

### Step 4: Wire Up Navigation and Data
- Connect submodule cards to their respective pages
- Hook up API calls for dynamic data (stats, notifications)
- Implement quick action handlers

### Step 5: Style and Polish
- Ensure consistency with HR module styling standards
- Apply MudBlazor theming and layout providers
- Add responsive design considerations
- Test breadcrumb navigation

### Step 6: Test and Validate
- Verify all navigation links work
- Confirm data loads correctly
- Test responsive behavior
- Validate against reference modules (Finance, Inventory)

---

## Key Patterns

### Layout Conventions
- Use MudContainer for overall page structure
- Wrap sections in MudPaper for visual grouping
- Apply consistent spacing and grid layouts
- Use breadcrumbs for navigation context
- **CRITICAL: All layout pieces must follow a UNIFORM PATTERN across modules**
  - Breadcrumb structure: identical format
  - Page title format: identical (icon + text styling)
  - Card layouts: identical grid structure, spacing, and flex behavior
  - Button placement and styling: identical
  - "About [Module]" section: identical structure and positioning
  - Module info sections: identical format
  - Use Finance and Inventory modules as the exact reference templates
  - Copy spacing, classes, and structure EXACTLY - consistency is paramount

### Submodule Card Structure (CRITICAL - Follow Finance Pattern Exactly)
**Grid Layout:**
```razor
<MudGrid Spacing="3" Class="mb-6">
    <MudItem xs="12" sm="6" md="4" lg="3">
```

**Card Structure for Active/Clickable Cards:**
```razor
<MudCard Class="mud-card-hover d-flex flex-column" Style="height: 100%;">
    <MudCardContent Class="text-center pa-6 flex-grow-1">
        <MudIcon Icon="@Icons.Material.Filled.IconName" Size="Size.Large" Color="Color.Primary" Class="mb-4" />
        <MudText Typo="Typo.h6" GutterBottom="true">Card Title</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary" Class="mb-2">
            Description text
        </MudText>
        <MudChip T="string" Size="Size.Small" Color="Color.Primary" Variant="Variant.Text">Metric/Status</MudChip>
    </MudCardContent>
    <MudCardActions>
        <MudButton Variant="Variant.Text" Color="Color.Primary" FullWidth="true" Href="/module/route">
            Action Text →
        </MudButton>
    </MudCardActions>
</MudCard>
```

**Card Structure for Coming Soon/Disabled Cards:**
```razor
<MudCard Class="d-flex flex-column" Style="height: 100%;" Elevation="1">
    <MudCardContent Class="text-center pa-6 flex-grow-1">
        <MudIcon Icon="@Icons.Material.Filled.IconName" Size="Size.Large" Color="Color.Default" Class="mb-4" />
        <MudText Typo="Typo.h6" GutterBottom="true" Color="Color.Secondary">Card Title</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary" Class="mb-2">
            Description text
        </MudText>
        <MudChip T="string" Size="Size.Small" Color="Color.Default" Variant="Variant.Text">Coming Soon</MudChip>
    </MudCardContent>
    <MudCardActions>
        <MudButton Variant="Variant.Text" Color="Color.Default" FullWidth="true" Disabled="true">
            Coming Soon
        </MudButton>
    </MudCardActions>
</MudCard>
```

**Key Requirements:**
- ✅ **Center-aligned text** (`text-center`)
- ✅ **Icon → Title → Description → Chip** vertical flow
- ✅ **Separate MudCardContent and MudCardActions** (don't put button in content)
- ✅ **`pa-6`** padding on CardContent
- ✅ **`flex-grow-1`** on CardContent for equal card heights
- ✅ **`d-flex flex-column`** on MudCard
- ✅ **`Style="height: 100%;"`** on MudCard
- ✅ **`FullWidth="true"`** on action buttons
- ✅ **Arrow `→`** in button text for active cards
- ✅ **`mud-card-hover`** class for active cards
- ✅ **`Elevation="1"`** for disabled cards (no hover)
- ✅ **Grid: `xs="12" sm="6" md="4" lg="3"`** (4 columns on large screens)
- ✅ **Section Heading:** Add `<MudItem xs="12">` with `<MudText Typo="Typo.h5">` heading before cards
- ✅ **Spacing:** Use `Class="mt-6"` on MudGrid to separate from insight cards above
- ✅ **Varied Icon Colors:** Use meaningful colors for visual differentiation (see Icon Color Guidelines below)

**Complete Submodule Section Template:**
```razor
<!-- Submodule Navigation Cards -->
<MudGrid Spacing="3" Class="mb-6 mt-6">
    <MudItem xs="12">
        <MudText Typo="Typo.h5" Class="mb-4">[Module Name] Management</MudText>
    </MudItem>

    <!-- Then your submodule cards here -->
</MudGrid>
```

### Icon Color Guidelines

**Purpose:** Icon colors should provide visual variety and semantic meaning to help users quickly identify and differentiate submodule cards.

**Color Palette & Usage:**

| Color | MudBlazor Value | Use Cases | Examples |
|-------|----------------|-----------|----------|
| **Primary** (Blue) | `Color.Primary` | Core/main features, primary workflows | Product catalogs, main dashboards, core lists |
| **Success** (Green) | `Color.Success` | Positive actions, relationships, growth | Payments received, vendor/supplier management, approvals |
| **Info** (Cyan/Teal) | `Color.Info` | Infrastructure, locations, configuration | Warehouses, facilities, settings, system info |
| **Warning** (Amber) | `Color.Warning` | Monitoring, alerts, attention needed | Stock levels, pending items, overdue tracking |
| **Tertiary** (Purple) | `Color.Tertiary` | Secondary workflows, specialized operations | Purchase orders, scheduling, payroll |
| **Secondary** (Dark) | `Color.Secondary` | Analytics, reporting, utilities | Reports, dashboards, statistics |
| **Default** (Gray) | `Color.Default` | Coming soon features, disabled items | Placeholder cards, future features |
| **Error** (Red) | `Color.Error` | Critical alerts only (use sparingly) | System errors, critical warnings |

**Best Practices:**
- ✅ **Don't use all blue** - Vary colors across cards for visual interest
- ✅ **Assign colors semantically** - Match color meaning to feature purpose
- ✅ **Balance the palette** - Distribute colors evenly across the dashboard
- ✅ **Keep it professional** - Stick to MudBlazor's theme colors (no custom/random colors)
- ✅ **Use Default for "Coming Soon"** - Gray indicates inactive/future features
- ✅ **Limit Error usage** - Red should be reserved for truly critical items

**Example Application (Inventory Module):**
```razor
<!-- Products: Core catalog (Primary/Blue) -->
<MudIcon Icon="@Icons.Material.Filled.Inventory" Size="Size.Large" Color="Color.Primary" Class="mb-4" />

<!-- Warehouses: Infrastructure (Info/Cyan) -->
<MudIcon Icon="@Icons.Material.Filled.Warehouse" Size="Size.Large" Color="Color.Info" Class="mb-4" />

<!-- Stock Management: Monitoring/Alerts (Warning/Amber) -->
<MudIcon Icon="@Icons.Material.Filled.Inventory2" Size="Size.Large" Color="Color.Warning" Class="mb-4" />

<!-- Purchase Orders: Secondary workflow (Tertiary/Purple) -->
<MudIcon Icon="@Icons.Material.Filled.ShoppingCart" Size="Size.Large" Color="Color.Tertiary" Class="mb-4" />

<!-- Suppliers: Relationships/Growth (Success/Green) -->
<MudIcon Icon="@Icons.Material.Filled.Business" Size="Size.Large" Color="Color.Success" Class="mb-4" />

<!-- Reports: Analytics (Secondary/Dark) -->
<MudIcon Icon="@Icons.Material.Filled.Assessment" Size="Size.Large" Color="Color.Secondary" Class="mb-4" />
```

### Component Structure
- Follow HR module as the gold standard for component organization
- Use MudBlazor components (MudCard, MudButton, MudIcon, etc.)
- Keep components modular and reusable
- Maintain separation of concerns (presentation vs. logic)

### Data Integration
- **CRITICAL:** Use named HttpClient for API calls (via IHttpClientFactory)
- **NEVER inject bare HttpClient** in Blazor components (causes DI errors)
- Pattern: `var client = HttpClientFactory.CreateClient("{ModuleName}Api");`
- Implement loading states with MudProgressCircular
- Add error handling with ISnackbar for user feedback
- Cache data appropriately to avoid excessive API calls
- Use 2-second timeout with CancellationTokenSource for fast fallback to mock data

### Navigation
- All navigation should integrate with the shell's routing
- Use NavigationManager for programmatic navigation
- Ensure routes are registered in the module's routing table
- Maintain consistent route naming conventions

---

## Common Issues

### HttpClient / DI Errors (MOST COMMON)

#### Issue: "Cannot resolve service for type 'System.Net.Http.HttpClient'"
**Cause:** Trying to inject bare `HttpClient` in a Blazor Server component  
**Fix:**
```csharp
// ❌ WRONG:
@inject HttpClient Http

// ✅ CORRECT:
@inject IHttpClientFactory HttpClientFactory

protected override async Task OnInitializedAsync()
{
    var client = HttpClientFactory.CreateClient("YourModuleApi");
    var data = await client.GetFromJsonAsync<List<YourDto>>("api/yourmodule");
}
```

#### Issue: Named client returns 404 or connection refused
**Cause:** Shell's Program.cs missing HttpClient registration OR wrong port  
**Fix:**
1. Check `frontend/BusinessAsUsual.Web/Program.cs` has:
```csharp
builder.Services.AddHttpClient("YourModuleApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:YOUR_PORT");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```
2. Verify port matches `docs/PORT_REGISTRY.md` and `launchSettings.json`
3. Ensure API is running (check Visual Studio Output window)

### Port Conflicts
- **Consult:** `docs/PORT_REGISTRY.md` (authoritative registry)
- Ensure each module API has a unique port in launchSettings.json
- Update shell's named HttpClient configuration to match
- Verify no hardcoded URLs in Program.cs
- **If you get "connection refused"**: API port in Program.cs doesn't match launchSettings.json

### Route Mismatches
- Use explicit lowercase routes in API controllers: `[Route("api/modulename")]`
- Ensure route patterns match between API and Web calls
- Check that route parameters are correctly typed (e.g., `{Id:guid}`)

### Layout Problems
- If pages look "plain-jane", verify they're using the shell's MainLayout
- Check that MudThemeProvider and related providers are in the layout chain
- Ensure pages are using MudBlazor components consistently
- **Compare against HR module structure** (HR is the gold standard)
- If layout is missing: Check App.razor has your module's assembly in AdditionalAssemblies
- **If sidebar is missing but header/footer show:** Module route missing from MainLayout.razor.cs `UpdateModuleFromUri` hardcoded routes (line ~192)

### Routing and Assembly Conflicts
**⚠️ "Ambiguous Routes" Error:**
```
Error: The following routes are ambiguous: 'Error' in 'YourModule.Web.Components.Pages.Error' 'Error' in 'OtherModule.Web.Components.Pages.Error'
```

**Root Cause:** Blazor template (`dotnet new blazor`) includes default pages that conflict when multiple module assemblies are loaded via `AdditionalAssemblies` in the shell.

**Solution:** Delete these files from `services/{ModuleName}/{ModuleName}.Web/Components/`:
```bash
cd "services/{ModuleName}/{ModuleName}.Web/Components"
Remove-Item -Force Routes.razor
cd Pages
Remove-Item -Force Error.razor
Remove-Item -Force Weather.razor
```

**Why?**
- Shell (`frontend/BusinessAsUsual.Web`) provides these pages for all modules
- Only module-specific pages should exist in module assemblies
- See CreateModule SKILL.md section 8 for complete details

### Module Discovery
- Verify module is in ModuleDiscoveryService fallback list
- Check ModuleRegistration.json has correct metadata
- Ensure navigation items point to valid routes
- Confirm module assembly is included in shell's AdditionalAssemblies in App.razor
- **CRITICAL:** Add module route to `MainLayout.razor.cs` UpdateModuleFromUri method:
  ```csharp
  else if (path.StartsWith("/yourmodule"))
      _currentModule = "YourModule";
  ```
  Without this, the sidebar will be hidden when navigating to your module

### API Integration
- Check that API is running and accessible on the configured port
- Verify connection strings and database initialization (if using EF Core)
- Implement proper fallback to InMemory repository if database isn't configured
- Add appropriate timeout handling for API calls

---

## Notes

This skill is a living document. As we build out module dashboards, capture additional patterns, components, and solutions here.

**Optional Components Decision Framework:**
- **Use Insight Cards** when: The module has 2-4 key metrics that users check frequently
- **Use Quick Stats** when: Visual data representation adds value (trends, comparisons, distributions)
- **Skip them** when: The module is simple or metrics aren't meaningful at the dashboard level
