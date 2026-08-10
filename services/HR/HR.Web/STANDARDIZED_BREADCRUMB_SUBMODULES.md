# Standardized HR Breadcrumb SubModules

This file documents the standardized SubModules list that should be used on ALL HR pages for the "HR" breadcrumb item.

## Pattern (matching Platform gold standard):

```csharp
new() 
{ 
	Text = "HR", 
	Href = "/hr",
	Icon = Icons.Material.Filled.People,
	SubModules = new List<PageBreadcrumb.SubModuleItem>
	{
		new() { Text = "HR Dashboard", Href = "/hr", Icon = Icons.Material.Filled.Dashboard, Color = Color.Default },
		new() { Text = "Employees", Href = "/hr/employees", Icon = Icons.Material.Filled.People, Color = Color.Primary },
		new() { Text = "Departments", Href = "/hr/departments", Icon = Icons.Material.Filled.AccountTree, Color = Color.Success },
		new() { Text = "Recruiting", Href = "/hr/recruiting-hub", Icon = Icons.Material.Filled.PersonSearch, Color = Color.Info },
		new() { Text = "Performance", Href = "/hr/performance-hub", Icon = Icons.Material.Filled.TrendingUp, Color = Color.Warning },
		new() { Text = "Training", Href = "/hr/training-hub", Icon = Icons.Material.Filled.School, Color = Color.Secondary },
		new() { Text = "Administration", Href = "/hr/hr-admin-hub", Icon = Icons.Material.Filled.AdminPanelSettings, Color = Color.Default },
		new() { Text = "Timesheets", Href = "/hr/timesheets", Icon = Icons.Material.Filled.AccessTime, Color = Color.Primary },
		new() { Text = "Approvals", Href = "/hr/approvals", Icon = Icons.Material.Filled.CheckCircle, Color = Color.Success },
		new() { Text = "Reports", Href = "/hr/reports", Icon = Icons.Material.Filled.Assessment, Color = Color.Info },
		new() { Text = "Settings", Href = "/hr/settings", Icon = Icons.Material.Filled.Settings, Color = Color.Secondary }
	}
}
```

## Usage

Copy this exact SubModules definition to the "HR" breadcrumb item on EVERY HR page for consistency.
