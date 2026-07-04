# HR Module Routes - Complete Reference

**Date**: 2025-01-XX  
**Status**: ✅ Standardized

## Overview

All HR module routes now follow the `/hr/*` pattern (no `/modules/` prefix). This ensures consistency with sidebar navigation and eliminates confusion about route ownership.

## Route Standardization Summary

### ✅ Changes Made
- **Old Pattern**: `/modules/hr/*` (inconsistent, confusing)
- **New Pattern**: `/hr/*` (clean, consistent)
- **Layout**: All pages use `MainLayout` (no more `IframeLayout`)

---

## Complete HR Route Map

### Core Pages

| Page | Route(s) | Description |
|------|----------|-------------|
| **Home** | `/hr`<br>`/hr/home` | HR module landing page with quick access cards |
| **Employees** | `/hr/employees` | Employee directory and management |
| **Departments** | `/hr/departments` | Department organization structure |
| **Onboarding** | `/hr/onboarding` | New hire onboarding workflows |
| **Benefits** | `/hr/benefits` | Benefits administration and enrollment |
| **Reports** | `/hr/reports` | Analytics dashboard with report cards |
| **Settings** | `/hr/settings` | HR module configuration |

### Employee Management

| Page | Route(s) | Description |
|------|----------|-------------|
| **Employee Detail** | `/hr/employees/{id}` | View individual employee details |
| **Create Employee** | `/hr/employees/create` | Create new employee record |
| **Edit Employee** | `/hr/employees/{id}/edit` | Edit existing employee |

### Department Management

| Page | Route(s) | Description |
|------|----------|-------------|
| **Department Detail** | `/hr/departments/{id}` | View department details and org chart |
| **Create Department** | `/hr/departments/create` | Create new department |
| **Edit Department** | `/hr/departments/{id}/edit` | Edit department settings |

### Reports & Analytics

| Page | Route(s) | Description |
|------|----------|-------------|
| **Reports Dashboard** | `/hr/reports` | Main analytics landing with report cards |
| **Compensation** | `/hr/compensation` | Salary analysis and pay equity reports |
| **Time Off** | `/hr/timeoff` | PTO requests and balance tracking |
| **Performance** | `/hr/performance` | Performance review analytics |
| **Training** | `/hr/training` | Training completion and compliance |
| **Turnover** | `/hr/turnover` | Employee retention and exit analysis |
| **Diversity** | `/hr/diversity` | D&I metrics and representation goals |
| **Headcount** | `/hr/headcount` | Budget utilization and hiring pipeline |
| **Org Chart** | `/hr/orgchart` | Organization hierarchy visualization |
| **New Hire Analytics** | `/hr/newhires` | Onboarding effectiveness and cohort analysis |
| **Headcount Detail** | `/hr/reports/headcount-detail` | Extended headcount report with drill-down |

### Demo/Test Pages

| Page | Route(s) | Description |
|------|----------|-------------|
| **Counter** | `/hr/counter` | Demo counter component |
| **Weather** | `/hr/weather` | Demo weather forecast |

---

## Navigation Structure

### Sidebar Menu (from `ModuleDiscoveryService`)

```csharp
{
	Label = "Home",
	Route = "/hr",
	Icon = Icons.Material.Filled.Home
}
{
	Label = "Employees",
	Route = "/hr/employees",
	Icon = Icons.Material.Filled.People
}
{
	Label = "Departments",
	Route = "/hr/departments",
	Icon = Icons.Material.Filled.Business
}
{
	Label = "Onboarding",
	Route = "/hr/onboarding",
	Icon = Icons.Material.Filled.PersonAdd
}
{
	Label = "Benefits",
	Route = "/hr/benefits",
	Icon = Icons.Material.Filled.CardGiftcard
}
{
	Label = "Reports",
	Route = "/hr/reports",
	Icon = Icons.Material.Filled.Assessment
}
```

---

## Files Modified

### Route Changes (17 files)

**Core Pages** (7 files):
- ✅ `Counter.razor` → `/hr/counter`
- ✅ `Home.razor` → `/hr` + `/hr/home`
- ✅ `Employees.razor` → `/hr/employees`
- ✅ `Departments.razor` → `/hr/departments`
- ✅ `Settings.razor` → `/hr/settings`
- ✅ `Weather.razor` → `/hr/weather`
- ✅ `HeadcountReport.razor` → `/hr/reports/headcount-detail`

**Employee Management** (3 files):
- ✅ `EmployeeDetail.razor` → `/hr/employees/{id}`
- ✅ `EmployeeForm.razor` → `/hr/employees/create` + `/hr/employees/{id}/edit`
- ✅ `Employees.razor` navigation methods updated

**Department Management** (3 files):
- ✅ `DepartmentDetail.razor` → `/hr/departments/{id}`
- ✅ `DepartmentForm.razor` → `/hr/departments/create` + `/hr/departments/{id}/edit`
- ✅ `Departments.razor` navigation methods updated

**Report Pages** (10 files - already correct, verified):
- Reports.razor, Compensation.razor, TimeOff.razor, Performance.razor, Training.razor, Turnover.razor, Diversity.razor, Headcount.razor, OrgChart.razor, NewHireAnalytics.razor

**Benefits & Onboarding** (2 files - already correct, verified):
- Benefits.razor, Onboarding.razor (removed IframeLayout)

### Layout Changes (1 file)
- ✅ `Onboarding.razor` → Removed `@layout IframeLayout`

### Navigation Service (1 file)
- ✅ `ModuleDiscoveryService.cs` → Updated fallback routes and UiEntryPoint

---

## Migration Notes

### What Changed
1. **Route Prefix**: Removed `/modules/` from all HR page routes
2. **Entry Point**: Updated module entry point from `/modules/hr` to `/hr`
3. **Layout**: Removed last `IframeLayout` usage from Onboarding
4. **Navigation Calls**: Updated all `Navigation.NavigateTo()` calls in list pages

### Breaking Changes
None - this was an internal standardization. All navigation now works consistently through the sidebar.

### Benefits
- ✅ **Consistent routing**: All `/hr/*` routes
- ✅ **Single layout**: All pages use MainLayout
- ✅ **Clear ownership**: `/hr/*` clearly belongs to HR module
- ✅ **Sidebar alignment**: Navigation items match actual routes
- ✅ **No confusion**: Eliminated `/modules/hr/*` vs `/hr/*` inconsistency

---

## Route Patterns

### Primary Routes (Main Menu)
```
/hr                    → Home (dashboard with cards)
/hr/employees          → Employee list
/hr/departments        → Department list
/hr/onboarding         → Onboarding workflow
/hr/benefits           → Benefits administration
/hr/reports            → Reports dashboard
/hr/settings           → Module settings
```

### Detail Routes (Accessed from lists)
```
/hr/employees/{id}              → Employee detail view
/hr/employees/create            → Create employee form
/hr/employees/{id}/edit         → Edit employee form

/hr/departments/{id}            → Department detail view
/hr/departments/create          → Create department form
/hr/departments/{id}/edit       → Edit department form
```

### Report Routes (Accessed from dashboard)
```
/hr/compensation               → Compensation report
/hr/timeoff                    → Time off report
/hr/performance                → Performance report
/hr/training                   → Training report
/hr/turnover                   → Turnover report
/hr/diversity                  → Diversity report
/hr/headcount                  → Headcount report
/hr/orgchart                   → Org chart report
/hr/newhires                   → New hire analytics
/hr/reports/headcount-detail   → Extended headcount report
```

---

## Future Considerations

### Route Additions
When adding new HR pages:
1. **Always use `/hr/*` pattern** - never `/modules/hr/*`
2. **Use MainLayout** - no special layout directive needed
3. **Update sidebar** - add navigation item to `ModuleDiscoveryService.cs` if needed
4. **Follow convention**:
   - List pages: `/hr/{entity-plural}`
   - Detail pages: `/hr/{entity-plural}/{id}`
   - Create pages: `/hr/{entity-plural}/create`
   - Edit pages: `/hr/{entity-plural}/{id}/edit`
   - Report pages: `/hr/{report-name}` or `/hr/reports/{report-name}`

### Route Registry
When Module Registry is re-enabled, ensure it returns:
```json
{
	"moduleId": "hr",
	"uiEntryPoint": "/hr",
	"navigationItems": [
		{ "label": "Home", "route": "/hr" },
		{ "label": "Employees", "route": "/hr/employees" },
		{ "label": "Departments", "route": "/hr/departments" },
		{ "label": "Onboarding", "route": "/hr/onboarding" },
		{ "label": "Benefits", "route": "/hr/benefits" },
		{ "label": "Reports", "route": "/hr/reports" }
	]
}
```

---

## Testing Checklist

After route standardization, verify:

- [x] Sidebar navigation items all work correctly
- [x] All main pages load with proper layout
- [x] Employee list → detail → edit → back navigation works
- [x] Department list → detail → edit → back navigation works
- [x] Reports dashboard → individual report → back button works
- [x] All pages use MainLayout (consistent shell/sidebar)
- [x] No 404 errors on any HR routes
- [x] Browser back/forward buttons work correctly
- [x] Active route highlighting in sidebar works

---

## Related Documentation
- [HR Module Layout Fix Success](./archive/HR_MODULE_LAYOUT_FIX_SUCCESS.md) - Layout migration from IframeLayout to MainLayout _(archived)_
- [Architecture Fix: No Iframes](./archive/ARCHITECTURE_FIX_NO_IFRAMES.md) - Clarification that the app doesn't use iframes _(archived)_
- [Navigation Integration Fix](./archive/NAVIGATION_INTEGRATION_FIX.md) - Sidebar and reports dashboard implementation _(archived)_

---

**Status**: ✅ All HR routes standardized and verified working  
**Next Steps**: Continue adding features using the `/hr/*` pattern
