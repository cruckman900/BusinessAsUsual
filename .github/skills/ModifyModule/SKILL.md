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

## UX Enhancement Patterns (MANDATORY)

When modifying or creating module pages, implement these patterns to maintain consistency across all modules. These enhancements significantly improve usability, accessibility, and user experience.

### ✅ UX Enhancement Checklist

Apply ALL of the following to every module page:

#### Required Components
- ✅ **PageBreadcrumb** - Always use `<PageBreadcrumb Items="_breadcrumbItems">` with Actions slot for page actions
- ✅ **KeyboardShortcutHandler** - Add `<KeyboardShortcutHandler OnShortcut="HandleKeyboardShortcut" />` after PageTitle
- ✅ **ToastService** - Inject and use for all user feedback (create/save/delete/error operations)
- ✅ **SmartDefaultsService** - Inject and use to remember/prefill form values across sessions
- ✅ **Quick Filter** - Add search/filter functionality to all data grids with `_searchString` binding

#### Recommended Components (Context-Dependent)
- ✅ **ContextualHint** - Show inline help for empty states, getting started, warnings
- ✅ **HelpTooltip** - Wrap complex form fields with contextual help text
- ✅ **LoadingSpinner** / **Skeleton Loaders** - Show loading states for async operations
- ✅ **Smart Empty States** - Provide helpful guidance when no data exists
- ✅ **Progressive Disclosure** - Hide advanced/rarely-used options in expansion panels
- ✅ **Bulk Actions** - Enable multi-select and bulk operations for data grids
- ✅ **Export Functionality** - Add CSV/PDF export for reports and data grids

#### Standard Keyboard Shortcuts (Alt+ to avoid browser conflicts)
- `Alt+N` → Create/Add (match breadcrumb primary action)
- `Alt+S` → Save
- `Alt+E` → Export
- `Alt+K` → Command Palette
- `Alt+F` → Focus Search/Filter
- `Escape` → Close Dialog
- `?` → Show Keyboard Shortcuts Help
- `g+d` → Go to Dashboard
- `g+u` → Go to Users
- `g+r` → Go to Roles
- `g+n` → Go to Notifications
- `g+s` → Go to Settings

**Reference Implementations:**
- `services/Platform/Platform.Web/Components/Pages/Users.razor` (Lines 1-578) - Gold standard for all patterns
- `services/Platform/Platform.Web/Components/Pages/Roles.razor` - Full pattern implementation
- `services/Platform/Platform.Web/Components/Pages/Settings.razor` - Settings page patterns

**See Also:**
- `.github/skills/CreateModule/SKILL.md` section 8.2.1 for detailed UX component implementation examples
- `services/Platform/Platform.Web/Components/Shared/` for all reusable UX components
- `services/Platform/Platform.Web/Services/` for ToastService and SmartDefaultsService

---

### Smart Empty States
**Apply to:** All data grids, lists, and collections  
**Pattern:**
```razor
@if (!_items.Any())
{
    <div class="text-center pa-12">
        <MudIcon Icon="@Icons.Material.Filled.{RelevantIcon}" Size="Size.Large" 
                 Style="font-size: 96px; opacity: 0.3;" Color="Color.Primary" Class="mb-4" />
        <MudText Typo="Typo.h4" GutterBottom="true">No {items} yet</MudText>
        <MudText Typo="Typo.body1" Color="Color.Secondary" Class="mb-4" Style="max-width: 500px; margin: 0 auto;">
            Get started by {helpful description}. {Why this matters}.
        </MudText>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" Size="Size.Large" 
                   StartIcon="@Icons.Material.Filled.Add" OnClick="OpenCreateDialog">
            Add Your First {Item}
        </MudButton>
        <div class="mt-6">
            <MudText Typo="Typo.caption" Color="Color.Secondary">
                💡 Tip: {Next step or helpful hint}
            </MudText>
        </div>
    </div>
}
```

**Guidelines:**
- Use conversational, human language
- Include a clear call-to-action
- Provide context (why this feature matters)
- Add helpful tips or next steps

### Skeleton Loaders
**Apply to:** All pages with async data loading  
**Pattern:**
```razor
@if (_isLoading)
{
    <SkeletonLoader Type="SkeletonLoader.SkeletonType.StatsCard" Rows="1" />
}
else
{
    <!-- Actual stats card -->
}
```

**Implementation:**
1. Add `_isLoading` field to `@code` block
2. Switch to `OnInitializedAsync` for data loading
3. Set `_isLoading = true` before fetch, `false` after
4. Call `StateHasChanged()` before async operation
5. Wrap UI in loading branch using `SkeletonLoader`

**Available Types:**
- `DataGrid` - Table/grid with rows
- `Card` - Content cards
- `StatsCard` - Metric cards
- `List` - Simple lists

**Best Practices:**
- Match skeleton type to actual layout
- Use realistic row counts
- Keep load times brief (< 1s ideal)
- Show skeleton for initial load only

---

### Breadcrumb Navigation with Actions
**Apply to:** All module pages  
**Pattern:**
```razor
<PageBreadcrumb Items="_breadcrumbItems">
    <Actions>
        <MudButton Variant="Variant.Outlined" Color="Color.Default" StartIcon="@Icons.Material.Filled.FileDownload" Size="Size.Small">
            Export
        </MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add" OnClick="OpenCreateDialog">
            Add Item
        </MudButton>
    </Actions>
</PageBreadcrumb>
```

**Migration Steps:**
1. **Remove old breadcrumb:**
   ```razor
   <!-- DELETE THIS -->
   <div class="mb-3">
       <MudLink Href="/dashboard">Dashboard</MudLink>
       <span class="mx-2">/</span>
       ...
   </div>
   ```

2. **Remove page header action row:**
   ```razor
   <!-- DELETE THIS -->
   <div class="d-flex justify-space-between align-center mb-4">
       <div>...</div>
       <MudButton>...</MudButton>
   </div>
   ```

3. **Add breadcrumb component:**
   ```razor
   <PageBreadcrumb Items="_breadcrumbItems">
       <Actions>
           <!-- Move action buttons here -->
       </Actions>
   </PageBreadcrumb>
   ```

4. **Simplify page header:**
   ```razor
   <div class="mb-4">
       <MudText Typo="Typo.h4" GutterBottom="true">Page Title</MudText>
       <MudText Typo="Typo.body1" Color="Color.Secondary">Description</MudText>
   </div>
   ```

5. **Add breadcrumb items in @code:**
   ```csharp
   private List<PageBreadcrumb.BreadcrumbItem> _breadcrumbItems = new()
   {
       new() { Text = "Dashboard", Href = "/dashboard" },
       new() { Text = "Module", Href = "/module" },
       new() { Text = "Current Page", Href = "/module/page", Icon = Icons.Material.Filled.Icon }
   };
   ```

**Action Button Best Practices:**
- Primary: `Variant.Filled`, `Color.Primary` (save/create/submit)
- Secondary: `Variant.Outlined`, `Color.Default` (export/settings)
- Always: `Size.Small`, `StartIcon` for consistency
- Limit to 2-3 actions max
- Order: secondary first, primary last

---

### Keyboard Shortcuts
**Apply to:** All module pages  
**Pattern:**
```razor
@inject IDialogService DialogService
@inject NavigationManager Navigation

<KeyboardShortcutHandler OnShortcut="HandleKeyboardShortcut" />
```

**Implementation:**
1. **Add component** at top of page (after `PageTitle`)
2. **Add injections** in directive section
3. **Add handler method** in `@code` block:

```csharp
private async Task HandleKeyboardShortcut(string shortcut)
{
    switch (shortcut)
    {
        case "alt-n":  // Changed from cmd-n to avoid browser conflicts
            OpenCreateDialog(); // Match primary action
            break;
        case "alt-s":  // Changed from cmd-s
            await SaveChanges(); // If applicable
            break;
        case "alt-e":  // Changed from cmd-e
            await ExportData(); // If export exists
            break;
        case "alt-k":  // Command palette
            await ShowCommandPalette();
            break;
        case "alt-f":  // Focus search/filter
            FocusSearchField();
            break;
        case "escape":
            if (_dialogVisible)
                CloseDialog();
            break;
        case "question":
            await DialogService.ShowAsync<KeyboardShortcutsDialog>("Keyboard Shortcuts");
            break;
        case "g-d":
            Navigation.NavigateTo("/dashboard");
            break;
        case "g-u":
            Navigation.NavigateTo("/platform/users");
            break;
        case "g-r":
            Navigation.NavigateTo("/platform/roles");
            break;
        case "g-n":
            Navigation.NavigateTo("/platform/notifications");
            break;
        case "g-s":
            Navigation.NavigateTo("/platform/settings");
            break;
        // Add g-[key] for module-specific navigation
    }
}
```

**Standard Shortcuts to Implement:**
- `alt-n` → Match "Create/Add" breadcrumb action
- `alt-s` → Match "Save" breadcrumb action (if exists)
- `alt-e` → Match "Export" breadcrumb action (if exists)
- `alt-k` → Command palette (if implemented)
- `alt-f` → Focus search/filter field
- `escape` → Close open dialog
- `question` or `?` → Show help (always)
- `g-d` → Dashboard (always)
- `g-[key]` → Module pages (match your module)

**⚠️ CRITICAL: Use Alt+ not Ctrl+**
- **Always use `alt-` prefix** to avoid browser conflicts (Ctrl+S = Save Page, Ctrl+N = New Window)
- The KeyboardShortcutHandler component already handles Alt modifier
- Update KeyboardShortcutsDialog.razor to show "Alt+" in help text

**Required:**
- Always add `question` and `g-d`
- Always close dialogs on `escape` if any exist
- Match keyboard shortcuts to visible breadcrumb actions

---

### Toast Notifications with Actions
**Apply to:** All create/save/delete operations  
**Migration:**
```csharp
@inject ToastService Toast

// ❌ Replace Snackbar.Add calls
// ✅ Use Toast service instead

// Create
Toast.Created("User");

// Save
Toast.Saved("Settings");

// Delete with undo
var deleted = item;
_items.Remove(item);
Toast.Deleted(item.Name, () =>
{
    _items.Add(deleted);
    StateHasChanged();
});
```

**Standard Patterns:**
- `Toast.Created(name)` - Brief success with checkmark
- `Toast.Saved(name)` - Brief success with checkmark
- `Toast.Deleted(name, undoAction)` - 5s with Undo button
- `Toast.Success/Info/Warning/Error(message)` - Standard severities

**Undo Pattern:**
1. Capture item reference before removal
2. Perform the delete
3. Call `Toast.Deleted` with undo lambda
4. In undo: restore item + `StateHasChanged()`

**Always provide undo for:**
- Delete operations
- Archive operations
- Permanent state changes

---

### Recent Activity Widget
**Apply to:** Module dashboards/home pages  
**Pattern:**
```razor
<RecentActivityWidget Activities="_recentActivities" MaxItems="8" ShowViewAll="true" OnViewAll="NavigateToDetails" />
```

**Implementation:**
```csharp
private List<RecentActivityWidget.ActivityItem> _recentActivities = new();

// In OnInitializedAsync or after data load
_recentActivities = new List<RecentActivityWidget.ActivityItem>
{
    new() { 
        Type = RecentActivityWidget.ActivityType.Created, 
        User = "User Name", 
        Action = "created item", 
        Target = "Item Name", 
        Timestamp = DateTime.Now.AddMinutes(-5) 
    }
};
```

**Activity Types:**
- `Created`, `Updated`, `Deleted` - CRUD operations
- `Login`, `Logout` - Auth events
- `Export`, `Import` - Data operations
- `Approved`, `Rejected` - Workflow
- `Comment` - User feedback

**Use metadata badges for priority/status:**
- `Metadata = "High"` → Red badge
- `Metadata = "Pending"` → Orange badge
- `Metadata = "Approved"` → Green badge

---

### Visual Feedback on Actions
**Apply to:** Form submissions, save operations, data loading  
**Pattern:**
```razor
<!-- Auto-feedback button -->
<ActionButton Text="Save" LoadingText="Saving..." SuccessText="Saved!" OnClick="SaveChanges" />

<!-- Manual spinner -->
<LoadingSpinner IsVisible="@_isLoading" Text="Loading data..." />
```

**ActionButton Usage:**
- Automatically shows spinner during async operation
- Displays success checkmark for 2 seconds
- Prevents double-clicks
- Use for: save, create, delete, submit actions

**LoadingSpinner Usage:**
- Manual control via `IsVisible` parameter
- Use for: data fetching, background processes
- Optional text message

**Migration:**
```razor
<!-- ❌ OLD: Static button -->
<MudButton OnClick="SaveUser">Save</MudButton>

<!-- ✅ NEW: With feedback -->
<ActionButton Text="Save User" LoadingText="Saving..." OnClick="SaveUser" />
```

**Always show feedback for:**
- Form submissions
- Save operations
- Delete operations
- Data exports
- Async operations > 200ms

---

### Smart Defaults & Prefill
**Apply to:** All create/edit forms  
**Pattern:**
```razor
@inject SmartDefaultsService Defaults

// Prefill on create
var defaults = Defaults.GetUserDefaults();
_newItem = new ItemModel { Status = defaults.DefaultStatus };

// Remember after save
Defaults.RememberValue("item.lastStatus", _newItem.Status);
```

**Migration:**
```razor
<!-- ❌ OLD: Empty form -->
_newUser = new UserModel { IsActive = true };

<!-- ✅ NEW: Smart defaults -->
var defaults = Defaults.GetUserDefaults();
_newUser = new UserModel 
{ 
    IsActive = defaults.DefaultActive,
    Role = defaults.DefaultRole,
    Department = defaults.DefaultDepartment 
};
```

**After save, remember selections:**
```csharp
Defaults.RememberUserFormData(role, department, isActive);
```

**Common defaults to apply:**
- IsActive/Enabled → true
- Role/Status → last used
- Department/Category → last used
- Date → today
- Currency/Language → user preference

---

### Contextual Help / Tooltips
**Apply to:** All forms and complex sections  
**Pattern:**
```razor
<!-- Wrap fields with help -->
<HelpTooltip HelpText="Primary email for system notifications and login">
    <MudTextField @bind-Value="Email" Label="Email" />
</HelpTooltip>

<!-- Add page hints -->
@if (_items.Count < 5)
{
    <ContextualHint Title="Quick Tip" Message="..." Dismissible="true" />
}
```

**Migration:**
```razor
<!-- ❌ OLD: No help -->
<MudTextField @bind-Value="Role" Label="Role" />

<!-- ✅ NEW: With tooltip -->
<HelpTooltip HelpText="Determines user permissions and access levels">
    <MudTextField @bind-Value="Role" Label="Role" />
</HelpTooltip>
```

**Add hints for:**
- Empty states (0 items)
- Getting started (< 5 items)
- Warnings (system-wide changes)
- Security tips (permissions, roles)
- Keyboard shortcuts

---

### Progressive Disclosure
**Apply to:** Settings, advanced features, optional fields  
**Pattern:**
```razor
<MudExpansionPanels Class="mt-4" Elevation="0">
    <MudExpansionPanel Style="border: 1px solid var(--mud-palette-divider);">
        <TitleContent>
            <div class="d-flex align-center" style="gap: 8px;">
                <MudIcon Icon="@Icons.Material.Filled.Settings" />
                <MudText>Advanced Options</MudText>
            </div>
        </TitleContent>
        <ChildContent>
            <MudGrid Spacing="3" Class="pa-2">
                <!-- Advanced fields -->
            </MudGrid>
        </ChildContent>
    </MudExpansionPanel>
</MudExpansionPanels>
```

**Migration:**
```razor
<!-- ❌ OLD: Everything visible -->
<MudGrid>
    <MudItem>Basic Field</MudItem>
    <MudItem>Advanced Field</MudItem>
    <MudItem>Rarely Used Field</MudItem>
</MudGrid>

<!-- ✅ NEW: Progressive disclosure -->
<MudGrid>
    <MudItem>Basic Field</MudItem>
</MudGrid>
<MudExpansionPanels Class="mt-4">
    <MudExpansionPanel>
        <TitleContent>Advanced Options</TitleContent>
        <ChildContent>
            <MudGrid Class="pa-2">
                <MudItem>Advanced Field</MudItem>
                <MudItem>Rarely Used Field</MudItem>
            </MudGrid>
        </ChildContent>
    </MudExpansionPanel>
</MudExpansionPanels>
```

**Use for:**
- Settings tabs (basic vs advanced)
- Optional form sections
- Power-user features
- Experimental options

---

### Inline Editing & Bulk Actions
**Apply to:** Data grids and tables  
**Pattern:**
```razor
<MudDataGrid T="ItemModel" Items="@_items" 
             EditMode="DataGridEditMode.Cell"
             MultiSelection="true" 
             @bind-SelectedItems="_selectedItems">
    <ToolBarContent>
        @if (_selectedItems.Any())
        {
            <MudChip>@_selectedItems.Count selected</MudChip>
            <MudButtonGroup Size="Size.Small">
                <MudButton OnClick="BulkActivate">Activate</MudButton>
                <MudButton OnClick="BulkDelete" Color="Color.Error">Delete</MudButton>
            </MudButtonGroup>
        }
    </ToolBarContent>
    <Columns>
        <SelectColumn T="ItemModel" />
        <PropertyColumn Property="x => x.Name" Editable="true" />
    </Columns>
</MudDataGrid>

@code {
    private HashSet<ItemModel> _selectedItems = new();
}
```

**Inline Editing:**
- Use `EditMode="DataGridEditMode.Cell"` for quick edits
- Use `EditMode="DataGridEditMode.Form"` for complex records
- Mark editable columns with `Editable="true"`

**Bulk Actions:**
1. Add `MultiSelection="true"` to grid
2. Add `<SelectColumn />` as first column
3. Bind `_selectedItems` field
4. Show bulk buttons when items selected
5. Clear selection after action

**Common bulk operations:**
- Activate/Deactivate
- Delete (with undo via Toast)
- Assign/Reassign
- Export selected

---

### Export Functionality
**Apply to:** Data grids, reports, lists  
**Pattern:**
```razor
@using System.Text
@inject IJSRuntime JS

<MudMenu Icon="@Icons.Material.Filled.FileDownload" Label="Export" EndIcon="@Icons.Material.Filled.ArrowDropDown">
    <MudMenuItem OnClick="ExportToCsv">Export to CSV</MudMenuItem>
    <MudMenuItem OnClick="ExportToPdf">Export to PDF</MudMenuItem>
</MudMenu>

@code {
    private async Task ExportToCsv()
    {
        var items = _selectedItems.Any() ? _selectedItems : _items;
        var csv = new StringBuilder();
        csv.AppendLine("Column1,Column2");
        foreach (var item in items)
        {
            csv.AppendLine($"\"{item.Name}\",\"{item.Status}\"");
        }
        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var base64 = Convert.ToBase64String(bytes);
        await JS.InvokeVoidAsync("downloadFile", $"export-{DateTime.Now:yyyy-MM-dd}.csv", base64, "text/csv");
        Toast.Success($"Exported {items.Count} item(s)");
    }
}
```

**JS Helper (in `Platform.Web/wwwroot/js/export.js`):**
```javascript
window.downloadFile = function (fileName, base64, type) {
    const link = document.createElement('a');
    link.href = `data:${type};base64,${base64}`;
    link.download = fileName;
    link.click();
};
```

**Key points:**
- Export selected items if any, else all
- Escape CSV values with quotes
- Use descriptive filename with date
- Show success toast with count
- Don't export sensitive fields

---

### Accessibility (a11y)
**Always apply** to every page and component  

**Quick Wins:**
```razor
<!-- Icon buttons need labels -->
<MudIconButton Icon="@Icons.Material.Filled.Delete" aria-label="Delete item" />

<!-- Decorative icons hidden from screen readers -->
<MudIcon Icon="@Icons.Material.Filled.Star" aria-hidden="true" />

<!-- Alerts need role and live region -->
<div role="alert" aria-live="polite">
    Success message
</div>
```

**Essential Checklist:**
- ✅ Skip link: `<a href="#main-content">Skip to main content</a>`
- ✅ ARIA labels on all icon-only buttons
- ✅ Keyboard navigation works (Tab/Shift+Tab through all interactive elements)
- ✅ Color contrast ≥ 4.5:1 for text
- ✅ Form labels visible and associated with inputs
- ✅ Focus indicators visible (never `outline: none` without replacement)
- ✅ Semantic HTML (`<nav>`, `<main>`, `<article>`)
- ✅ Alt text on informational images
- ✅ Logical heading hierarchy (h1→h2→h3)

**Common ARIA Attributes:**
- `aria-label` - Icon button names
- `aria-live="polite"` - Dynamic content (toasts)
- `aria-hidden="true"` - Decorative icons
- `aria-expanded` - Collapsible panels
- `aria-describedby` - Field help text

**Testing:**
- Tab through page (all interactive elements reachable?)
- Screen reader test (NVDA/JAWS/VoiceOver)
- Zoom to 200% (layout still works?)
- Keyboard only (no mouse)

---

## Notes

This skill is a living document. As we build out module dashboards, capture additional patterns, components, and solutions here.

**Optional Components Decision Framework:**
- **Use Insight Cards** when: The module has 2-4 key metrics that users check frequently
- **Use Quick Stats** when: Visual data representation adds value (trends, comparisons, distributions)
- **Skip them** when: The module is simple or metrics aren't meaningful at the dashboard level

