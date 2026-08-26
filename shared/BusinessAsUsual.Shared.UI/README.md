# Shared UI Components Library

## Overview
The `BusinessAsUsual.Shared.UI` project contains reusable Blazor/Razor components that can be used across all Web projects in the solution.

## Location
`shared/BusinessAsUsual.Shared.UI/`

## Components

### CustomDataGrid<TItem>
A powerful, reusable data grid wrapper around MudBlazor's MudDataGrid with built-in search, filtering, sorting, and pagination capabilities.

**Features:**
- Integrated search toolbar with customizable placeholder
- Column filtering and sorting
- Pagination with customizable page size
- Configurable density, hover effects, and striping
- Fixed header support for scrollable grids
- Custom quick filter functions
- Optional title and custom toolbar content

**Usage Example:**
```razor
@using BusinessAsUsual.Shared.UI.Components

<CustomDataGrid TItem="Employee"
				Items="@employees"
				Title="Employees"
				ShowSearch="true"
				SearchPlaceholder="Search employees..."
				Height="600px"
				RowsPerPage="25">
	<ToolbarContent>
		<MudButton Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add">
			Add Employee
		</MudButton>
	</ToolbarContent>
	<ChildContent>
		<PropertyColumn Property="x => x.FirstName" Title="First Name" />
		<PropertyColumn Property="x => x.LastName" Title="Last Name" />
		<PropertyColumn Property="x => x.Email" Title="Email" />
		<TemplateColumn Title="Actions">
			<CellTemplate>
				<MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" />
			</CellTemplate>
		</TemplateColumn>
	</ChildContent>
</CustomDataGrid>
```

## How to Use in Your Project

### 1. Add Project Reference
In your `.csproj` file, add:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\..\shared\BusinessAsUsual.Shared.UI\BusinessAsUsual.Shared.UI.csproj" />
</ItemGroup>
```

### 2. Import in _Imports.razor
In your `Components/_Imports.razor`, add:
```razor
@using BusinessAsUsual.Shared.UI.Components
```

### 3. Use the Components
The components will now be available throughout your Blazor project without needing additional imports in individual pages.

## Projects Already Using Shared.UI
- HR.Web
- Platform.Web

## Benefits
- **Single Source of Truth**: One component definition used everywhere
- **Consistency**: All grids behave the same across modules
- **Maintainability**: Update once, apply everywhere
- **No Duplication**: Eliminates component name conflicts
- **Easy Testing**: Components can be tested in isolation

## Dependencies
- .NET 9.0
- MudBlazor 8.3.1
- Microsoft.AspNetCore.Components.Web 9.0.19
