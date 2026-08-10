# SKILL: Create a Complete Business Module

## Objective
Create a fully functional business module from scratch in the Business As Usual platform, including API layer, web UI, database persistence, module registration, navigation, and mobile contracts.

## Prerequisites
- .NET 9 SDK installed
- Visual Studio 2026 or later
- Understanding of the module being created (domain, features, entities)
- Module Registry API running on port 5100
- SQL Server or in-memory database configured

## ⚠️ CRITICAL SETUP CHECKLIST

**Before creating ANY module, complete these steps to avoid common issues:**

### 1. Reserve Port Numbers (MANDATORY)
- 📖 **Consult:** `docs/PORT_REGISTRY.md` (authoritative port registry)
- 🔒 **Reserve** API and Web UI ports BEFORE creating launchSettings.json
- ✅ **Update** PORT_REGISTRY.md with your module's ports immediately
- ⚠️ **Never reuse** ports from existing modules (causes connection refused errors)

### 2. HttpClient Registration (MANDATORY)
**Location:** `frontend/BusinessAsUsual.Web/Program.cs`

After creating your module, you MUST:
```csharp
// Add THIS to Program.cs (line ~138):
var yourModuleServiceUrl = builder.Configuration["YourModuleApi:Url"] ?? "http://localhost:YOUR_PORT";
builder.Services.AddHttpClient("YourModuleApi", client =>
{
    client.BaseAddress = new Uri(yourModuleServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
```
- ✅ Use the **exact port** from your API's launchSettings.json
- ✅ Name pattern: `"{ModuleName}Api"` (e.g., `"ServicesApi"`, `"InventoryApi"`)
- ⚠️ **Module pages will fail (404/connection refused) without this registration**

### 3. Shell Integration (MANDATORY)
**Location:** `frontend/BusinessAsUsual.Web/`

- ✅ **ModuleDiscoveryService.cs** - Add module to fallback list
- ✅ **App.razor** - Add module Web assembly to AdditionalAssemblies
- ✅ **MainLayout.razor.cs** - Add module route to `UpdateModuleFromUri` hardcoded routes list (line ~192)
  ```csharp
  else if (path.StartsWith("/{modulename}"))
      _currentModule = "{ModuleName}";
  ```
- ⚠️ **Without ModuleDiscoveryService:** Module won't appear in navigation
- ⚠️ **Without App.razor assembly:** Module pages won't load
- ⚠️ **Without MainLayout route:** Sidebar will be hidden when navigating to module pages

### 4. Use HR Module as Template (GOLD STANDARD)
- ✅ **Copy structure from:** `services/HR/`
- ✅ **Not from:** Inventory, Sales, or older modules (they have known issues)
- ✅ **HR module is the reference** for:
  - Project structure
  - HttpClient usage (via IHttpClientFactory)
  - Component organization
  - Layout integration with shell MainLayout

### 5. API Controller Routes (MANDATORY)
```csharp
[Route("api/modulename")]  // ⚠️ MUST be lowercase, explicit route
[ApiController]
public class YourController : ControllerBase
{
    // ...
}
```
- ✅ Use explicit lowercase route
- ⚠️ **Route mismatches cause 404 errors in production**

### 6. Dual-Catalog Maintenance (CRITICAL)
**ALWAYS update BOTH catalogs when creating a new module:**

1. ✅ **ModuleCatalog.cs** (`BusinessAsUsual.Core/Modules/ModuleCatalog.cs`)
   - Conceptual definition: Group, Key, Name, Submodules
   ```csharp
   new("GroupName", "modulekey", "Display Name", new []
   {
       new SubmoduleDefinition("SubKey", "Sub Display Name"),
       // ...
   })
   ```

2. ✅ **GetFallbackModules()** (`frontend/BusinessAsUsual.Web/Services/ModuleDiscoveryService.cs`)
   - Runtime navigation: full hierarchy, icons, routes
   ```csharp
   new ModuleDto
   {
       ModuleId = "modulekey",
       Key = "modulekey",
       DisplayName = "Display Name",
       Description = "Brief description",
       UiEntryPoint = "/modulekey",
       Icon = Icons.Material.Filled.IconName,
       IsActive = true,
       NavigationItems = new List<NavigationItemDto> { /* ... */ }
   }
   ```

3. ✅ **Reference**: See `docs/MODULE_CATALOG_UNIFIED_REFERENCE.md` for complete protocol

**Why both?**
- ModuleCatalog.cs = Design-time reference, cross-module discovery
- GetFallbackModules() = Runtime shell navigation (what users see in sidebar)

### 7. Common Mistakes to Avoid
- ❌ **DON'T** inject bare `HttpClient` in Blazor components (causes runtime errors)
- ✅ **DO** use `IHttpClientFactory.CreateClient("YourModuleApi")`
- ❌ **DON'T** hardcode URLs in components
- ✅ **DO** use named HttpClient from shell registration
- ❌ **DON'T** skip updating PORT_REGISTRY.md
- ✅ **DO** update it BEFORE creating launchSettings

### 8. Remove Conflicting Default Template Pages (CRITICAL)
**When creating a module Web project from `dotnet new blazor`, the template includes default pages that conflict with the shell. You MUST remove these immediately after project creation.**

**⚠️ Symptom:** `The following routes are ambiguous: 'Error' in 'YourModule.Web.Components.Pages.Error' 'Error' in 'OtherModule.Web.Components.Pages.Error'`

**📂 Files to DELETE from `services/{ModuleName}/{ModuleName}.Web/Components/`:**
```bash
cd "services/{ModuleName}/{ModuleName}.Web/Components"
Remove-Item -Force Routes.razor
cd Pages
Remove-Item -Force Error.razor
Remove-Item -Force Weather.razor
```

**Why?**
- The shell (`frontend/BusinessAsUsual.Web`) already provides these pages
- Module assemblies are loaded via `AdditionalAssemblies` in `App.razor`
- Multiple modules with the same routes cause Blazor routing conflicts
- Only module-specific pages (like `Home.razor`, `Dashboard.razor`, feature pages) should remain

**✅ Keep ONLY:**
- Module-specific pages (e.g., `/Components/Pages/Home.razor` for your module dashboard)
- Module-specific components
- `_Imports.razor` (but verify it doesn't conflict)
- `App.razor` marker file (may be needed for assembly detection)

**❌ Always DELETE:**
- `Routes.razor` (shell provides routing)
- `Error.razor` (shell provides error boundary)
- `Weather.razor` (template demo page)
- Any other template demo pages

---

### 9. UX Enhancement Patterns (MANDATORY)

**Every module MUST implement these user experience patterns to maintain consistency and quality:**

#### A. Smart Empty States
**When:** No data exists in lists/grids  
**Pattern:**
```razor
@if (!_items.Any())
{
    <div class="text-center pa-12">
        <MudIcon Icon="@Icons.Material.Filled.{RelevantIcon}" Size="Size.Large" 
                 Style="font-size: 96px; opacity: 0.3;" Color="Color.Primary" Class="mb-4" />
        <MudText Typo="Typo.h4" GutterBottom="true">No {items} yet</MudText>
        <MudText Typo="Typo.body1" Color="Color.Secondary" Class="mb-4" Style="max-width: 500px; margin: 0 auto;">
            Helpful description of what the user can do here. Make it conversational and guide them.
        </MudText>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" Size="Size.Large" 
                   StartIcon="@Icons.Material.Filled.Add" OnClick="OpenCreateDialog" Class="mt-2">
            Add Your First {Item}
        </MudButton>
        <div class="mt-6">
            <MudText Typo="Typo.caption" Color="Color.Secondary">
                💡 Tip: {Helpful tip for getting started}
            </MudText>
        </div>
    </div>
}
else
{
    <!-- Your actual data grid/list -->
}
```

**Required Elements:**
- ✅ Large icon (96px) with 30% opacity
- ✅ Clear headline (what's missing)
- ✅ Helpful description (why it matters, what they can do)
- ✅ Primary CTA button (how to add first item)
- ✅ Tip or next step suggestion
- ✅ Human, conversational micro-copy

**Example Icons by Module Type:**
- Users/Team: `PersonAddAlt1`, `People`, `ManageAccounts`
- Roles/Permissions: `Shield`, `AdminPanelSettings`, `Security`
- Inventory: `Inventory2`, `ViewList`, `QrCode`
- Finance: `AccountBalance`, `Receipt`, `TrendingUp`
- Reports: `Assessment`, `BarChart`, `Analytics`

---

#### B. Skeleton Loaders
**When:** Data is loading from API/database  
**Pattern:**
```razor
@if (_isLoading)
{
    <SkeletonLoader Type="SkeletonLoader.SkeletonType.DataGrid" Rows="5" />
}
else if (!_items.Any())
{
    <!-- Smart empty state here -->
}
else
{
    <!-- Actual data grid/list -->
}
```

**Implementation:**
1. **Create Shared Component:** `Components/Shared/SkeletonLoader.razor`
   - Reusable component with configurable types: `DataGrid`, `Card`, `StatsCard`, `List`
   - Parameterized row count for flexible layouts
   - Uses MudBlazor `MudSkeleton` primitives

2. **Import in `_Imports.razor`:**
   ```razor
   @using {ModuleName}.Web.Components.Shared
   ```

3. **Add Loading State:**
   ```csharp
   private bool _isLoading = false;

   protected override async Task OnInitializedAsync()
   {
       _isLoading = true;
       StateHasChanged();

       // API call or data load
       await Task.Delay(800); // Simulate API
       _data = await FetchData();

       _isLoading = false;
   }
   ```

**Skeleton Types:**
- `DataGrid` - For table/grid listings (avatar + 2 text lines + badges)
- `Card` - For card-based layouts (title + description + actions)
- `StatsCard` - For metric cards (label + value + icon)
- `List` - For simple list items (icon + 2 text lines + metadata)

**Best Practices:**
- ✅ Always show skeleton during initial load
- ✅ Call `StateHasChanged()` before async operations to trigger skeleton
- ✅ Match skeleton type to actual content layout
- ✅ Use realistic row counts (5-8 for grids, 3-4 for cards)
- ✅ Keep skeleton duration brief (< 1s ideal, < 2s max)
- ⚠️ Don't show skeleton for cached/instant data
- ⚠️ Don't use skeleton for sub-200ms loads (feels janky)

---

#### C. Breadcrumb Navigation with Actions
**When:** Every page (provides context + quick access to common actions)  
**Pattern:**
```razor
<PageBreadcrumb Items="_breadcrumbItems">
    <Actions>
        <MudButton Variant="Variant.Outlined" 
                   Color="Color.Default" 
                   StartIcon="@Icons.Material.Filled.FileDownload"
                   Size="Size.Small">
            Export
        </MudButton>
        <MudButton Variant="Variant.Filled" 
                   Color="Color.Primary" 
                   StartIcon="@Icons.Material.Filled.Add"
                   OnClick="OpenCreateDialog">
            Add Item
        </MudButton>
    </Actions>
</PageBreadcrumb>
```

**Implementation:**
1. **Component Already Exists:** `Components/Shared/PageBreadcrumb.razor`
   - Automatically imported via `_Imports.razor`
   - Uses `BreadcrumbItem` model for navigation hierarchy
   - Optional `Actions` slot for page-level buttons

2. **Define Breadcrumb Items:**
   ```csharp
   private List<PageBreadcrumb.BreadcrumbItem> _breadcrumbItems = new()
   {
       new() { Text = "Dashboard", Href = "/dashboard" },
       new() { Text = "Module Name", Href = "/module" },
       new() { Text = "Current Page", Href = "/module/page", Icon = Icons.Material.Filled.PageIcon }
   };
   ```

3. **Replace Old Breadcrumb/Header Pattern:**
   - **Remove:** Manual breadcrumb `<div>` with links + separators
   - **Remove:** Page header row with title + action button in `justify-space-between`
   - **Add:** Single `<PageBreadcrumb>` with actions slot
   - **Simplify:** Page header to just title + subtitle (no icon, no button)

**Action Button Guidelines:**
- **Primary action** (create/add/save): `Variant.Filled`, `Color.Primary`
- **Secondary actions** (export/settings/filter): `Variant.Outlined`, `Color.Default`
- **All buttons:** `Size.Small` for compact layout
- **Icon placement:** Always use `StartIcon` for consistency
- **Order:** Secondary actions first, primary action last (right-most)

**Common Action Patterns by Page Type:**
- **List/Grid Pages:** Export + Add/Create
- **Detail Pages:** Edit + Delete + Duplicate
- **Settings Pages:** Reset to Defaults + Save Changes
- **Dashboard Pages:** Refresh + Settings + Add Widget
- **Notification Pages:** Mark All Read + Settings

**Best Practices:**
- ✅ Always include Dashboard as first breadcrumb item
- ✅ Include module name as middle breadcrumb (for multi-module apps)
- ✅ Show icon only on final (current) breadcrumb item
- ✅ Keep action count to 2-3 max (avoid overcrowding)
- ✅ Use descriptive action labels ("Add User" not just "Add")
- ⚠️ Don't duplicate actions that exist in the page content below
- ⚠️ Don't use tertiary actions in breadcrumb (keep them in dropdowns/menus)

---

#### D. Keyboard Shortcuts
**When:** Every page (improves power-user productivity)  
**Pattern:**
```razor
@inject IDialogService DialogService
@inject NavigationManager Navigation

<!-- At top of page -->
<KeyboardShortcutHandler OnShortcut="HandleKeyboardShortcut" />
```

**Implementation:**
1. **Components Already Exist:**
   - `Components/Shared/KeyboardShortcutHandler.razor` - Global listener component
   - `Components/Shared/KeyboardShortcutHandler.razor.js` - JavaScript interop
   - `Components/Shared/KeyboardShortcutsDialog.razor` - Help dialog

2. **Add Handler Method:**
   ```csharp
   private async Task HandleKeyboardShortcut(string shortcut)
   {
       switch (shortcut)
       {
           case "cmd-n":
               OpenCreateDialog();
               break;
           case "cmd-s":
               await SaveChanges();
               break;
           case "cmd-f":
           case "slash":
               // Focus search field
               break;
           case "cmd-e":
               await ExportData();
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
               Navigation.NavigateTo("/module/page");
               break;
       }
   }
   ```

3. **Add Required Injections:**
   ```razor
   @inject IDialogService DialogService
   @inject NavigationManager Navigation
   ```

**Standard Shortcuts:**
- **Ctrl/⌘ + N** → Create new item
- **Ctrl/⌘ + S** → Save changes
- **Ctrl/⌘ + E** → Export data
- **Ctrl/⌘ + F** or **/** → Focus search
- **Ctrl/⌘ + K** → Command palette (future)
- **Esc** → Close dialogs/clear search
- **?** → Show keyboard shortcuts help

**Vim-Style Navigation (g + key):**
- **g d** → Go to Dashboard
- **g u** → Go to Users
- **g r** → Go to Roles
- **g n** → Go to Notifications
- **g s** → Go to Settings

**Best Practices:**
- ✅ Always implement `question` shortcut for help dialog
- ✅ Always implement `escape` to close open dialogs
- ✅ Always implement `g-d` navigation to dashboard
- ✅ Implement action shortcuts that match breadcrumb actions (cmd-n for Create, etc.)
- ✅ Use `cmd-f` or `slash` to focus search fields when available
- ⚠️ Don't override browser shortcuts (Ctrl+T, Ctrl+W, etc.)
- ⚠️ Don't trigger shortcuts when user is typing in input fields (handler already filters)

---

#### E. Toast Notifications with Actions
**When:** All user actions (create, save, delete, etc.)  
**Pattern:**
```csharp
@inject ToastService Toast

// On create
Toast.Created("User");

// On save
Toast.Saved("User settings");

// On delete with undo
Toast.Deleted("John Doe", () =>
{
    // Undo logic
    _users.Add(deletedUser);
    StateHasChanged();
});

// With custom action
Toast.Success("Export complete", () => DownloadFile(), "Download");
```

**Implementation:**
1. **Service Already Exists:** `Services/ToastService.cs`
   - Automatically registered in `Program.cs`
   - Automatically imported via `_Imports.razor`

2. **Standard Methods:**
   - `Toast.Success(message, action?, actionLabel?)` - Generic success
   - `Toast.Info(message, action?, actionLabel?)` - Information
   - `Toast.Warning(message, action?, actionLabel?)` - Warnings
   - `Toast.Error(message, action?, actionLabel?)` - Errors with retry
   - `Toast.Saved(itemName)` - Quick save confirmation
   - `Toast.Created(itemName, viewAction?)` - Creation with optional view
   - `Toast.Deleted(itemName, undoAction)` - Deletion with mandatory undo

3. **Replace Direct Snackbar Calls:**
   ```csharp
   // ❌ OLD
   Snackbar.Add("User created successfully", Severity.Success);

   // ✅ NEW
   Toast.Created("User");

   // ❌ OLD
   _users.Remove(user);
   Snackbar.Add($"User {user.FullName} deleted", Severity.Info);

   // ✅ NEW
   var deletedUser = user;
   _users.Remove(user);
   Toast.Deleted(user.FullName, () =>
   {
       _users.Add(deletedUser);
       StateHasChanged();
   });
   ```

**Toast Types by Scenario:**
- **Create**: `Toast.Created(itemName)` - Shows checkmark, brief display
- **Save/Update**: `Toast.Saved(itemName)` - Shows checkmark, brief display
- **Delete**: `Toast.Deleted(itemName, undoAction)` - Shows "Undo" button, 5s display
- **Export/Download**: `Toast.Success(message, downloadAction, "Download")` - Shows action button
- **Errors**: `Toast.Error(message, retryAction, "Retry")` - Shows retry button, 5s display
- **Warnings**: `Toast.Warning(message)` - Important non-blocking alerts
- **Info**: `Toast.Info(message)` - Contextual information

**Best Practices:**
- ✅ Always provide undo for destructive actions (delete, archive, etc.)
- ✅ Use brief messages with checkmarks for success ("✓ User saved")
- ✅ Capture deleted item reference before removal for undo
- ✅ Call `StateHasChanged()` after undo restoration
- ✅ Use action labels that match the action ("Undo", "View", "Download", "Retry")
- ⚠️ Don't use generic "User deleted" - include the actual name
- ⚠️ Don't skip undo actions - users expect them for deletions

---

#### F. Recent Activity Widget
**When:** Dashboard/home pages (shows what's happening in the module)  
**Pattern:**
```razor
<RecentActivityWidget Activities="_recentActivities" MaxItems="8" ShowViewAll="true" OnViewAll="NavigateToAuditLog" />
```

**Implementation:**
1. **Component Already Exists:** `Components/Shared/RecentActivityWidget.razor`
   - Automatically imported via `_Imports.razor`
   - Displays recent user actions with avatars, timestamps, metadata

2. **Add Activity Data:**
   ```csharp
   private List<RecentActivityWidget.ActivityItem> _recentActivities = new();

   protected override async Task OnInitializedAsync()
   {
       _recentActivities = new List<RecentActivityWidget.ActivityItem>
       {
           new() { 
               Type = RecentActivityWidget.ActivityType.Created, 
               User = "John Doe", 
               Action = "created user", 
               Target = "Jane Smith", 
               Timestamp = DateTime.Now.AddMinutes(-5),
               Metadata = "High"  // Optional: priority/status badge
           },
           new() { 
               Type = RecentActivityWidget.ActivityType.Updated, 
               User = "Admin", 
               Action = "updated role", 
               Target = "Manager", 
               Timestamp = DateTime.Now.AddMinutes(-12) 
           },
           // ... more activities
       };
   }
   ```

**Available Activity Types:**
- `Created` - Green avatar, add icon
- `Updated` - Blue avatar, edit icon
- `Deleted` - Red avatar, delete icon
- `Login` / `Logout` - Authentication events
- `Export` / `Import` - Data transfer operations
- `Approved` / `Rejected` - Workflow actions
- `Comment` - Discussion/feedback

**Metadata Badge Colors (auto-styled):**
- `"High"`, `"Critical"`, `"Urgent"` → Red background
- `"Medium"`, `"Normal"` → Orange background
- `"Low"` → Blue background
- `"Approved"` → Green background
- `"Pending"` → Orange background
- `"Rejected"`, `"Failed"` → Red background

**Parameters:**
- `Activities` - List of activity items
- `MaxItems` - How many to display (default: 8)
- `ShowViewAll` - Show "View All" button (default: true)
- `OnViewAll` - Callback when "View All" clicked

**Best Practices:**
- ✅ Show most recent 8-10 activities
- ✅ Include user name, action verb, and target when applicable
- ✅ Use relative timestamps (5m ago, 2h ago, etc.)
- ✅ Add metadata badges for priority/status context
- ✅ Link "View All" to full audit log or activity page
- ✅ Order activities by timestamp descending (newest first)
- ⚠️ Don't show sensitive data (passwords, tokens, etc.)
- ⚠️ Don't clutter with too many metadata badges

---

#### G. Visual Feedback on Actions
**When:** All interactive actions (save, create, delete, submit)  
**Pattern:**
```razor
<!-- Automatic feedback button -->
<ActionButton Text="Save User" 
              LoadingText="Saving..." 
              SuccessText="Saved!" 
              StartIcon="@Icons.Material.Filled.Save"
              OnClick="SaveUser" />

<!-- Manual loading spinner -->
<LoadingSpinner IsVisible="@_isProcessing" Text="Processing your request..." />

<!-- Success checkmark animation -->
<SuccessCheckmark Show="@_showSuccess" />
```

**Implementation:**
1. **Components Already Exist:**
   - `ActionButton.razor` - Button with built-in loading/success states
   - `LoadingSpinner.razor` - Inline progress indicator
   - `SuccessCheckmark.razor` - Animated success checkmark

2. **Using ActionButton (Recommended):**
   ```razor
   <ActionButton Text="Create User"
                 LoadingText="Creating..."
                 SuccessText="Created!"
                 StartIcon="@Icons.Material.Filled.Add"
                 Color="Color.Primary"
                 OnClick="HandleCreate" />
   ```

   The button automatically:
   - Shows spinner during `OnClick` execution
   - Displays success state with checkmark
   - Resets after 2 seconds

3. **Manual Control:**
   ```csharp
   private bool _isSaving = false;
   private bool _showSuccess = false;

   private async Task SaveChanges()
   {
       _isSaving = true;
       StateHasChanged();

       await Task.Delay(1000); // Your save logic

       _isSaving = false;
       _showSuccess = true;
       StateHasChanged();

       await Task.Delay(2000);
       _showSuccess = false;
       StateHasChanged();
   }
   ```

**ActionButton Parameters:**
- `Text` - Default button text
- `LoadingText` - Text during processing (default: "Processing...")
- `SuccessText` - Text after success (default: "Success!")
- `StartIcon` - Icon to display
- `Variant`, `Color`, `Size`, `FullWidth` - Standard MudButton props
- `SuccessDisplayMs` - How long to show success (default: 2000ms)

**LoadingSpinner Parameters:**
- `IsVisible` - Show/hide spinner
- `Text` - Optional loading message
- `Size` - Small | Medium | Large
- `Color` - Spinner color

**Use Cases:**
- **Form submission** → ActionButton with "Saving..." → "Saved!"
- **Data refresh** → LoadingSpinner during fetch
- **Deletion** → ActionButton with "Deleting..." → Success toast
- **Export** → LoadingSpinner + progress text
- **Bulk operations** → LoadingSpinner with item count

**Best Practices:**
- ✅ Always show feedback for operations > 200ms
- ✅ Use ActionButton for single-click actions (save/create/delete)
- ✅ Use LoadingSpinner for background processes
- ✅ Show success state for 1-2 seconds, then reset
- ✅ Disable button during processing to prevent double-clicks
- ✅ Use descriptive loading text ("Saving user..." not just "Loading...")
- ⚠️ Don't show spinners for instant operations (< 100ms)
- ⚠️ Don't leave success state indefinitely - auto-reset

---

#### H. Smart Defaults & Prefill
**When:** All forms (speeds up data entry, reduces errors)  
**Pattern:**
```razor
@inject SmartDefaultsService Defaults

// On create dialog open
var defaults = Defaults.GetUserDefaults();
_currentUser = new UserModel 
{ 
    IsActive = defaults.DefaultActive,
    Role = defaults.DefaultRole,
    Department = defaults.DefaultDepartment
};

// After save
Defaults.RememberUserFormData(_currentUser.Role, _currentUser.Department, _currentUser.IsActive);
```

**Implementation:**
1. **Service Already Exists:** `Services/SmartDefaultsService.cs`
   - Automatically registered in `Program.cs`
   - Remembers user's last selections

2. **Common Patterns:**
   ```csharp
   // On create
   private void OpenCreateDialog()
   {
       var defaults = Defaults.GetUserDefaults();
       _currentItem = new ItemModel
       {
           Status = defaults.DefaultStatus,
           Priority = defaults.DefaultPriority,
           AssignedTo = defaults.DefaultAssignee
       };
   }

   // After save
   private void SaveItem()
   {
       // Save logic...

       // Remember for next time
       Defaults.RememberValue("item.lastStatus", _currentItem.Status);
       Defaults.RememberValue("item.lastPriority", _currentItem.Priority);
   }

   // Get specific default
   var lastDepartment = Defaults.GetValue("user.department", "Sales");
   ```

3. **Built-in Default Helpers:**
   - `GetUserDefaults()` - Role, department, active status
   - `GetFormDefaults()` - Date, currency, language
   - `RememberValue(key, value)` - Store any preference
   - `GetValue<T>(key, fallback)` - Retrieve with fallback

**Smart Default Strategies:**
- **Last used value** - Default to user's previous selection
- **Most common value** - Pre-select frequently used option
- **Context-aware** - Default based on current context (time, user role, etc.)
- **Calculated** - Derive from other fields (e.g., email domain → company)

**Common Use Cases:**
- **User creation**: Default role, department, timezone
- **Forms**: Date → today, currency → user's last, language → browser
- **Filters**: Remember last search/filter criteria
- **Settings**: Persist user preferences across sessions
- **Dropdown selections**: Pre-select user's frequent choices

**Best Practices:**
- ✅ Always prefill "IsActive" / "Enabled" to true for new items
- ✅ Remember last-used values for dropdowns (role, department, status)
- ✅ Default dates to today/now for new records
- ✅ Pre-fill email domain based on company settings
- ✅ Use browser timezone as default for new users
- ✅ Clear remembered values on logout for security
- ⚠️ Don't prefill sensitive fields (passwords, payment info)
- ⚠️ Don't auto-select destructive options (delete, archive)
- ⚠️ Don't persist across users - keep defaults user-specific

---

#### I. Contextual Help / Tooltips
**When:** All forms and complex UI sections  
**Pattern:**
```razor
<!-- Inline field tooltips -->
<HelpTooltip HelpText="Enter the user's full legal name as it appears on official documents">
    <MudTextField @bind-Value="Model.FullName" Label="Full Name" />
</HelpTooltip>

<!-- Page-level contextual hints -->
<ContextualHint Title="Pro Tip" 
               Message="Start by creating your first user. Use keyboard shortcuts for faster navigation."
               Dismissible="true" />
```

**Implementation:**
1. **Components Already Exist:**
   - `HelpTooltip.razor` - Inline field help with ? icon
   - `ContextualHint.razor` - Page-level tips with lightbulb icon

2. **HelpTooltip Usage:**
   ```razor
   <HelpTooltip HelpText="Determines user permissions and access levels">
       <MudSelect @bind-Value="Role" Label="Role">
           <MudSelectItem Value="Admin">Administrator</MudSelectItem>
       </MudSelect>
   </HelpTooltip>
   ```

3. **ContextualHint Usage:**
   ```razor
   @if (_items.Count == 0 && !_isLoading)
   {
       <ContextualHint Title="Getting Started" 
                      Message="Create your first item to begin tracking."
                      Dismissible="true" />
   }
   ```

**When to Use Each:**
- **HelpTooltip**: Form fields, buttons, complex controls
- **ContextualHint**: Empty states, page introductions, important warnings, pro tips

**Help Text Best Practices:**
- ✅ Be concise (1-2 sentences max)
- ✅ Explain WHY, not just WHAT ("Determines permissions..." vs "Select a role")
- ✅ Include examples when helpful ("e.g., 'Sales Manager', 'Support Lead'")
- ✅ Warn about consequences ("Affects all users" / "Cannot be undone")
- ✅ Show keyboard shortcuts in hints ("Press / for search, ? for help")
- ⚠️ Don't repeat the label ("Email" → "Enter email" is redundant)
- ⚠️ Don't state the obvious ("Click here to save" on Save button)
- ⚠️ Don't write novels - keep it scannable

**Contextual Hint Patterns:**
- **Empty state**: "Create your first X to get started"
- **Few items**: "Pro tip: Use keyboard shortcuts for faster work"
- **Warnings**: "Changes affect all users - test in staging first"
- **Security**: "Grant minimum permissions required for each role"
- **Getting started**: "System roles cannot be deleted. Create custom roles for team needs."

---

#### J. Progressive Disclosure
**When:** Settings pages, advanced features, optional fields  
**Pattern:**
```razor
<!-- MudBlazor Expansion Panels -->
<MudExpansionPanels Elevation="0">
    <MudExpansionPanel Style="border: 1px solid var(--mud-palette-divider);">
        <TitleContent>
            <div class="d-flex align-center" style="gap: 8px;">
                <MudIcon Icon="@Icons.Material.Filled.Settings" />
                <MudText>Advanced Options</MudText>
            </div>
        </TitleContent>
        <ChildContent>
            <!-- Advanced fields here -->
            <MudGrid Spacing="3" Class="pa-2">
                <MudItem xs="12">
                    <HelpTooltip HelpText="...">
                        <MudSwitch Label="Advanced Feature" />
                    </HelpTooltip>
                </MudItem>
            </MudGrid>
        </ChildContent>
    </MudExpansionPanel>
</MudExpansionPanels>
```

**When to Use:**
- **Settings pages**: Basic vs Advanced sections
- **Forms with optional fields**: Hide rarely-used fields behind "More Options"
- **Complex configurations**: Email, Security, API settings
- **Power-user features**: Bulk operations, developer tools, experimental features

**Best Practices:**
- ✅ Keep essential fields visible by default
- ✅ Group related advanced options together
- ✅ Use descriptive panel titles ("Advanced Security Options" not just "Advanced")
- ✅ Add icons to panel headers for visual clarity
- ✅ Combine with tooltips inside panels (users need more help with advanced options)
- ✅ Maintain the same styling/spacing inside panels as outside
- ⚠️ Don't hide required fields in panels
- ⚠️ Don't create more than 2-3 levels of nesting (confusing)
- ⚠️ Don't use for frequently-accessed options (defeats the purpose)

**Common Patterns:**
```razor
<!-- Security: Basic + Advanced -->
<MudGrid>Basic fields...</MudGrid>
<MudExpansionPanels Class="mt-4">
    <MudExpansionPanel>Advanced Security Options</MudExpansionPanel>
</MudExpansionPanels>

<!-- Email: SMTP + Advanced Email Settings -->
<MudGrid>SMTP fields...</MudGrid>
<MudExpansionPanels Class="mt-4">
    <MudExpansionPanel>Advanced Email Settings</MudExpansionPanel>
</MudExpansionPanels>

<!-- Form: Required + Optional Fields -->
<MudGrid>Name, Email, Role...</MudGrid>
<MudExpansionPanels Class="mt-3">
    <MudExpansionPanel>Additional Details (Optional)</MudExpansionPanel>
</MudExpansionPanels>
```

---

#### K. Inline Editing
**When:** Data grids, tables with frequent updates  
**Pattern:**
```razor
<!-- MudDataGrid with inline cell editing -->
<MudDataGrid T="ItemModel" Items="@_items" 
             EditMode="DataGridEditMode.Cell" 
             ReadOnly="false">
    <Columns>
        <PropertyColumn Property="x => x.Name" Title="Name" Editable="true" />
        <PropertyColumn Property="x => x.Status" Title="Status" Editable="true" />
        <PropertyColumn Property="x => x.Price" Title="Price" Editable="true" />
    </Columns>
</MudDataGrid>
```

**Edit Modes:**
- **DataGridEditMode.Cell** - Click to edit individual cells (fastest for quick edits)
- **DataGridEditMode.Form** - Opens dialog for full row editing (better for complex data)
- **DataGridEditMode.Inline** - Edit row inline without dialog (balance of both)

**Best Practices:**
- ✅ Use Cell mode for simple text/number fields
- ✅ Use Form mode for complex records with many fields
- ✅ Mark read-only columns as `Editable="false"`
- ✅ Validate on commit (built into MudDataGrid)
- ✅ Show visual feedback on save (use ToastService)
- ⚠️ Don't make every column editable (confusing UX)
- ⚠️ Don't allow inline editing of critical/destructive fields without confirmation

---

#### L. Bulk Actions
**When:** Data grids, lists with multiple items  
**Pattern:**
```razor
<MudDataGrid T="ItemModel" Items="@_items" 
             MultiSelection="true" 
             @bind-SelectedItems="_selectedItems">
    <ToolBarContent>
        <MudText Typo="Typo.h6">Items</MudText>
        <MudSpacer />

        @if (_selectedItems.Any())
        {
            <MudChip Color="Color.Info" Size="Size.Small" Class="mr-2">
                @_selectedItems.Count selected
            </MudChip>
            <MudButtonGroup Variant="Variant.Outlined" Size="Size.Small">
                <MudButton StartIcon="@Icons.Material.Filled.CheckCircle" OnClick="BulkActivate">
                    Activate
                </MudButton>
                <MudButton StartIcon="@Icons.Material.Filled.Delete" Color="Color.Error" OnClick="BulkDelete">
                    Delete
                </MudButton>
            </MudButtonGroup>
        }
    </ToolBarContent>
    <Columns>
        <SelectColumn T="ItemModel" />
        <!-- Other columns -->
    </Columns>
</MudDataGrid>

@code {
    private HashSet<ItemModel> _selectedItems = new();

    private void BulkActivate()
    {
        foreach (var item in _selectedItems)
        {
            item.IsActive = true;
        }
        Toast.Success($"Activated {_selectedItems.Count} item(s)");
        _selectedItems.Clear();
    }

    private async Task BulkDelete()
    {
        var count = _selectedItems.Count;
        var itemsToDelete = _selectedItems.ToList();

        foreach (var item in itemsToDelete)
        {
            _items.Remove(item);
        }

        _selectedItems.Clear();
        Toast.Deleted($"{count} item(s)", async () =>
        {
            // Undo: restore deleted items
            _items.AddRange(itemsToDelete);
            await Task.CompletedTask;
        });
    }
}
```

**Implementation Steps:**
1. Add `MultiSelection="true"` to MudDataGrid
2. Bind `@bind-SelectedItems` to a `HashSet<T>` field
3. Add `<SelectColumn T="ItemModel" />` as first column
4. Show bulk action buttons conditionally when items are selected
5. Use `MudButtonGroup` for related actions
6. Clear selection after action completes
7. Use Toast with undo for destructive actions

**Common Bulk Actions:**
- **Activate/Deactivate** - Toggle status on multiple items
- **Delete** - Remove multiple items (always provide undo)
- **Assign/Reassign** - Change owner, category, status
- **Export** - Export selected items only
- **Tag/Categorize** - Add labels or categories
- **Archive** - Move to archive (soft delete)

**Best Practices:**
- ✅ Show selection count prominently (`5 selected`)
- ✅ Disable individual row actions when bulk mode is active
- ✅ Provide undo for destructive actions (delete, archive)
- ✅ Use MudButtonGroup to visually group related bulk actions
- ✅ Clear selection after successful bulk operation
- ✅ Add keyboard shortcut for "Select All" (Ctrl+A)
- ⚠️ Don't allow bulk operations on mixed item types/states
- ⚠️ Don't execute destructive bulk actions without confirmation for large selections (> 10 items)
- ⚠️ Don't hide the selection count - users need to know scope

---

#### M. Export Functionality
**When:** Data grids, reports, lists  
**Pattern:**
```razor
<!-- Export menu in breadcrumb actions -->
<MudMenu Icon="@Icons.Material.Filled.FileDownload" 
         Label="Export" 
         Variant="Variant.Outlined" 
         EndIcon="@Icons.Material.Filled.ArrowDropDown">
    <MudMenuItem Icon="@Icons.Material.Filled.TableView" OnClick="@(() => ExportToCsv())">
        Export to CSV
    </MudMenuItem>
    <MudMenuItem Icon="@Icons.Material.Filled.PictureAsPdf" OnClick="@(() => ExportToPdf())">
        Export to PDF
    </MudMenuItem>
    <MudMenuItem Icon="@Icons.Material.Filled.Description" OnClick="@(() => ExportToExcel())">
        Export to Excel
    </MudMenuItem>
    <MudDivider />
    <MudMenuItem Icon="@Icons.Material.Filled.Print" OnClick="@(() => Print())">
        Print
    </MudMenuItem>
</MudMenu>

@code {
    @inject IJSRuntime JS

    private async Task ExportToCsv()
    {
        var itemsToExport = _selectedItems.Any() ? _selectedItems.ToList() : _items;

        var csv = new StringBuilder();
        csv.AppendLine("Column1,Column2,Column3");

        foreach (var item in itemsToExport)
        {
            csv.AppendLine($"\"{item.Name}\",\"{item.Status}\",\"{item.Price}\"");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var base64 = Convert.ToBase64String(bytes);
        var fileName = $"export-{DateTime.Now:yyyy-MM-dd}.csv";

        await JS.InvokeVoidAsync("downloadFile", fileName, base64, "text/csv");
        Toast.Success($"Exported {itemsToExport.Count} item(s) to CSV");
    }
}
```

**JavaScript Helper (already exists in `Platform.Web/wwwroot/js/export.js`):**
```javascript
window.downloadFile = function (fileName, base64Content, contentType) {
    const link = document.createElement('a');
    link.href = `data:${contentType};base64,${base64Content}`;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
```

**Implementation Steps:**
1. Add `@inject IJSRuntime JS` and `@using System.Text`
2. Create export menu with dropdown options
3. Implement CSV export first (universal, no dependencies)
4. Respect selection: export selected items if any, otherwise all
5. Include column headers
6. Escape values with quotes for CSV safety
7. Use `downloadFile` JS helper for client-side download
8. Show success toast with count

**Export Formats:**
- **CSV** - Universal, opens in Excel/Sheets, easy to parse
- **Excel (.xlsx)** - Requires library (EPPlus, ClosedXML, NPOI)
- **PDF** - Requires library (iTextSharp, QuestPDF)
- **Print** - Window.print() via JS

**Best Practices:**
- ✅ Always export selected items if selection exists
- ✅ Include column headers in first row
- ✅ Escape special characters (",) in CSV
- ✅ Use descriptive filename with date (`users-export-2024-01-15.csv`)
- ✅ Show feedback toast after export
- ✅ Consider file size limits (> 10k rows → suggest server-side export)
- ✅ Respect user permissions (don't export restricted data)
- ⚠️ Don't export sensitive fields (passwords, SSN, credit cards)
- ⚠️ Don't block UI during large exports (use background task)
- ⚠️ Don't export without user action (privacy/security risk)

**CSV Formatting Tips:**
```csharp
// Escape quotes
var safe = value.Replace("\"", "\"\"");

// Wrap in quotes if contains comma, newline, or quote
if (value.Contains(',') || value.Contains('\n') || value.Contains('"'))
{
    value = $"\"{safe}\"";
}

// Date formatting
csv.AppendLine($"{date:yyyy-MM-dd HH:mm}");
```

---

#### N. Accessibility (a11y)
**Always:** Every component and page  
**Pattern:**
```razor
<!-- Skip to content link (in MainLayout) -->
<a href="#main-content" class="skip-link">Skip to main content</a>
<main id="main-content">@Body</main>

<!-- ARIA labels on buttons -->
<MudIconButton Icon="@Icons.Material.Filled.Close" 
               aria-label="Close dialog" />
<MudIconButton Icon="@Icons.Material.Filled.Delete" 
               aria-label="Delete user" />

<!-- ARIA roles and live regions -->
<div role="alert" aria-live="polite">
    Important message here
</div>

<!-- Decorative icons -->
<MudIcon Icon="@Icons.Material.Filled.Star" aria-hidden="true" />

<!-- Form labels and descriptions -->
<MudTextField Label="Email" 
              aria-describedby="email-help" />
<MudText id="email-help" Typo="Typo.caption">
    We'll never share your email
</MudText>
```

**Essential Accessibility Checklist:**
- ✅ **Skip links**: Add "Skip to main content" for keyboard users
- ✅ **ARIA labels**: Label all icon-only buttons/links
- ✅ **ARIA roles**: Use `role="alert"`, `role="dialog"`, `role="navigation"`
- ✅ **Live regions**: Use `aria-live="polite"` for dynamic content
- ✅ **Keyboard navigation**: All interactive elements reachable via Tab
- ✅ **Focus indicators**: Never remove focus outlines (`:focus-visible` is ok)
- ✅ **Color contrast**: Minimum 4.5:1 for text, 3:1 for UI components
- ✅ **Form labels**: Every input needs a visible label
- ✅ **Error messages**: Associate errors with fields via `aria-describedby`
- ✅ **Semantic HTML**: Use `<nav>`, `<main>`, `<aside>`, `<article>`
- ✅ **Alt text**: All informational images need descriptive alt
- ✅ **Heading hierarchy**: Logical h1→h2→h3, no skipping levels

**Keyboard Navigation:**
```razor
<!-- Already handled by MudBlazor components -->
<MudButton>Tab-focusable by default</MudButton>
<MudDataGrid>Arrow keys work automatically</MudDataGrid>

<!-- Custom components need tabindex -->
<div tabindex="0" role="button" @onclick="DoSomething" @onkeydown="HandleKey">
    Custom clickable
</div>
```

**Screen Reader Support:**
```razor
<!-- Loading state -->
<div aria-busy="true" aria-live="polite">
    <MudProgressCircular />
    <span class="sr-only">Loading users...</span>
</div>

<!-- Visually hidden text -->
<style>
.sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    margin: -1px;
    padding: 0;
    overflow: hidden;
    clip: rect(0,0,0,0);
    white-space: nowrap;
    border: 0;
}
</style>
```

**Common ARIA Attributes:**
- `aria-label` - Accessible name for icon buttons
- `aria-labelledby` - Reference to visible label element
- `aria-describedby` - Additional description/help text
- `aria-live="polite"` - Announce dynamic changes (toasts, alerts)
- `aria-hidden="true"` - Hide decorative elements from screen readers
- `aria-expanded` - Indicate collapsible state (dropdowns, panels)
- `aria-selected` - Indicate selection in lists/tabs
- `aria-pressed` - Toggle button state

**Testing:**
- Tab through entire page - all interactive elements reachable?
- Use screen reader (NVDA/JAWS/VoiceOver) - does it make sense?
- Zoom to 200% - layout still usable?
- Check contrast with browser DevTools
- Test with keyboard only (no mouse)

**Best Practices:**
- ✅ Provide text alternatives for all non-text content
- ✅ Ensure sufficient color contrast
- ✅ Make all functionality keyboard-accessible
- ✅ Give users enough time to read/use content
- ✅ Don't rely on color alone to convey information
- ✅ Use clear, consistent navigation
- ⚠️ Don't remove focus outlines without replacing with visible alternative
- ⚠️ Don't use placeholder as label (placeholders disappear on focus)
- ⚠️ Don't auto-play audio/video without user control

---

## Module Architecture Overview
Each module follows a clean architecture pattern:
```
services/{ModuleName}/
├── {ModuleName}.API/           # REST API endpoints
├── {ModuleName}.Web/           # Blazor UI (standalone)
├── {ModuleName}.Application/   # Business logic, DTOs, services
├── {ModuleName}.Domain/        # Domain entities, interfaces
├── {ModuleName}.Infrastructure/# Database, persistence
├── {ModuleName}.Contracts/     # Mobile UI specifications
└── {ModuleName}.Tests/         # Unit & integration tests
```

## Step-by-Step Process

### Phase 1: Module Planning & Design

#### 1.1 Choose Module Domain
**Common module suggestions:**
- **Inventory Management** - Track products, stock levels, warehouses, purchase orders
- **Project Management** - Projects, tasks, milestones, time tracking, budgets
- **Sales** - Quotes, orders, order fulfillment, shipping, sales analytics
 - **Services** - Manage sellable services, pricing, appointments, service providers, and service orders
- **Marketing** - Campaigns, leads sources, email marketing, analytics
- **Support/Helpdesk** - Tickets, knowledge base, SLA tracking, customer support
- **Asset Management** - Company assets, maintenance schedules, depreciation
- **Procurement** - Vendor management, RFQs, purchase requisitions, contracts
- **Quality Management** - Audits, inspections, non-conformances, corrective actions
- **Document Management** - Document repository, version control, approval workflows
- **Compliance** - Regulatory requirements, audits, certifications, training records

#### 1.2 Define Core Entities
List 3-7 primary entities for the module:
```
Example for Inventory:
- Product
- StockItem
- Warehouse
- PurchaseOrder
- StockAdjustment
- Supplier
- InventoryTransaction
```

#### 1.3 Define Navigation Structure
Plan the sidebar menu hierarchy:
```
Example for Inventory:
- Dashboard
- Products (group)
  - All Products
  - Categories
  - Add Product
- Stock Management (group)
  - Stock Levels
  - Adjustments
  - Transfers
- Warehouses
- Purchase Orders (group)
  - All Orders
  - Create Order
- Suppliers
- Reports
```

---

### Phase 2: Create Project Structure

#### 2.1 Create Solution Folders
```bash
cd "D:\DotNet Projects\BusinessAsUsual\services"
mkdir {ModuleName}
cd {ModuleName}

# Create projects
dotnet new webapi -n {ModuleName}.API
dotnet new blazor -n {ModuleName}.Web
dotnet new classlib -n {ModuleName}.Application
dotnet new classlib -n {ModuleName}.Domain
dotnet new classlib -n {ModuleName}.Infrastructure
dotnet new classlib -n {ModuleName}.Contracts
dotnet new xunit -n {ModuleName}.Tests

# ⚠️ CRITICAL: Immediately remove conflicting template files from Web project
cd {ModuleName}.Web/Components
Remove-Item -Force Routes.razor
cd Pages
Remove-Item -Force Error.razor
Remove-Item -Force Weather.razor
cd ../../../
```

**Why remove these files?**
- Prevents "ambiguous routes" errors when multiple modules are loaded in the shell
- Shell provides `Routes.razor`, `Error.razor` via `AdditionalAssemblies`
- See section 8 above for full explanation

#### 2.2 Add Projects to Solution
**CRITICAL:** Add all projects to the solution immediately after creating them:

```bash
cd "D:\DotNet Projects\BusinessAsUsual"

dotnet sln add services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Application/{ModuleName}.Application.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Domain/{ModuleName}.Domain.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Contracts/{ModuleName}.Contracts.csproj
dotnet sln add services/{ModuleName}/{ModuleName}.Tests/{ModuleName}.Tests.csproj
```

⚠️ **Common Issue:** If you forget this step, projects won't appear in Visual Studio's startup project list!

#### 2.3 Verify .NET Target Framework
**CRITICAL:** Ensure all projects target `net9.0`, not `net10.0` or other versions.

Check each `.csproj` file:
```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  ...
</PropertyGroup>
```

If any project has `net10.0` or a different version, manually edit the `.csproj` file to fix it.

#### 2.4 Add Project References
```bash
# API references
cd services/{ModuleName}/{ModuleName}.API
```bash
# API references
cd services/{ModuleName}/{ModuleName}.API
dotnet add reference ../{ModuleName}.Application/{ModuleName}.Application.csproj
dotnet add reference ../{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj
dotnet add reference ../{ModuleName}.Contracts/{ModuleName}.Contracts.csproj

# Web references  
cd ../{ModuleName}.Web
dotnet add reference ../{ModuleName}.Application/{ModuleName}.Application.csproj
dotnet add reference ../{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj

# Application references
cd ../{ModuleName}.Application
dotnet add reference ../{ModuleName}.Domain/{ModuleName}.Domain.csproj

# Infrastructure references
cd ../{ModuleName}.Infrastructure
dotnet add reference ../{ModuleName}.Domain/{ModuleName}.Domain.csproj
```

#### 2.5 Install Required NuGet Packages
**CRITICAL:** Verify package versions match .NET 9 compatibility:

```bash
# API - USE VERSION 9.0.0 for EF Core and OpenAPI
cd services/{ModuleName}/{ModuleName}.API
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.0
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 9.0.0

# Infrastructure
cd ../{ModuleName}.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0

# Web - Match shell's MudBlazor version (9.6.0)
cd ../{ModuleName}.Web
dotnet add package MudBlazor --version 9.6.0

# Application
cd ../{ModuleName}.Application
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 9.0.0
```

⚠️ **Common Issue:** Using version 10.x packages will cause build failures! Always use 9.0.x for .NET 9.

---

### Phase 3: Domain Layer

#### 3.1 Create Domain Entities
Create entity classes in `{ModuleName}.Domain/Entities/`:

```csharp
// Example: Product.cs
namespace {ModuleName}.Domain.Entities;

public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string SKU { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int QuantityOnHand { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsActive { get; set; } = true;
}
```

#### 3.2 Create Repository Interfaces
Create in `{ModuleName}.Domain/Interfaces/`:

```csharp
namespace {ModuleName}.Domain.Interfaces;

public interface IProductRepository
{
	Task<IEnumerable<Product>> GetAllAsync();
	Task<Product?> GetByIdAsync(Guid id);
	Task<Product> AddAsync(Product entity);
	Task<Product> UpdateAsync(Product entity);
	Task DeleteAsync(Guid id);
}
```

---

### Phase 4: Infrastructure Layer

#### 4.1 Create DbContext
Create `{ModuleName}DbContext.cs` in `{ModuleName}.Infrastructure/Persistence/`:

```csharp
using Microsoft.EntityFrameworkCore;
using {ModuleName}.Domain.Entities;

namespace {ModuleName}.Infrastructure.Persistence;

public class {ModuleName}DbContext : DbContext
{
	public {ModuleName}DbContext(DbContextOptions<{ModuleName}DbContext> options)
		: base(options)
	{
	}

	public DbSet<Product> Products => Set<Product>();
	// Add other DbSets here

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
			entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
			entity.HasIndex(e => e.SKU).IsUnique();
		});
	}
}
```

#### 4.2 Create Repository Implementations
Create in `{ModuleName}.Infrastructure/Repositories/`:

```csharp
using {ModuleName}.Domain.Entities;
using {ModuleName}.Domain.Interfaces;
using {ModuleName}.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace {ModuleName}.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
	private readonly {ModuleName}DbContext _context;

	public ProductRepository({ModuleName}DbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Product>> GetAllAsync()
		=> await _context.Products.Where(p => p.IsActive).ToListAsync();

	public async Task<Product?> GetByIdAsync(Guid id)
		=> await _context.Products.FindAsync(id);

	public async Task<Product> AddAsync(Product entity)
	{
		entity.Id = Guid.NewGuid();
		entity.CreatedAt = DateTime.UtcNow;
		_context.Products.Add(entity);
		await _context.SaveChangesAsync();
		return entity;
	}

	public async Task<Product> UpdateAsync(Product entity)
	{
		entity.UpdatedAt = DateTime.UtcNow;
		_context.Products.Update(entity);
		await _context.SaveChangesAsync();
		return entity;
	}

	public async Task DeleteAsync(Guid id)
	{
		var entity = await _context.Products.FindAsync(id);
		if (entity != null)
		{
			entity.IsActive = false;
			await _context.SaveChangesAsync();
		}
	}
}
```

#### 4.3 Create Initial Migration
```bash
cd {ModuleName}.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../{ModuleName}.API
```

---

### Phase 5: Application Layer

#### 5.1 Create DTOs
Create in `{ModuleName}.Application/DTOs/`:

```csharp
namespace {ModuleName}.Application.DTOs;

public class ProductDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string SKU { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int QuantityOnHand { get; set; }
}

public class CreateProductRequest
{
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string SKU { get; set; } = string.Empty;
	public decimal Price { get; set; }
}
```

#### 5.2 Create Services
Create in `{ModuleName}.Application/Services/`:

```csharp
using {ModuleName}.Application.DTOs;
using {ModuleName}.Domain.Entities;
using {ModuleName}.Domain.Interfaces;

namespace {ModuleName}.Application.Services;

public interface IProductService
{
	Task<IEnumerable<ProductDto>> GetAllAsync();
	Task<ProductDto?> GetByIdAsync(Guid id);
	Task<ProductDto> CreateAsync(CreateProductRequest request);
	Task<ProductDto> UpdateAsync(Guid id, CreateProductRequest request);
	Task DeleteAsync(Guid id);
}

public class ProductService : IProductService
{
	private readonly IProductRepository _repository;

	public ProductService(IProductRepository repository)
	{
		_repository = repository;
	}

	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		var entities = await _repository.GetAllAsync();
		return entities.Select(MapToDto);
	}

	public async Task<ProductDto?> GetByIdAsync(Guid id)
	{
		var entity = await _repository.GetByIdAsync(id);
		return entity == null ? null : MapToDto(entity);
	}

	public async Task<ProductDto> CreateAsync(CreateProductRequest request)
	{
		var entity = new Product
		{
			Name = request.Name,
			Description = request.Description,
			SKU = request.SKU,
			Price = request.Price
		};

		var created = await _repository.AddAsync(entity);
		return MapToDto(created);
	}

	private static ProductDto MapToDto(Product entity)
	{
		return new ProductDto
		{
			Id = entity.Id,
			Name = entity.Name,
			Description = entity.Description,
			SKU = entity.SKU,
			Price = entity.Price,
			QuantityOnHand = entity.QuantityOnHand
		};
	}

	// Implement other methods...
}
```

#### 5.3 Create Module Registration Service
Create in `{ModuleName}.Application/Services/ModuleRegistrationService.cs`:

```csharp
using {ModuleName}.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace {ModuleName}.Application.Services;

public interface IModuleRegistrationService
{
	Task RegisterWithModuleRegistryAsync();
}

public class ModuleRegistrationService : IModuleRegistrationService
{
	private readonly HttpClient _httpClient;
	private readonly IConfiguration _configuration;

	public ModuleRegistrationService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_configuration = configuration;
	}

	public async Task RegisterWithModuleRegistryAsync()
	{
		var registryUrl = _configuration["ModuleRegistry:Url"] ?? "http://localhost:5100";
		var apiUrl = _configuration["{ModuleName}:ApiBaseUrl"] ?? "http://localhost:50XX";
		var webUrl = _configuration["{ModuleName}:UiEntryPoint"] ?? "http://localhost:50XX";

		var request = new RegisterModuleRequest
		{
			ModuleId = "{modulename}",
			Key = "{modulename}",
			DisplayName = "{ModuleName}",
			Description = "Module description here",
			Version = "1.0.0",
			ApiBaseUrl = apiUrl,
			UiEntryPoint = $"{webUrl}/{modulename}",
			Icon = Icons.Dashboard, // Choose appropriate icon
			Permissions = new List<string> { "{modulename}.read", "{modulename}.write", "{modulename}.admin" },
			Capabilities = new List<string> { "feature1", "feature2" },
			HealthUrl = $"{apiUrl}/health",
			TenantMode = "tenant-per-database",
			SupportsMobile = true,
			MobileUISpecUrl = $"{apiUrl}/api/{modulename}/mobile/ui-spec",
			MobileContractVersion = "1.0.0",
			NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
			{
				new() { Label = "Dashboard", Route = "/{modulename}", Icon = Icons.Dashboard },
				// Add navigation structure here
			}
		};

		try
		{
			var response = await _httpClient.PostAsJsonAsync($"{registryUrl}/api/modules/register", request);
			response.EnsureSuccessStatusCode();
			Console.WriteLine($"✓ Successfully registered {ModuleName} module with Module Registry");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to register with Module Registry: {ex.Message}");
		}
	}

	private static class Icons
	{
		public const string Dashboard = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z\"/>";
		// Add more icon SVG paths as needed
	}
}
```

---

### Phase 6: API Layer

#### 6.1 Create API Controllers
Create in `{ModuleName}.API/Controllers/`:

```csharp
using Microsoft.AspNetCore.Mvc;
using {ModuleName}.Application.DTOs;
using {ModuleName}.Application.Services;

namespace {ModuleName}.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly IProductService _service;

	public ProductsController(IProductService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
	{
		var result = await _service.GetAllAsync();
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<ProductDto>> GetById(Guid id)
	{
		var result = await _service.GetByIdAsync(id);
		if (result == null) return NotFound();
		return Ok(result);
	}

	[HttpPost]
	public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request)
	{
		var result = await _service.CreateAsync(request);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}
}
```

#### 6.2 Create Mobile UI Controller
Create in `{ModuleName}.API/Controllers/MobileUIController.cs`:

```csharp
using {ModuleName}.Contracts.Navigation;
using {ModuleName}.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace {ModuleName}.API.Controllers;

[ApiController]
[Route("api/{modulename}/mobile")]
public class MobileUIController : ControllerBase
{
	[HttpGet("ui-spec")]
	public ActionResult<MobileUISpecification> GetUISpecification()
	{
		var spec = new MobileUISpecification
		{
			ModuleId = "{modulename}",
			ModuleName = "{ModuleName}",
			DisplayName = "{ModuleName}",
			Version = "1.0.0",
			Navigation = GetNavigationMap(),
			Screens = new Dictionary<string, object>
			{
				// Define mobile screens here
			}
		};

		return Ok(spec);
	}

	[HttpGet("navigation")]
	public ActionResult<ModuleNavigationMap> GetNavigation() => Ok(GetNavigationMap());

	private static ModuleNavigationMap GetNavigationMap() => new()
	{
		ModuleId = "{modulename}",
		ModuleName = "{ModuleName}",
		Icon = "dashboard",
		Items = new List<NavigationItem>
		{
			new() { Id = "dashboard", Label = "Dashboard", Icon = "dashboard", Screen = "dashboard", Route = "/{modulename}" },
			// Add mobile navigation items here
		}
	};
}
```

#### 6.3 Configure Program.cs
Update `{ModuleName}.API/Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using {ModuleName}.Application.Services;
using {ModuleName}.Domain.Interfaces;
using {ModuleName}.Infrastructure.Persistence;
using {ModuleName}.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure database
var connectionString = builder.Configuration.GetConnectionString("{ModuleName}Db")
	?? "Server=localhost;Database=BusinessAsUsual_{ModuleName};Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
	options.UseSqlServer(connectionString));

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Add other repositories...

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
// Add other services...

// Register module registration service
builder.Services.AddHttpClient<IModuleRegistrationService, ModuleRegistrationService>();
builder.Services.AddHostedService<ModuleRegistrationHostedService>();

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<{ModuleName}DbContext>();
	db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### 6.3.1 🔥 CRITICAL: Configure In-Memory Database & Seed Test Data

**Problem:** If the API uses SQL Server without proper configuration, it will fail at runtime when the database connection isn't available. The dashboard will appear blank or show only a loading spinner.

**Solution:** Add in-memory database support for development and seed it with test data.

**Step 1:** Add the in-memory package to the API:
```bash
cd services/{ModuleName}/{ModuleName}.API
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 9.0.0
```

**Step 2:** Update `Program.cs` to support in-memory database:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Use in-memory database for development
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
    Console.WriteLine("⚠️  {ModuleName}.API using in-memory database");
    builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
        options.UseInMemoryDatabase("{ModuleName}_API"));
}
else
{
    builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("{ModuleName}Connection")));
}

// ... rest of DI registrations ...

var app = builder.Build();

// Seed in-memory database with test data
if (useInMemory)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<{ModuleName}DbContext>();
        SeedData(context);
    }
}

// ... rest of app configuration ...

app.Run();

static void SeedData({ModuleName}DbContext context)
{
    if (context.{PrimaryEntity}.Any()) return; // Already seeded

    // Create test entities with Guid IDs
    // ⚠️ IMPORTANT: Use Guid.NewGuid() for all IDs, not integers!
    // ⚠️ IMPORTANT: Use the exact property names from your domain entities!

    var entity1 = new {ModuleName}.Domain.Entities.{Entity}
    {
        Id = Guid.NewGuid(),
        Name = "Test Item 1",
        // ... match your entity's properties exactly ...
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    var entity2 = new {ModuleName}.Domain.Entities.{Entity}
    {
        Id = Guid.NewGuid(),
        Name = "Test Item 2",
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    context.{Entities}.AddRange(entity1, entity2);
    context.SaveChanges();

    Console.WriteLine("✅ {ModuleName} database seeded with test data");
}
```

**Why This Matters:**
- Without seed data, dashboard queries return empty results, making the UI appear broken
- The dashboard shows "Loading..." indefinitely if the API isn't running
- In-memory database lets you develop/test without SQL Server configuration
- Console messages help debug whether data was actually seeded

**Validation:**
When you run the API, you should see:
```
⚠️  {ModuleName}.API using in-memory database
✅ {ModuleName} database seeded with test data
```

#### 6.4 Create ModuleRegistrationHostedService
Create in `{ModuleName}.API/Services/`:

```csharp
using {ModuleName}.Application.Services;

namespace {ModuleName}.API.Services;

public class ModuleRegistrationHostedService : IHostedService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<ModuleRegistrationHostedService> _logger;

	public ModuleRegistrationHostedService(
		IServiceProvider serviceProvider,
		ILogger<ModuleRegistrationHostedService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var registrationService = scope.ServiceProvider.GetRequiredService<IModuleRegistrationService>();

		try
		{
			await registrationService.RegisterWithModuleRegistryAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to register module with Module Registry");
		}
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

---

### Phase 7: Mobile Contracts

#### 7.1 Create Navigation Contracts
Create in `{ModuleName}.Contracts/Navigation/ModuleNavigationMap.cs`:

```csharp
namespace {ModuleName}.Contracts.Navigation;

public class ModuleNavigationMap
{
	public string ModuleId { get; set; } = string.Empty;
	public string ModuleName { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
	public List<NavigationItem> Items { get; set; } = new();
}

public class NavigationItem
{
	public string Id { get; set; } = string.Empty;
	public string Label { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
	public string Screen { get; set; } = string.Empty;
	public string? Route { get; set; }
	public List<NavigationItem>? Children { get; set; }
	public bool RequiresPermission { get; set; } = false;
	public string? Permission { get; set; }
}
```

#### 7.2 Create Mobile UI Specification
Create in `{ModuleName}.Contracts/Specifications/MobileUISpecification.cs`:

```csharp
using {ModuleName}.Contracts.Navigation;

namespace {ModuleName}.Contracts.Specifications;

public class MobileUISpecification
{
	public string ModuleId { get; set; } = string.Empty;
	public string ModuleName { get; set; } = string.Empty;
	public string DisplayName { get; set; } = string.Empty;
	public string Version { get; set; } = string.Empty;
	public ModuleNavigationMap Navigation { get; set; } = new();
	public Dictionary<string, object> Screens { get; set; } = new();
}
```

#### 7.3 Update Dashboard Icon Mapping

**CRITICAL:** After creating mobile contracts, update the dashboard to recognize your module's icon.

**Problem:** The web dashboard's module cards may show a generic icon instead of your module's custom icon.

**Solution:** Add your module's icon mapping to `frontend/BusinessAsUsual.Web/Pages/Dashboard.razor` in the `ConvertIconToMudBlazor()` method:

```csharp
string ConvertIconToMudBlazor(string? icon)
{
	if (string.IsNullOrEmpty(icon))
		return Icons.Material.Filled.Apps;

	return icon switch
	{
		"mdi-account-group" => Icons.Material.Filled.People,
		"mdi-currency-usd" => Icons.Material.Filled.AttachMoney,
		"mdi-account-multiple" => Icons.Material.Filled.Contacts,
		"mdi-domain" => Icons.Material.Filled.Business,
		"account_balance" => Icons.Material.Filled.AccountBalance,      // Finance
		"inventory_2" => Icons.Material.Filled.Inventory2,               // Inventory
		"point_of_sale" => Icons.Material.Filled.PointOfSale,            // Sales
		"your_icon_name" => Icons.Material.Filled.YourIcon,              // ← ADD YOUR MODULE HERE
		_ => Icons.Material.Filled.Apps
	};
}
```

**Common Module Icons:**
- **HR:** `mdi-account-group` → `Icons.Material.Filled.People`
- **Finance:** `account_balance` → `Icons.Material.Filled.AccountBalance`
- **CRM:** `mdi-account-multiple` → `Icons.Material.Filled.Contacts`
- **Inventory:** `inventory_2` → `Icons.Material.Filled.Inventory2`
- **Sales:** `point_of_sale` → `Icons.Material.Filled.PointOfSale`
- **Projects:** `assignment` → `Icons.Material.Filled.Assignment`
- **Support:** `support_agent` → `Icons.Material.Filled.SupportAgent`

**Note:** The icon name must match what you set in your `ModuleNavigationMap.cs` (step 7.1).

**Validation:** After this change, your module card on the dashboard (`/dashboard`) should display the correct icon.

---

### Phase 8: Web UI (Blazor)

#### 8.1 Create _Imports.razor (IMPORTANT!)
Create `{ModuleName}.Web/Components/_Imports.razor` to avoid repetitive using statements:

```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MudBlazor
@using {ModuleName}.Web.Components
@using {ModuleName}.Application.DTOs
@using {ModuleName}.Application.Services
```

**Why:** This prevents `@using MudBlazor` errors and reduces boilerplate in every page.

#### 8.2 Standard Page Layout & Breadcrumbs

**CRITICAL:** All module pages must follow this consistent layout pattern:

**Page Structure:**
1. **Container:** `MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0"`
2. **Breadcrumbs:** Manual breadcrumb trail (Dashboard → Module → Page)
3. **Page Header:** H4 with icon and optional subtitle
4. **Action Button:** (optional) Top-right aligned button for primary action
5. **Page Content:** Tables, cards, forms, etc.

**Template for Dashboard Page:**
```razor
@page "/{modulename}"
@using System.Net.Http.Json
@using MudBlazor
@inject IHttpClientFactory HttpClientFactory
@inject NavigationManager Navigation

<PageTitle>{ModuleName} Dashboard</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- BREADCRUMBS -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">{ModuleName}</MudText>
    </div>

    <!-- PAGE HEADER -->
    <MudText Typo="Typo.h4" GutterBottom="true">
        <MudIcon Icon="@Icons.Material.Filled.{ModuleIcon}" Class="mr-2" />
        {ModuleName} Dashboard
    </MudText>
    <MudText Typo="Typo.body1" Color="Color.Secondary" Class="mb-4">
        {Module description}
    </MudText>

    <!-- PAGE CONTENT -->
    <!-- ... dashboard sections ... -->
</MudContainer>
```

**Template for Sub-Pages (e.g., Products, Employees):**
```razor
@page "/{modulename}/subpage"
@using MudBlazor
@inject NavigationManager Navigation

<PageTitle>{SubPage Name}</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- BREADCRUMBS -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudLink Href="/{modulename}">{ModuleName}</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">{SubPage Name}</MudText>
    </div>

    <!-- PAGE HEADER WITH ACTION BUTTON -->
    <div class="d-flex justify-space-between align-center mb-4">
        <div>
            <MudText Typo="Typo.h4" GutterBottom="true">
                <MudIcon Icon="@Icons.Material.Filled.{Icon}" Class="mr-2" />
                {SubPage Name}
            </MudText>
            <MudText Typo="Typo.body1" Color="Color.Secondary">
                {Page description}
            </MudText>
        </div>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add">
            {Primary Action}
        </MudButton>
    </div>

    <!-- PAGE CONTENT -->
    <!-- ... tables, cards, etc. ... -->
</MudContainer>
```

**Breadcrumb Rules:**
- Always start with `/dashboard`
- Use `MudLink` for clickable breadcrumbs
- Use `<span class="mx-2">/</span>` for separators
- Final breadcrumb uses `MudText` with `Color.Primary` (not a link)
- Keep breadcrumbs hierarchical: Dashboard → Module → SubPage → Detail

**⚠️ DEPRECATED - Manual Breadcrumbs:**
The above manual breadcrumb pattern is deprecated. **Instead, use the PageBreadcrumb component** (see section 8.2.1 below).

**Container Rules:**
- **Always** use `MaxWidth.ExtraExtraLarge` (not `ExtraLarge`)
- **Always** use `Class="mt-2 pa-0"` (not `mt-4`)
- This ensures consistent spacing and padding across all modules

#### 8.2.1 Modern Page Layout with UX Enhancements (REQUIRED)

**All new module pages MUST use these modern patterns:**

##### PageBreadcrumb Component
Replace manual breadcrumbs with the PageBreadcrumb component:

```razor
<PageBreadcrumb Items="_breadcrumbItems">
    <Actions>
        <MudButton Variant="Variant.Outlined" Color="Color.Default" StartIcon="@Icons.Material.Filled.FileDownload" Size="Size.Small" OnClick="ExportData">
            Export
        </MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add" Size="Size.Small" OnClick="OpenCreateDialog">
            Add New
        </MudButton>
    </Actions>
</PageBreadcrumb>

@code {
    private List<PageBreadcrumb.BreadcrumbItem> _breadcrumbItems = new()
    {
        new() { Text = "Dashboard", Href = "/dashboard" },
        new() { Text = "Module Name", Href = "/module" },
        new() { Text = "Current Page", Href = "/module/page", Icon = Icons.Material.Filled.PageIcon }
    };
}
```

**Benefits:**
- Consistent breadcrumb styling across all pages
- Built-in dropdown support for navigation
- Actions slot eliminates need for separate action button row
- Reduces page header boilerplate

##### Keyboard Shortcuts Handler
Add keyboard shortcut support to every page:

```razor
@inject IDialogService DialogService
@inject NavigationManager Navigation

<PageTitle>Page Name</PageTitle>
<KeyboardShortcutHandler OnShortcut="HandleKeyboardShortcut" />

@code {
    private async Task HandleKeyboardShortcut(string shortcut)
    {
        switch (shortcut)
        {
            case "alt-n":  // Create/Add
                OpenCreateDialog();
                break;
            case "alt-s":  // Save
                await SaveChanges();
                break;
            case "alt-e":  // Export
                await ExportData();
                break;
            case "alt-f":  // Focus search
                FocusSearchField();
                break;
            case "escape":  // Close dialog
                if (_dialogVisible) CloseDialog();
                break;
            case "question":  // Show help
                await DialogService.ShowAsync<KeyboardShortcutsDialog>("Keyboard Shortcuts");
                break;
            case "g-d":  // Go to dashboard
                Navigation.NavigateTo("/dashboard");
                break;
        }
    }
}
```

**Standard Shortcuts (Alt+ to avoid browser conflicts):**
- `Alt+N` → Create/Add
- `Alt+S` → Save
- `Alt+E` → Export
- `Alt+K` → Command Palette
- `Alt+F` → Focus Search
- `Escape` → Close Dialog
- `?` → Show Help
- `g+d` → Dashboard
- `g+u/r/n/s` → Platform pages

##### Toast Notifications
Use ToastService for all user feedback:

```razor
@inject ToastService Toast

@code {
    private async Task SaveItem()
    {
        try
        {
            // ... save logic ...
            Toast.Saved("Item");
        }
        catch (Exception ex)
        {
            Toast.Error($"Failed to save: {ex.Message}");
        }
    }

    private void DeleteItem(ItemModel item)
    {
        var deleted = item;
        _items.Remove(item);
        Toast.Deleted(item.Name, () =>
        {
            _items.Add(deleted);  // Undo action
            StateHasChanged();
        });
    }
}
```

**Toast Methods:**
- `Toast.Success(message)` - Generic success
- `Toast.Info(message)` - Informational
- `Toast.Warning(message)` - Warnings
- `Toast.Error(message)` - Errors
- `Toast.Created(name)` - Item created
- `Toast.Saved(name)` - Item saved
- `Toast.Deleted(name, undoAction)` - Item deleted with undo

##### Smart Defaults Service
Remember and prefill form values:

```razor
@inject SmartDefaultsService Defaults

@code {
    private void OpenCreateDialog()
    {
        // Get remembered defaults
        var userDefaults = Defaults.GetUserDefaults();

        _newItem = new ItemModel
        {
            IsActive = userDefaults.DefaultActive,
            Status = Defaults.GetValue<string>("item.lastStatus") ?? "Pending",
            Category = Defaults.GetValue<string>("item.lastCategory") ?? ""
        };

        _dialogVisible = true;
    }

    private async Task SaveItem()
    {
        // ... save logic ...

        // Remember selections for next time
        Defaults.RememberValue("item.lastStatus", _newItem.Status);
        Defaults.RememberValue("item.lastCategory", _newItem.Category);
        Defaults.RememberUserFormData(_newItem.IsActive, "", "");
    }
}
```

##### Quick Filter
Add search/filter to all data grids:

```razor
<MudTextField @bind-Value="_searchString"
              Placeholder="Search..."
              Adornment="Adornment.Start"
              AdornmentIcon="@Icons.Material.Filled.Search"
              Immediate="true"
              Class="mb-4" />

<MudDataGrid Items="@FilteredItems">
    <!-- columns -->
</MudDataGrid>

@code {
    private string _searchString = "";

    private IEnumerable<ItemModel> FilteredItems =>
        string.IsNullOrWhiteSpace(_searchString)
            ? _items
            : _items.Where(x =>
                x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase));
}
```

##### Contextual Help
Add inline help and tooltips:

```razor
<!-- Page-level hint -->
@if (_items.Count == 0)
{
    <ContextualHint Title="Getting Started" 
                    Message="Create your first item to get started. Click 'Add New' above." 
                    Dismissible="true" />
}

<!-- Field-level help -->
<HelpTooltip HelpText="This email will be used for system notifications and login">
    <MudTextField @bind-Value="_model.Email" Label="Email Address" />
</HelpTooltip>
```

**UX Enhancement Checklist for All Pages:**
- ✅ PageBreadcrumb with Actions (not manual breadcrumbs)
- ✅ KeyboardShortcutHandler with standard shortcuts
- ✅ ToastService for user feedback
- ✅ SmartDefaultsService for form prefill
- ✅ Quick filter for data grids
- ✅ ContextualHint for empty states
- ✅ HelpTooltip for complex fields
- ✅ LoadingSpinner or skeleton loaders for async operations
- ✅ Smart empty states with helpful guidance

**Reference Implementation:**
See `services/Platform/Platform.Web/Components/Pages/Users.razor` for a complete example with all patterns.

---

#### 8.3 Create Dashboard Page
Create in `{ModuleName}.Web/Components/Pages/Dashboard.razor`:

**CRITICAL - API Client Usage:**
- **ALWAYS** use `@inject IHttpClientFactory HttpClientFactory` (NOT `@inject HttpClient`)
- **ALWAYS** use the named client: `HttpClientFactory.CreateClient("{ModuleName}Api")`
- **NEVER** hardcode API URLs in pages - the base URL is configured in shell `Program.cs`
- All API calls should use **relative paths** (e.g., `"api/inventory/products"`, NOT `"https://localhost:7079/api/inventory/products"`)

**IMPORTANT:** A complete dashboard should include:
1. **Stats/Metrics Row** - Key numbers at the top (total items, value, alerts, etc.)
2. **Navigation Cards Section** - Clickable cards linking to major module features
3. **Quick Actions Card** - Common tasks users perform
4. **Alerts & Notifications Card** - Warnings, status messages
5. **About Module Section** - Description of module capabilities (2/3 width)
6. **Module Info Card** - Technical details: module ID, version, ports, status (1/3 width)

```razor
@page "/{modulename}"
@using System.Net.Http.Json
@using MudBlazor
@inject IHttpClientFactory HttpClientFactory
@inject NavigationManager Navigation

<PageTitle>{ModuleName} Dashboard</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-4">
	<MudText Typo="Typo.h4" GutterBottom="true">{ModuleName} Dashboard</MudText>

	@if (_loading)
	{
		<MudProgressCircular Indeterminate="true" />
		<MudText Class="mt-2">Loading dashboard data...</MudText>
	}
	else if (!string.IsNullOrEmpty(_errorMessage))
	{
		<MudAlert Severity="Severity.Error" Variant="Variant.Filled">
			<MudText>@_errorMessage</MudText>
			<MudText Class="mt-2">Please ensure {ModuleName}.API is running and accessible.</MudText>
		</MudAlert>
	}
	else if (_summary != null)
	{
		<!-- 1. STATS/METRICS ROW -->
		<MudGrid>
			<MudItem xs="12" sm="6" md="3">
				<MudCard Elevation="2">
					<MudCardContent>
						<div class="d-flex justify-space-between">
							<div>
								<MudText Typo="Typo.body2" Color="Color.Secondary">Total Products</MudText>
								<MudText Typo="Typo.h5">@_summary.TotalProducts</MudText>
							</div>
							<MudIcon Icon="@Icons.Material.Filled.Inventory" Size="Size.Large" Color="Color.Primary" />
						</div>
					</MudCardContent>
				</MudCard>
			</MudItem>
			<!-- Add 3-5 more metric cards here -->
		</MudGrid>

		<!-- 2. MODULE NAVIGATION CARDS -->
		<MudGrid Class="mt-6">
			<MudItem xs="12">
				<MudText Typo="Typo.h5" Class="mb-4">{ModuleName} Management</MudText>
			</MudItem>

			<MudItem xs="12" sm="6" md="4">
				<MudCard Elevation="2" Class="pa-4 mud-card-hover" Style="cursor: pointer;" onclick="@(() => NavigateTo("/{modulename}/products"))">
					<MudCardContent>
						<div class="d-flex align-center mb-2">
							<MudIcon Icon="@Icons.Material.Filled.Inventory" Size="Size.Large" Color="Color.Primary" Class="mr-3" />
							<MudText Typo="Typo.h6">Products</MudText>
						</div>
						<MudText Typo="Typo.body2" Color="Color.Secondary">
							Manage product catalog and pricing
						</MudText>
						<MudButton Variant="Variant.Text" Color="Color.Primary" Class="mt-2" Href="/{modulename}/products">
							View Products
						</MudButton>
					</MudCardContent>
				</MudCard>
			</MudItem>
			<!-- Add 5-7 more navigation cards for other major features -->
			<!-- NOTE: Use varied icon colors for visual differentiation - see Icon Color Guidelines in ModifyModule skill -->
			<!-- Example colors: Color.Primary (blue), Color.Success (green), Color.Info (cyan), -->
			<!-- Color.Warning (amber), Color.Tertiary (purple), Color.Secondary (dark gray) -->
		</MudGrid>

		<!-- 3. QUICK ACTIONS & ALERTS -->
		<MudGrid Class="mt-6">
			<MudItem xs="12" md="6">
				<MudCard Elevation="2">
					<MudCardHeader>
						<CardHeaderContent>
							<MudText Typo="Typo.h6">Quick Actions</MudText>
						</CardHeaderContent>
					</MudCardHeader>
					<MudCardContent>
						<MudStack Spacing="2">
							<MudButton Variant="Variant.Text" StartIcon="@Icons.Material.Filled.Add" Href="/{modulename}/products/new" FullWidth="true" Class="justify-start">
								Add New Product
							</MudButton>
							<!-- Add 3-5 more common actions -->
						</MudStack>
					</MudCardContent>
				</MudCard>
			</MudItem>

			<MudItem xs="12" md="6">
				<MudCard Elevation="2">
					<MudCardHeader>
						<CardHeaderContent>
							<MudText Typo="Typo.h6">Alerts & Notifications</MudText>
						</CardHeaderContent>
					</MudCardHeader>
					<MudCardContent>
						@if (_summary.LowStockCount > 0)
						{
							<MudAlert Severity="Severity.Warning" Variant="Variant.Filled" Class="mb-2">
								<MudText>@_summary.LowStockCount items need attention</MudText>
							</MudAlert>
						}
						<!-- Add more conditional alerts -->
					</MudCardContent>
				</MudCard>
			</MudItem>
		</MudGrid>

		<!-- 4. ABOUT MODULE & MODULE INFO -->
		<MudGrid Class="mt-6">
			<MudItem xs="12" md="8">
				<MudPaper Class="pa-4 d-flex flex-column" Elevation="1" Style="height: 100%;">
					<MudText Typo="Typo.h6" GutterBottom="true">
						<MudIcon Icon="@Icons.Material.Filled.Info" Class="mr-2" />
						About {ModuleName} Module
					</MudText>
					<MudText Typo="Typo.body2" Class="mb-4">
						The {ModuleName} module provides comprehensive capabilities including:
					</MudText>
					<MudList T="string" Dense="true">
						<MudListItem T="string" Icon="@Icons.Material.Filled.Check">
							<strong>Feature 1</strong> - Description
						</MudListItem>
						<MudListItem T="string" Icon="@Icons.Material.Filled.Check">
							<strong>Feature 2</strong> - Description
						</MudListItem>
						<!-- Add 4-6 key features -->
					</MudList>
					<MudText Typo="Typo.caption" Color="Color.Secondary" Class="mt-auto pt-4">
						This module is dynamically loaded via the Module Registry Service and provides both web UI and mobile API contracts.
					</MudText>
				</MudPaper>
			</MudItem>

			<MudItem xs="12" md="4">
				<MudPaper Class="pa-4 d-flex flex-column" Elevation="1" Style="height: 100%;">
					<MudText Typo="Typo.h6" GutterBottom="true">
						<MudIcon Icon="@Icons.Material.Filled.Settings" Class="mr-2" />
						Module Info
					</MudText>
					<MudStack Spacing="2" Class="flex-grow-1">
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Module ID</MudText>
							<MudChip T="string" Size="Size.Small" Color="Color.Default" Variant="Variant.Filled">{modulename}</MudChip>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Version</MudText>
							<MudText Typo="Typo.body2">1.0.0</MudText>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">API Port</MudText>
							<MudText Typo="Typo.body2">50XX</MudText>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Web UI Port</MudText>
							<MudText Typo="Typo.body2">50YY</MudText>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Mobile Support</MudText>
							<MudChip T="string" Size="Size.Small" Color="Color.Success" Variant="Variant.Filled">Yes</MudChip>
						</div>
						<div>
							<MudText Typo="Typo.caption" Color="Color.Secondary">Status</MudText>
							<MudChip T="string" Size="Size.Small" Color="Color.Success" Variant="Variant.Filled">Active</MudChip>
						</div>
					</MudStack>
				</MudPaper>
			</MudItem>
		</MudGrid>
	}
</MudContainer>

@code {
	private bool _loading = true;
	private {ModuleName}Summary? _summary;
	private string? _errorMessage;

	protected override async Task OnInitializedAsync()
	{
		try
		{
			var httpClient = HttpClientFactory.CreateClient("{ModuleName}Api");
			_summary = await httpClient.GetFromJsonAsync<{ModuleName}Summary>("/api/{modulename}/dashboard/summary");
		}
		catch (HttpRequestException ex)
		{
			_errorMessage = $"Failed to connect to {ModuleName} API: {ex.Message}";
			Console.WriteLine($"Error loading dashboard: {ex.Message}");
		}
		catch (Exception ex)
		{
			_errorMessage = $"Error loading dashboard: {ex.Message}";
			Console.WriteLine($"Error loading dashboard: {ex.Message}");
		}
		finally
		{
			_loading = false;
		}
	}

	private void NavigateTo(string url)
	{
		Navigation.NavigateTo(url);
	}

	public class {ModuleName}Summary
	{
		public int TotalProducts { get; set; }
		// Add other summary properties
	}
}

<style>
	.mud-card-hover {
		transition: transform 0.2s ease, box-shadow 0.2s ease;
	}

	.mud-card-hover:hover {
		transform: translateY(-4px);
		box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15) !important;
	}
</style>
```

---

#### 8.3.1 Create Sub-Pages with Data Loading

All sub-pages (Products, Employees, etc.) must follow these patterns:

**Using CustomDataGrid Instead of Regular Tables**

The platform provides a reusable `CustomDataGrid` component that wraps MudBlazor's `MudDataGrid` with enhanced features including built-in toolbar, search, filtering, and consistent styling. **Always use CustomDataGrid instead of creating raw MudDataGrid or HTML tables** for listing data.

**Location:** `{ModuleName}.Web/Components/Shared/CustomDataGrid.razor`

**Key Benefits:**
- Built-in search with customizable quick filter
- Automatic toolbar with title and action buttons
- Consistent styling across all modules
- Support for custom toolbar content (filters, dropdowns, etc.)
- All standard MudDataGrid features (sorting, filtering, pagination)

**Complete Example:**

```razor
@page "/{modulename}/products"
@using {ModuleName}.Web.Components.Shared
@using {ModuleName}.Application.DTOs
@inject IProductService ProductService
@inject NavigationManager Navigation
@inject ISnackbar Snackbar

<PageTitle>Products</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- Breadcrumb -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudLink Href="/{modulename}">@ModuleName</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">Products</MudText>
    </div>

    <!-- Header -->
    <MudStack Row="true" AlignItems="AlignItems.Center" Class="mb-4">
        <MudText Typo="Typo.h4" Class="flex-grow-1">
            <MudIcon Icon="@Icons.Material.Filled.Inventory" Class="mr-2" />
            Products
        </MudText>
        <MudButton Variant="Variant.Filled" 
                   Color="Color.Primary" 
                   StartIcon="@Icons.Material.Filled.Add"
                   OnClick="@(() => Navigation.NavigateTo("/{modulename}/products/new"))">
            Add Product
        </MudButton>
    </MudStack>

    @if (_loading)
    {
        <MudProgressCircular Indeterminate="true" />
    }
    else
    {
        <!-- CustomDataGrid with all features -->
        <CustomDataGrid TItem="ProductDto"
                        Items="@_products"
                        Title="Product List"
                        SearchPlaceholder="Search products..."
                        QuickFilterFunc="@((ProductDto p) => 
                            string.IsNullOrEmpty(_searchString) || 
                            (p.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (p.SKU?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false))"
                        Elevation="2">
            <ToolbarContent>
                <!-- Add custom filters or actions in the toolbar -->
                <MudSelect T="string" @bind-Value="_categoryFilter" 
                          Label="Category" 
                          Variant="Variant.Outlined" 
                          Class="ml-4" 
                          Style="min-width: 150px;">
                    <MudSelectItem Value="@("All")">All Categories</MudSelectItem>
                    <MudSelectItem Value="@("Electronics")">Electronics</MudSelectItem>
                    <MudSelectItem Value="@("Furniture")">Furniture</MudSelectItem>
                </MudSelect>
            </ToolbarContent>
            <ChildContent>
                <!-- Define columns using PropertyColumn or TemplateColumn -->
                <PropertyColumn T="ProductDto" TProperty="string" Property="p => p.Name" Title="Product Name">
                    <CellTemplate>
                        <MudText Typo="Typo.body2" Style="font-weight: 500;">@context.Item.Name</MudText>
                    </CellTemplate>
                </PropertyColumn>

                <PropertyColumn T="ProductDto" TProperty="string" Property="p => p.SKU" Title="SKU" />

                <PropertyColumn T="ProductDto" TProperty="decimal" Property="p => p.Price" Title="Price">
                    <CellTemplate>
                        @context.Item.Price.ToString("C2")
                    </CellTemplate>
                </PropertyColumn>

                <PropertyColumn T="ProductDto" TProperty="int" Property="p => p.QuantityOnHand" Title="Stock">
                    <CellTemplate>
                        <MudChip T="string" 
                                Size="Size.Small" 
                                Color="@(context.Item.QuantityOnHand > 10 ? Color.Success : Color.Warning)">
                            @context.Item.QuantityOnHand
                        </MudChip>
                    </CellTemplate>
                </PropertyColumn>

                <PropertyColumn T="ProductDto" TProperty="bool" Property="p => p.IsActive" Title="Status">
                    <CellTemplate>
                        <MudChip T="string" 
                                Size="Size.Small" 
                                Color="@(context.Item.IsActive ? Color.Success : Color.Default)">
                            @(context.Item.IsActive ? "Active" : "Inactive")
                        </MudChip>
                    </CellTemplate>
                </PropertyColumn>

                <!-- Actions column -->
                <TemplateColumn T="ProductDto" Title="Actions" Sortable="false" Filterable="false">
                    <CellTemplate>
                        <MudStack Row="true" Spacing="1">
                            <MudIconButton Icon="@Icons.Material.Filled.Visibility" 
                                          Size="Size.Small" 
                                          Color="Color.Info"
                                          OnClick="@(() => ViewProduct(context.Item.Id))" />
                            <MudIconButton Icon="@Icons.Material.Filled.Edit" 
                                          Size="Size.Small" 
                                          Color="Color.Primary"
                                          OnClick="@(() => EditProduct(context.Item.Id))" />
                            <MudIconButton Icon="@Icons.Material.Filled.Delete" 
                                          Size="Size.Small" 
                                          Color="Color.Error"
                                          OnClick="@(() => DeleteProduct(context.Item.Id))" />
                        </MudStack>
                    </CellTemplate>
                </TemplateColumn>
            </ChildContent>
        </CustomDataGrid>
    }
</MudContainer>

@code {
    private bool _loading = true;
    private List<ProductDto> _products = new();
    private string _searchString = string.Empty;
    private string _categoryFilter = "All";

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        try
        {
            _loading = true;
            _products = (await ProductService.GetAllAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading products: {ex.Message}", Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }

    private void ViewProduct(Guid id) => Navigation.NavigateTo($"/{modulename}/products/{id}");
    private void EditProduct(Guid id) => Navigation.NavigateTo($"/{modulename}/products/{id}/edit");

    private async Task DeleteProduct(Guid id)
    {
        // Add confirmation dialog and delete logic
        await ProductService.DeleteAsync(id);
        await LoadProducts();
        Snackbar.Add("Product deleted successfully", Severity.Success);
    }
}
```

**Important Notes:**
1. **@using directive:** Always add `@using {ModuleName}.Web.Components.Shared` at the top of your page to use CustomDataGrid
2. **TItem parameter:** Must match your DTO type (e.g., `TItem="ProductDto"`)
3. **QuickFilterFunc:** Provides instant client-side search across specified fields
4. **ToolbarContent:** Use for additional filters, dropdowns, or action buttons
5. **PropertyColumn vs TemplateColumn:** Use PropertyColumn for simple data display, TemplateColumn for custom rendering
6. **Don't use raw tables:** Avoid creating `<table>`, `<MudTable>`, or raw `<MudDataGrid>` - always use CustomDataGrid for consistency

**Example: Pro

```razor
@page "/{modulename}/products"
@using System.Net.Http.Json
@using MudBlazor
@inject IHttpClientFactory HttpClientFactory

<PageTitle>Products</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
    <!-- Breadcrumbs (see section 8.2) -->
    <div class="mb-3">
        <MudLink Href="/dashboard">Dashboard</MudLink>
        <span class="mx-2">/</span>
        <MudLink Href="/{modulename}">{ModuleName}</MudLink>
        <span class="mx-2">/</span>
        <MudText Typo="Typo.body2" Inline="true" Color="Color.Primary">Products</MudText>
    </div>

    <!-- Page Header -->
    <div class="d-flex justify-space-between align-center mb-4">
        <div>
            <MudText Typo="Typo.h4" GutterBottom="true">
                <MudIcon Icon="@Icons.Material.Filled.Inventory" Class="mr-2" />
                Products
            </MudText>
            <MudText Typo="Typo.body1" Color="Color.Secondary">
                Manage your product catalog, SKUs, and pricing
            </MudText>
        </div>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add">
            Add Product
        </MudButton>
    </div>

    <!-- Loading / Error / Data States -->
    @if (_loading)
    {
        <MudProgressCircular Indeterminate="true" />
    }
    else if (!string.IsNullOrEmpty(_errorMessage))
    {
        <MudAlert Severity="Severity.Error">@_errorMessage</MudAlert>
    }
    else
    {
        <MudCard>
            <MudCardContent>
                <MudTable Items="@_products" Hover="true" Breakpoint="Breakpoint.Sm" Dense="true">
                    <HeaderContent>
                        <MudTh>SKU</MudTh>
                        <MudTh>Name</MudTh>
                        <MudTh>Price</MudTh>
                        <MudTh>Stock</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd DataLabel="SKU">@context.SKU</MudTd>
                        <MudTd DataLabel="Name">@context.Name</MudTd>
                        <MudTd DataLabel="Price">@context.Price.ToString("C")</MudTd>
                        <MudTd DataLabel="Stock">@context.TotalStock</MudTd>
                    </RowTemplate>
                </MudTable>
            </MudCardContent>
        </MudCard>
    }
</MudContainer>

@code {
    private bool _loading = true;
    private List<ProductDto> _products = new();
    private string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("{ModuleName}Api");
            _products = await client.GetFromJsonAsync<List<ProductDto>>("api/{modulename}/products") ?? new();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error loading products: {ex.Message}";
            Console.WriteLine(_errorMessage);
        }
        finally
        {
            _loading = false;
        }
    }

    // Define DTOs locally or reference from Contracts assembly
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalStock { get; set; }
    }
}
```

**Critical Rules for Sub-Pages:**
1. **Always** use `IHttpClientFactory` - NEVER inject `HttpClient` directly
2. **Always** use the named client: `HttpClientFactory.CreateClient("{ModuleName}Api")`
3. **Always** use relative API paths (e.g., `"api/inventory/products"`)
4. **Always** include loading, error, and data states
5. **Always** follow breadcrumb/padding conventions from section 8.2
6. **Always** add meaningful error messages for debugging

---

#### 8.3.2 Configure Web Program.cs
**IMPORTANT:** {ModuleName}.Web is a **standalone Blazor Web App** that:
- Runs on its own port (e.g., 5008 for Finance, 5002 for HR)
- Can be launched independently for testing
- Also gets embedded/referenced by the main shell (BusinessAsUsual.Web) for integrated navigation

Update `{ModuleName}.Web/Program.cs`:

```csharp
using {ModuleName}.Application.Services;
using {ModuleName}.Domain.Interfaces;
using {ModuleName}.Infrastructure.Persistence;
using {ModuleName}.Infrastructure.Repositories;
using {ModuleName}.Web.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Database configuration - use in-memory for standalone mode
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
	Console.WriteLine("⚠️  {ModuleName}.Web using in-memory database");
	builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
		options.UseInMemoryDatabase("{ModuleName}_Web"));
}
else
{
	var connectionString = builder.Configuration.GetConnectionString("{ModuleName}Database") 
		?? "Server=localhost;Database=BusinessAsUsual_{ModuleName};Trusted_Connection=True;TrustServerCertificate=True;";
	builder.Services.AddDbContext<{ModuleName}DbContext>(options =>
		options.UseSqlServer(connectionString));
}

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Add other repositories...

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
// Add other services...

// Named HTTP client for API calls (optional, if Web needs to call API)
var apiUrl = builder.Configuration["{ModuleName}Service:Url"] ?? "http://localhost:50XX";
builder.Services.AddHttpClient("{ModuleName}Api", client =>
{
	client.BaseAddress = new Uri(apiUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
```

#### 8.4 Configure Web launchSettings.json
Create `{ModuleName}.Web/Properties/launchSettings.json`:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
	"http": {
	  "commandName": "Project",
	  "dotnetRunMessages": true,
	  "launchBrowser": false,
	  "applicationUrl": "http://localhost:50XX",
	  "environmentVariables": {
		"ASPNETCORE_ENVIRONMENT": "Development"
	  }
	}
  }
}
```

**Port Assignment Guide:**
- 5000: Main shell (BusinessAsUsual.Web)
- 5001: Finance.API
- 5002: HR.Web
- 5003: CRM.Web
- 5004: CRM.API
- 5008: Finance.Web
- 5041: HR.API
- 5100: ModuleRegistry.API
- Choose an available port in the 5000-5100 range for your module

#### 8.5 Configure Web Project File
Ensure `{ModuleName}.Web/{ModuleName}.Web.csproj` has:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <ItemGroup>
	<ProjectReference Include="..\{ModuleName}.Application\{ModuleName}.Application.csproj" />
	<ProjectReference Include="..\{ModuleName}.Infrastructure\{ModuleName}.Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
	<PackageReference Include="MudBlazor" Version="9.6.0" />
  </ItemGroup>

  <PropertyGroup>
	<TargetFramework>net9.0</TargetFramework>
	<Nullable>enable</Nullable>
	<ImplicitUsings>enable</ImplicitUsings>
	<StaticWebAssetBasePath>_content/{ModuleName}.Web</StaticWebAssetBasePath>
  </PropertyGroup>

  <ItemGroup>
	<!-- Exclude bootstrap from static web assets to avoid conflicts with parent shell -->
	<Content Remove="wwwroot\lib\bootstrap\**" />
	<None Include="wwwroot\lib\bootstrap\**">
	  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
	</None>
  </ItemGroup>

</Project>
```

⚠️ **Key Settings:**
- `Sdk="Microsoft.NET.Sdk.Web"` - NOT Razor SDK!
- `StaticWebAssetBasePath` - prevents asset conflicts when embedded in shell
- Bootstrap exclusion - prevents duplicate static files when referenced by main app

---

### Phase 9: Frontend Integration

#### 9.1 Add Web Project Reference to Main Shell
Add your module's Web project to `frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj`:

```xml
<ItemGroup>
  <!-- ... existing references ... -->
  <ProjectReference Include="..\..\services\Finance\Finance.Web\Finance.Web.csproj" />
  <ProjectReference Include="..\..\services\{ModuleName}\{ModuleName}.Web\{ModuleName}.Web.csproj" />
</ItemGroup>
```

Also add to the publish exclusion filter in the same file:

```xml
<Target Name="RemoveDuplicateReferencedWebContent" AfterTargets="ComputeFilesToPublish">
  <ItemGroup>
    <ResolvedFileToPublish Remove="@(ResolvedFileToPublish)"
      Condition="( $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/HR.Web/'))
                   Or $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/CRM.Web/'))
                   Or $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/Finance.Web/'))
                   Or $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/{ModuleName}.Web/')) )
                 And ( $([System.String]::Copy('%(FullPath)').Replace('\','/').Contains('/wwwroot/'))
                       Or $([System.String]::Copy('%(Filename)').StartsWith('appsettings')) )" />
  </ItemGroup>
</Target>
```

This prevents duplicate wwwroot/appsettings files during publish.

**ALSO ADD** to the static web assets build filter (to prevent asset conflicts during build):

```xml
<!-- Also remove duplicate static web assets during build -->
<Target Name="RemoveDuplicateStaticWebAssets" BeforeTargets="GenerateStaticWebAssetsManifest">
  <ItemGroup>
    <StaticWebAsset Remove="@(StaticWebAsset)" 
      Condition="( $([System.String]::Copy('%(SourceId)').Equals('HR.Web'))
                   Or $([System.String]::Copy('%(SourceId)').Equals('CRM.Web'))
                   Or $([System.String]::Copy('%(SourceId)').Equals('Finance.Web'))
                   Or $([System.String]::Copy('%(SourceId)').Equals('{ModuleName}.Web')) )
                 And ( $([System.String]::Copy('%(RelativePath)').StartsWith('lib/'))
                       Or $([System.String]::Copy('%(OriginalItemSpec)').Contains('appsettings')) )" />
  </ItemGroup>
</Target>
```

#### 9.1.1 🔥 CRITICAL: Add Module Assembly to App.razor

**Problem:** Even with the project reference, Blazor won't discover your module's Razor components.

**Why:** Blazor's Router needs to know which assemblies to scan for `@page` components.

**Solution:** Add your module assembly to `frontend/BusinessAsUsual.Web/App.razor`:

```csharp
@code {
    private readonly Assembly[] _additionalAssemblies = new[]
    {
        typeof(HR.Web.Components.App).Assembly,
        typeof(CRM.Web.Components.App).Assembly,
        typeof(Finance.Web.Components.App).Assembly,
        typeof({ModuleName}.Web.Components.App).Assembly  // ← ADD THIS LINE
    };
}
```

**Validation:** After this change, navigating to `/{modulename}/*` routes should load your module's Razor pages.

**Note:** The `App` class must exist in your module's `Components` folder. If you created it in a different location, adjust the namespace accordingly.

#### 9.2 Update ModuleDiscoveryService Fallback
Add your module to `frontend/BusinessAsUsual.Web/Services/ModuleDiscoveryService.cs` in the `GetFallbackModules()` method:

```csharp
new ModuleDto
{
	ModuleId = "{modulename}",
	Key = "{modulename}",
	DisplayName = "{ModuleName}",
	Description = "Module description",
	UiEntryPoint = "/{modulename}",
	Icon = Icons.Material.Filled.Dashboard, // Choose appropriate icon
	IsActive = true,
	NavigationItems = new List<NavigationItemDto>
	{
		new() { Label = "Dashboard", Route = "/{modulename}", Icon = Icons.Material.Filled.Dashboard },
		new() 
		{ 
			Label = "Group Name", 
			Route = "/{modulename}/path", 
			Icon = Icons.Material.Filled.Inventory,
			ExpandedByDefault = false,
			Children = new List<NavigationItemDto>
			{
				new() { Label = "Submenu 1", Route = "/{modulename}/path1", Icon = Icons.Material.Filled.List },
				new() { Label = "Submenu 2", Route = "/{modulename}/path2", Icon = Icons.Material.Filled.Add }
			}
		},
		new() { Label = "Reports", Route = "/{modulename}/reports", Icon = Icons.Material.Filled.Analytics }
	}
}
```

#### 9.2.1 🔥 CRITICAL: Register Module Route in MainLayout.razor.cs

**Problem:** Even if the module is in the sidebar and AdditionalAssemblies, the **sidebar won't appear** when you navigate to the module because `_currentModule` stays null.

**Why:** The shell's `MainLayout.razor.cs` has hardcoded module route detection that needs updated for each new module.

**Solution:** Add your module to the legacy route detection in `frontend/BusinessAsUsual.Web/Components/Layout/MainLayout.razor.cs`:

Find the `UpdateModuleFromUri` method (around line 182) and add your module:

```csharp
// Legacy hardcoded routes
if (path.StartsWith("/hr"))
	_currentModule = "HR";
else if (path.StartsWith("/finance"))
	_currentModule = "Finance";
else if (path.StartsWith("/crm"))
	_currentModule = "CRM";
else if (path.StartsWith("/{modulename}"))
	_currentModule = "{ModuleName}";  // ← ADD THIS LINE
else if (path.StartsWith("/timekeeping"))
	_currentModule = "Timekeeping";
// ... rest of the conditions
```

**Validation:** After this change, navigating to `/{modulename}` should show the sidebar with your module's navigation.

**Note:** The `_currentModule` string must match your module's `DisplayName` from `ModuleDiscoveryService.cs`.

#### 9.2.2 🔥 CRITICAL: Register Module HttpClient in Shell (if module calls its API)

**Problem:** Module pages call the API via HttpClient, but when running inside the shell you get "An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set."

**Why:** The module's `Program.cs` registers a named HttpClient (e.g., "SalesApi") for standalone operation, but when the module runs embedded in the shell, it uses the **shell's DI container**, which doesn't have that registration.

**Solution:** Register the module's named HttpClient in the shell's `frontend/BusinessAsUsual.Web/Program.cs`:

Find the HttpClient registration section (around line 105-118) and add your module's client:

```csharp
// Register named HttpClient for the Inventory microservice
var inventoryServiceUrl = builder.Configuration["InventoryService:Url"] ?? "http://localhost:5142";
builder.Services.AddHttpClient("InventoryApi", client =>
{
    client.BaseAddress = new Uri(inventoryServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register named HttpClient for the {ModuleName} microservice
var {modulename}ServiceUrl = builder.Configuration["{ModuleName}Api:Url"] ?? "http://localhost:50XX";
builder.Services.AddHttpClient("{ModuleName}Api", client =>
{
    client.BaseAddress = new Uri({modulename}ServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

**Important:** The named client string (e.g., "SalesApi") must **exactly match** what your module pages use in `HttpClientFactory.CreateClient("SalesApi")`.

**Configuration:** Add the API URL to `frontend/BusinessAsUsual.Web/appsettings.json`:

```json
{
  "SalesApi": {
    "Url": "http://localhost:5143"
  }
}
```

**Validation:** After this change, module pages should successfully call their API when running through the shell.

**Note:** This is only required for modules that use HttpClient to call their API. Modules that inject Application services directly (like HR) don't need this step.

#### 9.3 Add to Visual Studio Solution (if not done in Step 2.2)
If you haven't already added projects to the solution:

Right-click solution → Add → Existing Project, add all 7 projects.

⚠️ **Double-check:** All projects should appear in Solution Explorer. If any are missing, they weren't added correctly in step 2.2.

#### 9.4 Configure Multiple Startup Projects
**CRITICAL:** Set up multi-project startup so all services run together:

1. Right-click solution → **Properties**
2. Select **Multiple startup projects**
3. Set **Action = Start** for:
   - `ModuleRegistry.API`
   - `{ModuleName}.API`
   - `{ModuleName}.Web` ← **Important!** This allows standalone testing
   - `BusinessAsUsual.Web` (main shell)
   - Any other module APIs you need running (Finance.API, HR.API, etc.)

4. **Click OK**

Now pressing F5 will start all projects together!

**Testing Modes:**
- **Integrated:** Navigate to `http://localhost:5000` (main shell) and click your module in the sidebar
- **Standalone:** Navigate directly to `http://localhost:50XX` (your Web app's port) to test in isolation

---

### Phase 10: Testing & Validation

#### 10.1 Create Unit Tests
Create in `{ModuleName}.Tests/Unit/`:

```csharp
using Xunit;
using Moq;
using {ModuleName}.Application.Services;
using {ModuleName}.Domain.Interfaces;

public class ProductServiceTests
{
	[Fact]
	public async Task GetAllAsync_ReturnsAllProducts()
	{
		// Arrange
		var mockRepo = new Mock<IProductRepository>();
		mockRepo.Setup(r => r.GetAllAsync())
			.ReturnsAsync(new List<Product> { /* test data */ });

		var service = new ProductService(mockRepo.Object);

		// Act
		var result = await service.GetAllAsync();

		// Assert
		Assert.NotNull(result);
		mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
	}
}
```

#### 10.2 Validation Checklist
- [ ] API starts without errors
- [ ] Database migrations apply successfully
- [ ] Module registers with Module Registry
- [ ] Navigation appears in sidebar
- [ ] Mobile UI contract endpoint returns valid JSON
- [ ] CRUD operations work correctly
- [ ] Dashboard loads with data
- [ ] All navigation links work
- [ ] Build succeeds with no warnings
- [ ] Unit tests pass

---

## Port Numbers Reference

⚠️ **ALWAYS consult the authoritative port registry FIRST:**

📖 **See:** `docs/PORT_REGISTRY.md` - Complete list of all assigned ports

**Quick Reference (verify against PORT_REGISTRY.md):**

| Module | API Port (HTTP) | API Port (HTTPS) | Web UI Port | HttpClient Name |
|--------|-----------------|------------------|-------------|-----------------|
| ModuleRegistry | 5100 | 7100 | — | — |
| HR | 5041 | 7171 | 5002 | `HrApi` |
| Finance | 5007 | — | 5008 | `FinanceApi` |
| Inventory | 5142 | 7079 | 5009 | `InventoryApi` |
| Sales | 5143 | 7143 | 5293 | `SalesApi` |
| CRM | 5004 | — | 5005 | `CrmApi` |
| Services | 7286 | 7285 | 61172 | `ServicesApi` |
| AI | 5300 | — | — | `AiApi` |
| **{New Module}** | **5XXX** | **7XXX** | **5XXX** | **`{ModuleName}Api`** |

### Port Assignment Rules:
1. **Reserve port in PORT_REGISTRY.md BEFORE creating launchSettings.json**
2. **Update PORT_REGISTRY.md immediately after assignment**
3. **Register named HttpClient in shell's Program.cs** (see CRITICAL SETUP CHECKLIST)
4. **Use the exact same port** in:
   - API's `launchSettings.json`
   - Shell's `Program.cs` HttpClient registration
   - `PORT_REGISTRY.md` documentation

---

## Common Material Icons
```csharp
Icons.Material.Filled.Dashboard
Icons.Material.Filled.Inventory
Icons.Material.Filled.ShoppingCart
Icons.Material.Filled.LocalShipping
Icons.Material.Filled.Campaign
Icons.Material.Filled.Support
Icons.Material.Filled.Build
Icons.Material.Filled.Assignment
Icons.Material.Filled.Description
Icons.Material.Filled.VerifiedUser
Icons.Material.Filled.Warehouse
Icons.Material.Filled.Category
Icons.Material.Filled.Store
Icons.Material.Filled.Sell
```

---

## Common Issues & Troubleshooting

### Build Failures

#### Issue: "Program does not contain a static 'Main' method"
**Cause:** Project SDK mismatch or missing Program.cs  
**Fix:** 
- Ensure `.csproj` has `<Project Sdk="Microsoft.NET.Sdk.Web">` (NOT Razor SDK)
- Verify `Program.cs` exists in the Web project

#### Issue: "CS0246: The type or namespace name 'HttpContext' could not be found"
**Cause:** Standalone Blazor app trying to use ASP.NET Core types  
**Fix:** Remove or conditionally compile code that uses `HttpContext` in component libraries

#### Issue: "Conflicting assets with the same target path 'lib/bootstrap/...'"
**Cause:** Multiple Web projects shipping the same wwwroot files  
**Fix:** 
1. Add your module to the exclusion filter in `BusinessAsUsual.Web.csproj`
2. Add bootstrap exclusion to your Web project (see step 8.4)

#### Issue: Package downgrade warning (e.g., MudBlazor 9.7.0 to 9.6.0)
**Cause:** Version mismatch between main app and module  
**Fix:** Use MudBlazor 9.6.0 in all Web projects to match the shell

#### Issue: "The type or namespace name 'EF/OpenApi/etc.' could not be found"
**Cause:** Using .NET 10 packages with .NET 9 project  
**Fix:** 
```bash
# Downgrade to 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.0
```

#### Issue: Projects restore as net10.0 even though .csproj says net9.0
**Cause:** Template defaults or cached restore  
**Fix:**
1. Manually edit each `.csproj` to verify `<TargetFramework>net9.0</TargetFramework>`
2. Clean and rebuild: `dotnet clean && dotnet build`

### Runtime/Navigation Issues

#### Issue: "Nothing at this address" when clicking module link
**Possible Causes & Fixes:**

1. **Web project not added to shell reference**
   - Add `<ProjectReference Include="..\..\services\{ModuleName}\{ModuleName}.Web\{ModuleName}.Web.csproj" />` to `BusinessAsUsual.Web.csproj`

2. **Web project not in startup projects**
   - Configure Multiple Startup Projects (see step 9.4)
   - Set {ModuleName}.Web action to "Start"

3. **Route mismatch**
   - Verify `@page "/{modulename}"` in Dashboard.razor matches the route in ModuleDiscoveryService fallback navigation

4. **Web app not running**
   - Check Output window for startup errors
   - Verify port is not already in use

#### Issue: Module doesn't appear in sidebar
**Cause:** Missing from ModuleDiscoveryService fallback  
**Fix:** Add module entry to `GetFallbackModules()` in `ModuleDiscoveryService.cs` (see step 9.2)

#### Issue: Module appears in sidebar but clicking does nothing
**Cause:** Missing `@using MudBlazor` in Razor pages  
**Fix:** Add `@using MudBlazor` to each `.razor` file, or add to `_Imports.razor`

#### Issue: Sidebar menu items stay expanded instead of collapsed
**Cause:** Missing `ExpandedByDefault = false` on navigation groups  
**Fix:** Add `ExpandedByDefault = false` to parent navigation items with children

#### Issue: Dashboard shows stats/alerts but no navigation cards
**Cause:** Module dashboard missing navigation card section  
**Fix:** Add a grid with MudCards linking to each major module feature (Products, Warehouses, etc.). See Inventory Dashboard.razor for example.

#### Issue: No sidebar visible when navigating to module pages
**Possible Causes & Fixes:**

1. **Wrong browser URL/port**
   - **Symptom:** You see topbar/footer but no sidebar; page works but feels "isolated"
   - **Cause:** Browser navigated to module's standalone port (e.g., `localhost:5009`) instead of shell port
   - **Fix:** Always navigate through the shell app. Check the URL bar - it should be the shell's port (typically 5001 or 5000), not the module's port.
   - **Prevention:** Don't bookmark or directly visit module standalone URLs when developing integrated features

2. **Module using its own MainLayout**
   - **Symptom:** Module pages render with different layout than other modules
   - **Cause:** Module's App.razor or pages explicitly specify layout
   - **Fix:** Module pages should NOT specify `@layout` directive. They should inherit the shell's MainLayout automatically when loaded via the shell router.
   - **Note:** Module can have its own MainLayout for standalone development, but pages should not force it

3. **Module not properly registered in shell router**
   - **Fix:** Verify module assembly is in shell's `App.razor` `AdditionalAssemblies` array

**Best Practice:** When developing module features, always access them through the shell sidebar navigation, not by typing the module's standalone URL directly.

#### Issue: "An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set"
**Symptom:** Module pages load but data loading fails with HttpClient errors  
**Cause:** Module uses a named HttpClient to call its API, but the shell doesn't have that HttpClient registered in its DI container  
**Fix:** Register the module's named HttpClient in the shell's `Program.cs` (see Phase 9.2.2):
```csharp
var salesServiceUrl = builder.Configuration["SalesApi:Url"] ?? "http://localhost:5143";
builder.Services.AddHttpClient("SalesApi", client =>
{
    client.BaseAddress = new Uri(salesServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
```
**Important:** The client name must match what the module pages use in `HttpClientFactory.CreateClient("SalesApi")`

### Visual Studio Issues

#### Issue: Module not appearing in startup project list
**Cause:** Projects not added to solution  
**Fix:** 
```bash
cd "D:\DotNet Projects\BusinessAsUsual"
dotnet sln add services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj
# ... repeat for all 7 projects
```

#### Issue: "Project not found" error when adding reference
**Cause:** Wrong relative path  
**Fix:** Always use `../{ProjectName}/{ProjectName}.csproj` for sibling projects

### Database/EF Core Issues

#### Issue: Migrations fail to create
**Cause:** Missing EF tools or wrong startup project  
**Fix:**
```bash
dotnet tool install --global dotnet-ef
cd services/{ModuleName}/{ModuleName}.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../{ModuleName}.API
```

#### Issue: "No database provider configured"
**Cause:** DbContext not registered in DI  
**Fix:** Verify `builder.Services.AddDbContext<{ModuleName}DbContext>()` exists in both API and Web `Program.cs`

#### Issue: "Unable to determine the relationship represented by navigation 'Order.CustomFields' of type 'Dictionary<string, string>'"
**Cause:** EF Core cannot automatically map dictionary properties as relationships  
**Fix:** Configure the dictionary to be stored as JSON in the database using value conversion:
```csharp
// In OnModelCreating method of DbContext
modelBuilder.Entity<Order>(entity =>
{
    // ... other configurations ...

    // Configure CustomFields as JSON column
    entity.Property(e => e.CustomFields)
        .HasColumnType("nvarchar(max)")
        .HasConversion(
            v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
            v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
        );
});
```
**Alternative:** If the dictionary isn't needed for queries, mark it `[NotMapped]` or use `Ignore()` in fluent configuration.

### MudBlazor/Blazor Issues

#### Issue: "Generic type 'MudList<T>' requires 1 type argument"
**Cause:** MudBlazor version incompatibility  
**Fix:** Replace `MudList<MudListItem>` with simpler structures like `MudStack` + `MudButton`

#### Issue: "Cannot infer type for 'T' in MudChip"
**Cause:** Missing explicit type parameter  
**Fix:** Change `<MudChip>` to `<MudChip T="string">`

#### Issue: Icons.Material.Filled not recognized
**Cause:** Missing MudBlazor using  
**Fix:** Add `@using MudBlazor` at top of .razor file

#### Issue: "The following routes are ambiguous: 'counter' in 'Inventory.Web.Components.Pages.Counter' 'counter' in 'Sales.Web.Components.Pages.Counter'"
**Cause:** Multiple module web projects have demo/template pages (Counter, Weather, Home) with conflicting routes when loaded into the shell's router via `AdditionalAssemblies`  
**Fix:** Remove all template demo pages from module web projects:
```bash
# Remove from each module: HR, Sales, Inventory, etc.
rm services/{ModuleName}/{ModuleName}.Web/Components/Pages/Counter.razor
rm services/{ModuleName}/{ModuleName}.Web/Components/Pages/Weather.razor
```

#### Issue: "The following routes are ambiguous: '' in 'BusinessAsUsual.Web.Pages.Home' '' in 'Sales.Web.Components.Pages.Home'"
**Cause:** Module web project has a template Home.razor page with `@page "/"` that conflicts with the shell's root Home page  
**Fix:** Remove the module's Home.razor if it's just a template demo, or change its route to be module-specific (e.g., `@page "/sales/home"`)
```bash
rm services/{ModuleName}/{ModuleName}.Web/Components/Pages/Home.razor
```
**Root Cause:** When creating new Blazor projects with templates, the template includes demo pages (Home, Counter, Weather). These must be removed from module projects before integrating into the shell to prevent route conflicts.

### Module Registration Issues

#### Issue: Module shows as offline in registry
**Cause:** Registration service not running or wrong URL  
**Fix:** 
1. Verify ModuleRegistry.API is running on port 5100
2. Check `appsettings.json` has correct `ModuleRegistry:Url`
3. Check API Output window for registration errors

#### Issue: Mobile UI contract returns 404
**Cause:** Missing MobileUIController or wrong route  
**Fix:** Ensure controller has `[Route("api/[controller]")]` and `[HttpGet("navigation")]` attributes

---

## Tips & Best Practices

1. **Naming Conventions**: Use PascalCase for projects, classes; camelCase for fields/parameters
2. **DTOs vs Entities**: Always map between them, never expose entities directly
3. **Async/Await**: Use async methods for all I/O operations
4. **Error Handling**: Return appropriate HTTP status codes (404, 400, 500)
5. **Logging**: Add logging to services for debugging
6. **Validation**: Use Data Annotations or FluentValidation
7. **Navigation**: Keep menu structure flat (max 2 levels deep)
8. **Mobile Contracts**: Update both registration and mobile UI controller
9. **Testing**: Write unit tests for business logic, integration tests for APIs
10. **Documentation**: Add XML comments to public methods

---

### Phase 11: Docker Deployment Configuration

After creating and testing your module locally, add Docker support for containerized deployment.

#### 11.1 Create Dockerfile for API
Create `services/{ModuleName}/{ModuleName}.API/Dockerfile`:

```dockerfile
# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the {ModuleName} dependency graph) for a cached restore
COPY services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj services/{ModuleName}/{ModuleName}.API/
COPY services/{ModuleName}/{ModuleName}.Application/{ModuleName}.Application.csproj services/{ModuleName}/{ModuleName}.Application/
COPY services/{ModuleName}/{ModuleName}.Contracts/{ModuleName}.Contracts.csproj services/{ModuleName}/{ModuleName}.Contracts/
COPY services/{ModuleName}/{ModuleName}.Domain/{ModuleName}.Domain.csproj services/{ModuleName}/{ModuleName}.Domain/
COPY services/{ModuleName}/{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj services/{ModuleName}/{ModuleName}.Infrastructure/

# Restore
# Retry the isolated restore to survive transient NuGet connectivity on small
# EC2 instances (a flaky network otherwise fails restore with exit code 82).
RUN for i in 1 2 3; do \
	  dotnet restore services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj && break; \
	  if [ "$i" = "3" ]; then echo "restore failed after 3 attempts"; exit 1; fi; \
	  echo "restore attempt $i failed; retrying in 10s..."; sleep 10; \
	done

# Copy source
COPY services/{ModuleName}/{ModuleName}.API services/{ModuleName}/{ModuleName}.API
COPY services/{ModuleName}/{ModuleName}.Application services/{ModuleName}/{ModuleName}.Application
COPY services/{ModuleName}/{ModuleName}.Contracts services/{ModuleName}/{ModuleName}.Contracts
COPY services/{ModuleName}/{ModuleName}.Domain services/{ModuleName}/{ModuleName}.Domain
COPY services/{ModuleName}/{ModuleName}.Infrastructure services/{ModuleName}/{ModuleName}.Infrastructure

# Publish
RUN dotnet publish services/{ModuleName}/{ModuleName}.API/{ModuleName}.API.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# curl is used by the container HEALTHCHECK below
RUN apt-get update && apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
	CMD curl -fsS http://localhost:80/health || exit 1

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "{ModuleName}.API.dll"]
```

**Key Features:**
- **Multi-stage build** - Smaller final image (SDK vs Runtime)
- **Retry logic** - Handles transient NuGet failures
- **Health check** - Monitors container health
- **Dependency order** - Copies projects in dependency graph order for efficient layer caching

#### 11.2 Create Dockerfile for Web
Create `services/{ModuleName}/{ModuleName}.Web/Dockerfile`:

```dockerfile
# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the {ModuleName} dependency graph) for a cached restore
COPY services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj services/{ModuleName}/{ModuleName}.Web/
COPY services/{ModuleName}/{ModuleName}.Application/{ModuleName}.Application.csproj services/{ModuleName}/{ModuleName}.Application/
COPY services/{ModuleName}/{ModuleName}.Domain/{ModuleName}.Domain.csproj services/{ModuleName}/{ModuleName}.Domain/
COPY services/{ModuleName}/{ModuleName}.Infrastructure/{ModuleName}.Infrastructure.csproj services/{ModuleName}/{ModuleName}.Infrastructure/

# Restore
RUN dotnet restore services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj

# Copy source
COPY services/{ModuleName}/{ModuleName}.Web services/{ModuleName}/{ModuleName}.Web
COPY services/{ModuleName}/{ModuleName}.Application services/{ModuleName}/{ModuleName}.Application
COPY services/{ModuleName}/{ModuleName}.Domain services/{ModuleName}/{ModuleName}.Domain
COPY services/{ModuleName}/{ModuleName}.Infrastructure services/{ModuleName}/{ModuleName}.Infrastructure

# Publish
RUN dotnet publish services/{ModuleName}/{ModuleName}.Web/{ModuleName}.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "{ModuleName}.Web.dll"]
```

#### 11.3 Test Docker Build Locally
Before pushing, test the Docker builds locally:

```bash
# Build API image
docker build -f services/{ModuleName}/{ModuleName}.API/Dockerfile -t {modulename}-api:latest .

# Build Web image
docker build -f services/{ModuleName}/{ModuleName}.Web/Dockerfile -t {modulename}-web:latest .

# Run API container
docker run -d -p 5007:80 --name {modulename}-api {modulename}-api:latest

# Run Web container
docker run -d -p 5008:80 --name {modulename}-web {modulename}-web:latest

# Test the containers
curl http://localhost:5007/health
curl http://localhost:5008

# Clean up
docker stop {modulename}-api {modulename}-web
docker rm {modulename}-api {modulename}-web
```

#### 11.4 Add Module to docker-compose.heavy.yml
After creating the Dockerfiles, add your module services to the orchestration file `docker-compose.heavy.yml`:

```yaml
  {modulename}-api:
    build:
      context: .
      dockerfile: services/{ModuleName}/{ModuleName}.API/Dockerfile
    image: bau/{modulename}-api:latest
    container_name: bau-{modulename}-api
    restart: unless-stopped
    networks:
      - bau-heavy
    ports:
      - "50XX:80"  # Choose an available port number
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Production}
      - ASPNETCORE_URLS=http://+:80
      # Shared RDS (MS SQL) connection string - set in .env on the instance
      - AWS_SQL_CONNECTION_STRING=${AWS_SQL_CONNECTION_STRING}
      # Module Registry URL for service discovery
      - ModuleRegistry__Url=${MODULE_REGISTRY_URL}
    deploy:
      resources:
        limits:
          memory: 512M

  {modulename}-web:
    build:
      context: .
      dockerfile: services/{ModuleName}/{ModuleName}.Web/Dockerfile
    image: bau/{modulename}-web:latest
    container_name: bau-{modulename}-web
    restart: unless-stopped
    networks:
      - bau-heavy
    ports:
      - "50YY:80"  # Choose an available port number
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Production}
      - ASPNETCORE_URLS=http://+:80
      # Module Web calls Module API over the internal compose network
      - {ModuleName}Api__Url=http://{modulename}-api:80
    depends_on:
      - {modulename}-api
    deploy:
      resources:
        limits:
          memory: 384M
```

**Port Assignments (update header comment too):**
- CRM: 5004 (API), 5005 (Web)
- HR: 5041 (API), 5002 (Web)
- Finance: 5006 (API), 5009 (Web)
- Inventory: 5010 (API), 5011 (Web)
- **Your module:** Choose the next available port numbers

**Important:**
- Add your module to the header comment section listing all services
- Ensure port numbers don't conflict with existing services
- Memory limits: 512M for API, 384M for Web (typical for module services)
- Use the internal Docker network name for inter-service communication

**Critical Configuration Alignment:**
The environment variable name in docker-compose MUST match the configuration key in your Web project's Program.cs:

```yaml
# docker-compose.heavy.yml
environment:
  - {ModuleName}Api__Url=http://{modulename}-api:80
  # Note: Double underscore (__) in environment variable = colon (:) in .NET configuration
```

```csharp
// {ModuleName}.Web/Program.cs
var apiUrl = builder.Configuration["{ModuleName}Api:Url"] ?? "http://localhost:50XX";
builder.Services.AddHttpClient("{ModuleName}Api", client =>
{
    client.BaseAddress = new Uri(apiUrl);
});
```

**Common Mistake:**
```csharp
// ❌ WRONG - Key mismatch
var apiUrl = builder.Configuration["{ModuleName}Service:Url"] ?? "http://localhost:50XX";
// Docker sets "{ModuleName}Api__Url" but code reads "{ModuleName}Service:Url"
// Result: Cannot assign requested address (localhost:50XX) error in Docker
```

```csharp
// ✅ CORRECT - Keys match
var apiUrl = builder.Configuration["{ModuleName}Api:Url"] ?? "http://localhost:50XX";
// Docker sets "{ModuleName}Api__Url" which maps to "{ModuleName}Api:Url" in config
// Result: Web successfully calls http://{modulename}-api:80 over Docker network
```

**Pattern Examples:**
- Finance: `FinanceApi:Url` ← `FinanceApi__Url`
- CRM: `CrmApi:Url` ← `CrmApi__Url`
- Inventory: `InventoryApi:Url` ← `InventoryApi__Url`

#### 11.5 Update CI/CD Pipeline
If you have a CI/CD pipeline (GitHub Actions, Azure DevOps, etc.), add build steps for the new module's Docker images.

**Important Notes:**
- Dockerfiles must be run from the **solution root directory** (not from the module folder) because they reference relative paths
- The COPY commands expect the full relative path from the solution root
- Don't add Dockerfiles until the module is fully integrated and tested in the main shell

---

## Code Quality Standards

### Documentation Requirements

**CRITICAL:** All public properties, methods, and classes MUST have XML documentation comments.

#### ✅ Good Examples:

```csharp
/// <summary>
/// Represents a product in the inventory system.
/// </summary>
public class Product
{
	/// <summary>
	/// Gets or sets the unique identifier for the product.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the product name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the current stock quantity.
	/// </summary>
	public int QuantityOnHand { get; set; }
}

/// <summary>
/// Service for managing product inventory operations.
/// </summary>
public class ProductService : IProductService
{
	/// <summary>
	/// Retrieves all products from the inventory.
	/// </summary>
	/// <returns>A collection of all products.</returns>
	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		// Implementation
	}

	/// <summary>
	/// Creates a new product in the inventory.
	/// </summary>
	/// <param name="request">The product creation request containing product details.</param>
	/// <returns>The newly created product.</returns>
	/// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
	public async Task<ProductDto> CreateAsync(CreateProductRequest request)
	{
		// Implementation
	}
}
```

#### ❌ Bad Examples (Missing Documentation):

```csharp
// BAD - No documentation
public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; }
}

// BAD - No documentation on public methods
public class ProductService
{
	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		// ...
	}
}
```

#### Enable Documentation Warnings

Add to your `.csproj` files to enforce documentation:

```xml
<PropertyGroup>
	<GenerateDocumentationFile>true</GenerateDocumentationFile>
	<NoWarn>$(NoWarn);1591</NoWarn> <!-- Remove this to enforce doc comments -->
</PropertyGroup>
```

To enforce documentation and fail builds on missing comments, remove the `NoWarn` suppression.

---

## Lessons Learned: Deployment Troubleshooting

This section captures common issues encountered during deployment and their solutions.

### MudBlazor Attribute Casing Issues

**Problem:** Build warnings like `MUD0002: Illegal Attribute 'Title' on 'MudIconButton' using pattern 'LowerCase'`

**Cause:** MudBlazor analyzer enforces lowercase attribute names for standard HTML attributes.

**Solution:**
- Change `Title="..."` to `title="..."` on `MudIconButton`
- Change `PanelClass="..."` to `Class="..."` on `MudTabs`
- Change `Dense="true"` - Remove this attribute entirely from `MudTimePicker` (not supported)

**Example Fixes:**
```razor
<!-- BEFORE (Warning) -->
<MudIconButton Icon="@Icons.Material.Filled.Edit" Title="Edit" />
<MudTabs PanelClass="pa-4">...</MudTabs>

<!-- AFTER (Fixed) -->
<MudIconButton Icon="@Icons.Material.Filled.Edit" title="Edit" />
<MudTabs Class="pa-4">...</MudTabs>
```

### Missing Component References

**Problem:** Warning `RZ10012: Found markup element with unexpected name 'CustomDataGrid'. If this is intended to be a component, add a @using directive for its namespace.`

**Cause:** Razor compiler can't find the component because the namespace isn't imported.

**Solution:** Add `@using {ModuleName}.Web.Components.Shared` at the top of the .razor file.

```razor
@page "/hr/benefits"
@using HR.Web.Components.Shared  <!-- Add this line -->
@rendermode InteractiveServer

<!-- Now CustomDataGrid can be used -->
<CustomDataGrid TItem="BenefitDto" Items="@_benefits">
	...
</CustomDataGrid>
```

**Alternative:** Add to `_Imports.razor` to apply to all pages:
```razor
@using {ModuleName}.Web.Components.Shared
```

### Null Reference Warnings in Production Builds

**Problem:** Warning `CS8604: Possible null reference argument for parameter` in Release builds but not Debug.

**Cause:** Nullable reference types enabled, and the compiler detects potential null values being passed to non-nullable parameters.

**Solution:** Add null checks before using nullable variables:

```csharp
// BEFORE (Warning)
await Service.UpdateAsync(_config);

// AFTER (Fixed)
if (_config != null)
{
	await Service.UpdateAsync(_config);
}
```

**Best Practice:** Always check nullable variables before use, or use null-coalescing operators:
```csharp
var name = customer?.Name ?? "Unknown";
```

### Async Method Without Await

**Problem:** Warning `CS1998: This async method lacks 'await' operators and will run synchronously.`

**Cause:** Method is marked `async` but doesn't contain any `await` calls.

**Solution:** Either add async operations or use `Task.FromResult` for synchronous methods that need to return `Task<T>`:

```csharp
// BEFORE (Warning)
private async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchText)
{
	return _employees.Where(e => e.Name.Contains(searchText));
}

// AFTER (Fixed Option 1 - Use Task.FromResult)
private Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchText)
{
	return Task.FromResult(_employees.Where(e => e.Name.Contains(searchText)));
}

// AFTER (Fixed Option 2 - Remove async if not needed)
private IEnumerable<EmployeeDto> SearchEmployees(string searchText)
{
	return _employees.Where(e => e.Name.Contains(searchText));
}
```

### Module Reference in Docker Build Context

**Problem:** Error `CS0246: The type or namespace name 'Inventory' could not be found` during Docker build.

**Cause:** Module project isn't included in the Docker build context or hasn't been pushed to the repository yet.

**Solution (Temporary):** Comment out the module reference in `App.razor` and `.csproj` until the module is deployed:

```razor
<!-- App.razor -->
@code {
	private readonly Assembly[] _additionalAssemblies = new[]
	{
		typeof(HR.Web.Components.App).Assembly,
		typeof(CRM.Web.Components.App).Assembly,
		typeof(Finance.Web.Components.App).Assembly
		// typeof(Inventory.Web.Components.App).Assembly  // TODO: Re-enable when deployed
	};
}
```

```xml
<!-- BusinessAsUsual.Web.csproj -->
<ItemGroup>
	<ProjectReference Include="..\..\services\HR\HR.Web\HR.Web.csproj" />
	<ProjectReference Include="..\..\services\CRM\CRM.Web\CRM.Web.csproj" />
	<ProjectReference Include="..\..\services\Finance\Finance.Web\Finance.Web.csproj" />
	<!-- <ProjectReference Include="..\..\services\Inventory\Inventory.Web\Inventory.Web.csproj" /> -->
</ItemGroup>
```

**Permanent Solution:** Create Dockerfiles for the new module (see Phase 11) and include them in your deployment pipeline.

### Generic Type Inference in MudBlazor Components

**Problem:** Error `RZ10001: The type of component 'MudChip' cannot be inferred based on the values provided.`

**Cause:** Some MudBlazor components require explicit type parameter `T`.

**Solution:** Add `T="string"` (or appropriate type) to the component:

```razor
<!-- BEFORE (Error) -->
<MudChip Size="Size.Small" Color="Color.Primary">New</MudChip>

<!-- AFTER (Fixed) -->
<MudChip T="string" Size="Size.Small" Color="Color.Primary">New</MudChip>
```

### MudBlazor TemplateColumn Type Inference Failure

**Problem:** Error `CS0411: The type arguments for method 'TemplateColumn<T>' cannot be inferred from the usage.`

**Cause:** When using `TemplateColumn` inside `MudDataGrid` or `CustomDataGrid`, Razor's type inference can fail even when the parent grid has `TItem` specified. This commonly occurs with explicit `<TemplateColumn>` markup inside grids.

**Solution:** Add explicit type parameter `T="YourDto"` to the `TemplateColumn`:

```razor
<!-- BEFORE (Compilation Error CS0411) -->
<CustomDataGrid TItem="OrderDto" Items="@_orders">
    <Columns>
        <PropertyColumn Property="x => x.OrderNumber" Title="Order #" />
        <TemplateColumn Title="Customer">  <!-- ERROR HERE -->
            <CellTemplate>
                @context.CustomerName
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</CustomDataGrid>

<!-- AFTER (Fixed) -->
<CustomDataGrid TItem="OrderDto" Items="@_orders">
    <Columns>
        <PropertyColumn Property="x => x.OrderNumber" Title="Order #" />
        <TemplateColumn T="OrderDto" Title="Customer">  <!-- Added T="OrderDto" -->
            <CellTemplate>
                @context.CustomerName
            </CellTemplate>
        </TemplateColumn>
    </Columns>
</CustomDataGrid>
```

**Real-World Examples:**

From Sales.Web Orders page:
```razor
<TemplateColumn T="OrderDto" Title="Status">
    <CellTemplate>
        <MudChip T="string" Size="Size.Small" Color="@GetStatusColor(context.Item.Status)">
            @context.Item.Status
        </MudChip>
    </CellTemplate>
</TemplateColumn>
```

From Sales.Web Quotes page:
```razor
<TemplateColumn T="QuoteDto" Title="Actions">
    <CellTemplate>
        <MudIconButton Icon="@Icons.Material.Filled.Send" title="Send" Size="Size.Small" />
        <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" title="Accept" Size="Size.Small" />
    </CellTemplate>
</TemplateColumn>
```

**Best Practice:** Always specify the type parameter on `TemplateColumn` when using it inside data grids to avoid inference ambiguity.

### Missing Interactive Server Components Registration

**Problem:** Runtime error `System.InvalidOperationException: Unable to find a provider for the render mode: Microsoft.AspNetCore.Components.Server.InternalServerRenderMode. This generally means that a call to 'AddInteractiveWebAssemblyComponents' or 'AddInteractiveServerComponents' is missing.`

**Cause:** Module's `Program.cs` is using the old Blazor Server registration pattern (`.AddServerSideBlazor()`) instead of the new .NET 8/9 Razor Components pattern.

**Solution:** Update your module's `Program.cs` to use the new registration pattern:

```csharp
// ❌ OLD PATTERN (Blazor Server - will cause runtime error)
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// ...
app.MapBlazorHub();

// ✅ NEW PATTERN (.NET 8/9 Razor Components - correct)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// ...
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
```

**Complete Working Example:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Blazor services - NEW .NET 8/9 pattern
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// ... other service registrations ...

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

**Note:** Do NOT mix old (`AddServerSideBlazor()`) and new (`.AddInteractiveServerComponents()`) patterns in the same project.

### Build vs. Runtime Environment Differences

**Problem:** Code builds fine locally but fails in Docker/CI pipeline.

**Common Causes:**
1. **Case-sensitive file paths** - Linux containers are case-sensitive, Windows is not
2. **Missing files** - .gitignore might exclude necessary files
3. **Environment-specific configuration** - Missing environment variables or connection strings
4. **Package restore issues** - Network timeouts in CI environment

**Solutions:**
1. Always use correct casing in file references: `"MyFile.cs"` not `"myfile.cs"`
2. Check `.gitignore` and `.dockerignore` for excluded files
3. Use `appsettings.Production.json` for deployment-specific settings
4. Add retry logic to Docker restores (see Phase 11.1 Dockerfile example)

### Testing Checklist Before Deployment

Before pushing changes that will trigger a deployment pipeline:

- [ ] All projects build successfully in **Release** configuration
- [ ] No compiler warnings (treat warnings as errors in production)
- [ ] All unit tests pass
- [ ] Manual testing completed in integrated mode (via shell)
- [ ] All public members have XML doc comments
- [ ] No `TODO` or `HACK` comments in committed code
- [ ] appsettings.Production.json configured (if needed)
- [ ] Dockerfiles created and tested locally (Phase 11)
- [ ] Module properly registered in shell references

**Build Release Mode Locally:**
```bash
dotnet build -c Release
dotnet test -c Release
```

### AWS Deployment: Disk Space Management

**Problem:** Docker build fails with `No space left on device` error during NuGet restore or file operations.

**Cause:** The default 8GB EBS volume is too small for building multiple .NET 9 microservices simultaneously. Each module's Docker build can consume 1-2GB during the build process (NuGet packages, intermediate layers, build cache).

**Example Error:**
```
error : No space left on device : '/root/.nuget/packages/...'
System.IO.IOException: No space left on device
```

**Immediate Cleanup (Temporary Fix):**
```sh
# Connect to your EC2 instance
# Check current disk usage
df -h /

# Stop all containers
sudo docker compose -f docker-compose.heavy.yml down

# Remove all unused Docker artifacts
sudo docker system prune -af --volumes

# Check space freed
df -h /
```

This typically frees 2-4GB but is only a temporary solution.

**Permanent Solution - Resize EBS Volume:**

You MUST resize the EBS volume to at least 20-30GB for sustainable operation with 4+ modules.

**Step 1: Resize in AWS Console**
1. Navigate to **EC2 Console** → **Volumes**
2. Select the volume attached to your heavy instance (currently 8 GiB)
3. **Actions** → **Modify Volume**
4. Change **Size** from `8` to `30` (30 GB recommended)
5. Click **Modify** → **Yes** to confirm
6. Wait 2-5 minutes for state to change to "optimizing"

**Step 2: Extend the Filesystem (on the EC2 instance)**

After AWS shows the volume as modified, SSH to your instance and run:

```sh
# Extend the partition to use new space
sudo growpart /dev/nvme0n1 1

# For XFS filesystem (Amazon Linux 2023 default)
sudo xfs_growfs /

# For ext4 filesystem (older AMIs)
sudo resize2fs /dev/nvme0n1p1

# Verify the new size is available
df -h /
```

You should now see ~30GB total instead of 8GB.

**Step 3: Rebuild Docker Containers**

```sh
# Navigate to repo (adjust path if different)
cd /home/ec2-user/BusinessAsUsual

# Pull latest code
git pull

# Rebuild and start all services
sudo docker compose -f docker-compose.heavy.yml up -d --build
```

**Why 30GB?**
- **8GB**: Too small - fills up during multi-service builds ❌
- **20GB**: Minimum for 4 modules, tight headroom ⚠️
- **30GB**: Recommended - room for logs, cache, future modules ✅
- **Cost**: ~$3/month for 30GB gp3 volume (minimal increase from 8GB)

**Important Notes:**
- Docker builds all services in parallel by default, consuming disk space simultaneously
- NuGet package cache (`/root/.nuget/packages/`) can grow to several GB
- Docker layer cache improves rebuild speed but consumes disk space
- Regular `docker system prune -af` cleanup is still recommended monthly

**User Account Context:**
- You may SSH as `ssm-user` (AWS Systems Manager) or `ec2-user`
- Repository is typically located at `/home/ec2-user/BusinessAsUsual`
- Docker commands require `sudo` privileges
- To switch users: `sudo su - ec2-user`
- To run commands as another user: Use full paths with sudo (e.g., `sudo docker compose -f /home/ec2-user/BusinessAsUsual/docker-compose.heavy.yml up -d`)

---

## Next Steps After Creation

1. Add authentication & authorization
2. Implement role-based permissions
3. Add audit logging
4. Create reports and analytics
5. Implement search and filtering
6. Add export functionality (Excel, PDF)
7. Create mobile app screens
8. Add real-time notifications
9. Implement webhooks/integrations
10. Performance optimization

---

## Example: Quick Inventory Module Creation

When you say "Let's create an Inventory module", this skill will:
1. ✅ Confirm the domain (Inventory Management)
2. ✅ Design entities (Product, Warehouse, StockItem, etc.)
3. ✅ Create 7 projects following the structure
4. ✅ Implement domain layer with entities
5. ✅ Build infrastructure with EF Core
6. ✅ Create application services
7. ✅ Build REST API with controllers
8. ✅ Register with Module Registry
9. ✅ Create mobile contracts
10. ✅ Build Blazor dashboard and pages
11. ✅ Integrate into main app navigation
12. ✅ Validate everything works

**Estimated time**: 2-4 hours for a complete, functional module

---

## Questions to Ask Before Starting
- What is the module name?
- What are the 3-5 core entities?
- What's the primary workflow/user journey?
- Should it integrate with existing modules?
- Mobile support required?
- Any special compliance/security requirements?
