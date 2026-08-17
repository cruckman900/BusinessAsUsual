# Known Issues & Technical Debt

## ✅ RESOLVED: PageBreadcrumb Migration - Remaining Pages (6)

**Status:** ✅ **COMPLETED** - All pages converted  
**Completed:** January 2025

### Background
Completed architectural shift from `PageHeaderService` (shared state) to `PageBreadcrumb` component (isolated, auto-cleanup). All 14 pages successfully converted, infrastructure removed.

### Pages Converted ✅
All following pages have been successfully converted to PageBreadcrumb:

1. ✅ `frontend/BusinessAsUsual.Web/Modules/LMS/Index.razor`
2. ✅ `frontend/BusinessAsUsual.Web/Modules/HR/Index.razor`
3. ✅ `frontend/BusinessAsUsual.Web/Modules/HR/Pages/EmployeeDirectory.razor`
4. ✅ `frontend/BusinessAsUsual.Web/Modules/HR/Pages/EmployeeDetail.razor`
5. ✅ `frontend/BusinessAsUsual.Web/Modules/HR/Pages/NewHireOnboarding.razor`
6. ✅ `frontend/BusinessAsUsual.Web/Modules/HR/Pages/BenefitsManagement.razor`

### Additional Fixes Applied ✅
- ✅ Removed `HeaderService` references from `MainLayout.razor.cs`
- ✅ Fixed cross-assembly reference in `services/HR/HR.Web/Components/Pages/EmployeeDetail.razor`
- ✅ Fixed `Dashboard.razor` by removing obsolete `NoHeaderPageBase` inheritance
- ✅ Fixed field initializer error in `EmployeeDetail.razor` by moving breadcrumb init to `OnParametersSet`
- ✅ Registered `IEventBus` in `Program.cs` for LMS command handlers (fixes runtime DI error)
- ✅ Added `Microsoft.EntityFrameworkCore.Sqlite` package to `BusinessAsUsual.Web` for LMS database support
- ✅ Added LMS database seeding on application startup (development environment)
- ✅ Updated `CourseManagement` page to load real courses from database instead of hardcoded GUID

### Build Status ✅
**Shell build:** ✅ Clean (0 errors)  
**Solution build:** ✅ Clean (0 errors)

### Conversion Pattern (Copy-Paste Ready)

For each file:

#### 1. Remove these lines:
```razor
@inject PageHeaderService HeaderService
```

#### 2. Remove these code blocks:
```razor
protected override void OnAfterRender(bool firstRender)
{
	if (firstRender)
	{
		HeaderService.SetHeader(BuildHeader());
	}
}

private RenderFragment BuildHeader() => __builder =>
{
	<PageHeader ... />
};
```

#### 3. Add at top:
```razor
@using Platform.Web.Components.Shared

<PageTitle>[Page Name] - [Module]</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="mt-2 pa-0">
	<PageBreadcrumb Items="_breadcrumbItems" />

	<div class="d-flex justify-space-between align-center mb-4">
		<div>
			<MudText Typo="Typo.h4" GutterBottom="true">
				<MudIcon Icon="@Icons.Material.Filled.[Icon]" Class="mr-2" />
				[Page Title]
			</MudText>
			<MudText Typo="Typo.body1" Color="Color.Secondary">[Description]</MudText>
		</div>
	</div>
```

#### 4. Close container before @code:
```razor
</MudContainer>

@code {
```

#### 5. Add breadcrumb definition in @code:
```razor
private List<PageBreadcrumb.BreadcrumbItem> _breadcrumbItems = new()
{
	new() { Text = "Dashboard", Href = "/dashboard" },
	new() { Text = "[Module Name]", Href = "/[route]", Icon = Icons.Material.Filled.[Icon] },
	new() { Text = "[Page Name]", Href = "/[full-route]" }
};
```

### Reference Examples (Working)
- `frontend/BusinessAsUsual.Web/Modules/LMS/Pages/MyCourses.razor`
- `frontend/BusinessAsUsual.Web/Modules/LMS/Pages/Admin/Dashboard.razor`
- `services/HR/HR.Web/Components/Pages/EmployeeList.razor` (gold standard)

---

---

## ✅ RESOLVED: BusinessAsUsual.Admin - UseSqlite Error
**File:** `frontend/BusinessAsUsual.Admin/Extensions/StartupExtensions.cs:48`  
**Error:** `CS1061: 'DbContextOptionsBuilder' does not contain a definition for 'UseSqlite'`  
**Status:** ✅ **RESOLVED** - Changed to `UseSqlServer` with SQL Server LocalDB connection string  
**Fix Applied:** Changed HR database from SQLite to SQL Server to match Admin app's existing infrastructure

---

**Last Updated:** January 2025  
**Documented By:** Copilot (during pre-existing build errors cleanup)
