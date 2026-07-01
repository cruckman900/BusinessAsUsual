# UI Standards & Best Practices

## Overview
This document defines the UI/UX standards and best practices for the BusinessAsUsual platform, including the main shell (BusinessAsUsual.Web) and all microservice module UIs (HR.Web, etc.).

## Design System

### Component Library
- **Primary Framework**: MudBlazor 7.x
- **Icon Set**: Material Design Icons (via MudBlazor)
- **Color Palette**: MudBlazor default theme with primary/secondary/tertiary colors

### Layout Patterns
- **Shell Layout**: `MainLayout.razor` with sidebar navigation and top header
- **Module Layout**: `IframeLayout.razor` (minimal chrome for iframe embedding)
- **Responsive Grid**: MudGrid with xs/sm/md/lg/xl breakpoints

---

## Mandatory UI Standards

### 1. **Tooltips (REQUIRED)**

**All interactive elements MUST have tooltips.**

#### Scope
Tooltips are required on:
- ✅ All icon buttons (`MudIconButton`)
- ✅ All regular buttons (`MudButton`)
- ✅ Navigation links (`MudNavLink`, card navigation)
- ✅ Filter/action buttons in toolbars
- ✅ Any interactive element where the purpose is not immediately obvious from text alone

#### Implementation Pattern
Use MudBlazor's `<MudTooltip>` component wrapping the interactive element:

```razor
<MudTooltip Text="View employee details">
	<MudIconButton Icon="@Icons.Material.Filled.Visibility" 
				   Color="Color.Primary" 
				   Size="Size.Small" 
				   OnClick="() => ViewDetails(id)" />
</MudTooltip>
```

#### Tooltip Text Guidelines
- **Action buttons**: Use imperative verbs (e.g., "View details", "Edit employee", "Delete record")
- **Navigation**: Describe destination (e.g., "Navigate to employee directory", "Open benefits page")
- **Filters**: Describe filter action (e.g., "Show all employees", "Filter active records")
- **Icons without labels**: Describe the icon's purpose clearly
- **Keep concise**: 2-5 words preferred, maximum 10 words
- **Avoid redundancy**: Don't repeat visible button text verbatim

#### Tooltip Placement
- **Sidebar navigation**: `Placement.Right` (tooltips appear to the right of the menu)
- **Action buttons in tables**: `Placement.Top` or `Placement.Bottom` (default)
- **Header/toolbar buttons**: `Placement.Bottom`
- **Icon-only buttons**: `Placement.Top` preferred for visibility

#### Examples

✅ **Good:**
```razor
<MudTooltip Text="Create a new department">
	<MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add">
		Add Department
	</MudButton>
</MudTooltip>
```

✅ **Good:**
```razor
<MudTooltip Text="Mark as complete">
	<MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Color="Color.Success" />
</MudTooltip>
```

❌ **Bad (no tooltip):**
```razor
<MudIconButton Icon="@Icons.Material.Filled.Edit" Color="Color.Secondary" />
```

❌ **Bad (redundant tooltip):**
```razor
<MudTooltip Text="Add Department">
	<MudButton>Add Department</MudButton>
</MudTooltip>
```

---

### 2. **Icons**

#### Standard Icon Patterns
- **Create/Add**: `Icons.Material.Filled.Add`
- **Edit**: `Icons.Material.Filled.Edit`
- **Delete**: `Icons.Material.Filled.Delete`
- **View**: `Icons.Material.Filled.Visibility`
- **Save**: `Icons.Material.Filled.Save`
- **Cancel**: `Icons.Material.Filled.Cancel`
- **Search**: `Icons.Material.Filled.Search`
- **Filter**: `Icons.Material.Filled.FilterList`
- **People/Employees**: `Icons.Material.Filled.People`
- **Business/Departments**: `Icons.Material.Filled.Business`
- **Settings**: `Icons.Material.Filled.Settings`

#### Icon Sizing
- **Large cards/headers**: `Size.Large`
- **Table action buttons**: `Size.Small`
- **Standard buttons**: `Size.Medium`

---

### 3. **Color Usage**

#### Semantic Colors
- **Primary**: Main actions, navigation highlights
- **Success**: Confirmations, completed states, active status
- **Warning**: Cautions, pending states
- **Error**: Destructive actions, error states
- **Info**: Informational badges, help text
- **Secondary**: Less prominent actions, edit buttons
- **Default**: Neutral/disabled states

#### Examples
```razor
<!-- Primary action -->
<MudButton Color="Color.Primary">Create New</MudButton>

<!-- Destructive action -->
<MudIconButton Icon="@Icons.Material.Filled.Delete" Color="Color.Error" />

<!-- Status indicators -->
<MudChip Color="Color.Success">Active</MudChip>
<MudChip Color="Color.Warning">Pending</MudChip>
```

---

### 4. **Loading States**

Always provide loading feedback:

```razor
@if (isLoading)
{
	<div class="d-flex justify-center align-center" style="min-height: 400px;">
		<MudProgressCircular Color="Color.Primary" Indeterminate="true" Size="Size.Large" />
	</div>
}
else
{
	<!-- Content -->
}
```

---

### 5. **Empty States**

Provide helpful empty state messages:

```razor
@if (!items.Any())
{
	<MudAlert Severity="Severity.Info" Variant="Variant.Filled" Icon="@Icons.Material.Filled.Info">
		No items found. Click "Add Item" to create your first record.
	</MudAlert>
}
```

---

### 6. **Accessibility**

- **All interactive elements must have tooltips** (helps screen readers and discoverability)
- Use semantic HTML elements when possible
- Ensure sufficient color contrast (MudBlazor default theme is WCAG AA compliant)
- Support keyboard navigation (MudBlazor components have built-in keyboard support)

---

## Module-Specific Standards

### Embedded Modules (HR.Web, etc.)
- Use `IframeLayout` to avoid layout duplication
- All routes must use iframe-safe navigation
- Keep module navigation items registered with Module Registry Service
- Follow the same tooltip/icon/color standards as the main shell

### Shell Pages (BusinessAsUsual.Web)
- Use `MainLayout` for full shell experience
- Dynamic module discovery via `IModuleDiscoveryService`
- Sidebar navigation uses iframe postMessage for module navigation

---

## Code Review Checklist

Before submitting UI changes, verify:
- [ ] All icon buttons have tooltips
- [ ] All action buttons have tooltips
- [ ] Navigation links have tooltips (where appropriate)
- [ ] Tooltip text is concise and action-oriented
- [ ] Icons follow the standard patterns
- [ ] Colors are semantically appropriate
- [ ] Loading states are implemented
- [ ] Empty states provide helpful guidance
- [ ] MudBlazor components are used consistently

---

## References

- [MudBlazor Documentation](https://mudblazor.com/)
- [Material Design Icons](https://fonts.google.com/icons)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)

---

## Changelog

**2024-01-XX** - Initial version
- Defined mandatory tooltip requirements for all interactive elements
- Established icon, color, and component usage standards
- Added examples and code review checklist
