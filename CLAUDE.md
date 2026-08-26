# MudBlazor Patterns Guide for Claude

This document contains patterns and examples for common MudBlazor components to prevent repetitive troubleshooting.

## MudDialog Pattern

### Dialog Component Structure

**Location**: `*.Web/Components/Dialogs/YourDialog.razor`

```razor
<MudDialog>
	<DialogContent>
		<!-- Your dialog content here -->
		<MudText>@Message</MudText>
	</DialogContent>
	<DialogActions>
		<MudButton OnClick="Cancel">Cancel</MudButton>
		<MudButton Color="Color.Primary" Variant="Variant.Filled" OnClick="Confirm">Confirm</MudButton>
	</DialogActions>
</MudDialog>

@code {
	// CRITICAL: This cascading parameter is required
	[CascadingParameter]
	IMudDialogInstance MudDialog { get; set; } = null!;

	// Your parameters
	[Parameter]
	public string Message { get; set; } = "Are you sure?";

	// Close methods
	void Confirm() => MudDialog.Close(DialogResult.Ok(true));
	void Cancel() => MudDialog.Cancel();
}
```

### Key Points for Dialogs

1. **DO NOT** try to import or qualify `IMudDialogInstance` - it's automatically available from `@using MudBlazor`
2. **ALWAYS** use `[CascadingParameter]` attribute (not async, not nullable)
3. **Type is `IMudDialogInstance`**, not `MudDialogInstance`
4. **Close methods**:
   - `MudDialog.Close(DialogResult.Ok(data))` - success with optional return data
   - `MudDialog.Cancel()` - user cancelled
4. **NO** `using MudBlazor` needed in most cases (already in _Imports.razor)

### Calling a Dialog from a Page

```csharp
@inject IDialogService DialogService

private async Task ShowMyDialog()
{
	var parameters = new DialogParameters 
	{ 
		{ "Message", "Custom message here" },
		{ "Title", "Dialog Title" }
	};

	var options = new DialogOptions 
	{ 
		CloseButton = true,
		MaxWidth = MaxWidth.Medium,
		FullWidth = true
	};

	var dialog = await DialogService.ShowAsync<MyDialog>("Dialog Title", parameters, options);
	var result = await dialog.Result;

	if (result != null && !result.Canceled)
	{
		// User confirmed - handle result
		var data = result.Data; // If you passed data with DialogResult.Ok(data)
	}
}
```

### Common Dialog Parameters

```csharp
var parameters = new DialogParameters
{
	{ "Message", "Are you sure?" },
	{ "YesText", "Confirm" },
	{ "CancelText", "Cancel" },
	{ "Color", Color.Error }
};
```

### Common Dialog Options

```csharp
var options = new DialogOptions
{
	CloseButton = true,           // Show X button
	MaxWidth = MaxWidth.Small,    // Small, Medium, Large, ExtraLarge
	FullWidth = true,             // Take full width of MaxWidth
	DisableBackdropClick = true,  // Prevent closing by clicking outside
	CloseOnEscapeKey = false      // Prevent closing with Escape key
};
```

## MudChip Pattern

### Basic MudChip Usage

```razor
<!-- Simple text chip -->
<MudChip T="string" Color="Color.Primary">Active</MudChip>

<!-- Chip with icon -->
<MudChip T="string" Icon="@Icons.Material.Filled.Check" Color="Color.Success">
	Completed
</MudChip>

<!-- Chip with size and variant -->
<MudChip T="string" Size="Size.Small" Color="Color.Info" Variant="Variant.Text">
	Badge
</MudChip>

<!-- Conditional color chip -->
<MudChip T="string" Color="@(isActive ? Color.Success : Color.Default)">
	@(isActive ? "Active" : "Inactive")
</MudChip>
```

### Key Points for MudChip

1. **ALWAYS** specify `T="string"` (or appropriate type)
2. Common colors: `Default`, `Primary`, `Secondary`, `Info`, `Success`, `Warning`, `Error`
3. Common variants: `Filled`, `Outlined`, `Text`
4. Common sizes: `Small`, `Medium`, `Large`

### MudChip in Tables

```razor
<MudTable Items="@items">
	<HeaderContent>
		<MudTh>Status</MudTh>
	</HeaderContent>
	<RowTemplate>
		<MudTd DataLabel="Status">
			<MudChip T="string" Size="Size.Small" Color="@GetStatusColor(context)">
				@context.Status
			</MudChip>
		</MudTd>
	</RowTemplate>
</MudTable>

@code {
	private Color GetStatusColor(MyItem item) => item.Status switch
	{
		"Active" => Color.Success,
		"Pending" => Color.Warning,
		"Failed" => Color.Error,
		_ => Color.Default
	};
}
```

## MudSelect Pattern

### Basic MudSelect

```razor
<MudSelect T="string" @bind-Value="_selectedValue" Label="Select Option" Variant="Variant.Outlined">
	<MudSelectItem Value="@((string)"Option1")">Option 1</MudSelectItem>
	<MudSelectItem Value="@((string)"Option2")">Option 2</MudSelectItem>
	<MudSelectItem Value="@((string)"Option3")">Option 3</MudSelectItem>
</MudSelect>

@code {
	private string _selectedValue = "";
}
```

### MudSelect with Loop

```razor
<MudSelect T="string" @bind-Value="_selectedTable" Label="Target Table" Variant="Variant.Outlined">
	@foreach (var table in _availableTables)
	{
		<MudSelectItem Value="@table">@table</MudSelectItem>
	}
</MudSelect>
```

### Key Points for MudSelect

1. **ALWAYS** specify `T="type"` matching your data type
2. Use `@bind-Value` for two-way binding
3. Cast literal values: `Value="@((string)"literal")"`
4. For objects, override `.Equals()` or use value types

## Common Troubleshooting

### "Type cannot be inferred" Error
**Cause**: Missing `T="type"` parameter  
**Fix**: Add `T="string"` or appropriate type to MudSelect/MudChip

### "MudDialogInstance not found" Error
**Cause**: Using wrong type name  
**Fix**: The correct type is `IMudDialogInstance` (with an `I` prefix), and it needs `@using MudBlazor` in _Imports.razor

### "Cannot convert to EventCallback" Error
**Cause**: Using wrong method signature for event handlers  
**Fix**: Match the expected signature (e.g., `ValueChanged<string>` expects `void Method(string value)`)

### Dialog Doesn't Close
**Cause**: Not calling `MudDialog.Close()` or `MudDialog.Cancel()`  
**Fix**: Ensure button handlers call the appropriate close method

## MudBlazor Version Reference

**Current Version**: 9.6.0 (.NET 9)

### Breaking Changes to Watch For
- Dialog API changed from `IDialogReference` to `IDialogReference` in v6+
- `MudDialog` component requires `DialogContent` and `DialogActions` sections
- Cascading parameter for `MudDialogInstance` must not be nullable starting v6+

## Additional Common Patterns

### MudTable with Actions

```razor
<MudTable Items="@items" Dense="true" Hover="true">
	<HeaderContent>
		<MudTh>Name</MudTh>
		<MudTh>Actions</MudTh>
	</HeaderContent>
	<RowTemplate>
		<MudTd DataLabel="Name">@context.Name</MudTd>
		<MudTd DataLabel="Actions">
			<MudIconButton Icon="@Icons.Material.Filled.Edit" 
						  Size="Size.Small" 
						  OnClick="@(() => EditItem(context))" />
			<MudIconButton Icon="@Icons.Material.Filled.Delete" 
						  Size="Size.Small" 
						  Color="Color.Error"
						  OnClick="@(() => DeleteItem(context))" />
		</MudTd>
	</RowTemplate>
</MudTable>
```

### MudTextField with Validation

```razor
<MudTextField @bind-Value="_model.Email" 
			  Label="Email" 
			  Variant="Variant.Outlined"
			  Validation="@(new Func<string, string>(ValidateEmail))" />

@code {
	private string ValidateEmail(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
			return "Email is required";
		if (!email.Contains("@"))
			return "Invalid email format";
		return null; // Valid
	}
}
```

### MudFileUpload

```razor
<MudFileUpload T="IBrowserFile" OnFilesChanged="OnFileSelected" Accept=".csv,.xlsx">
	<ActivatorContent>
		<MudPaper Outlined="true" Class="pa-8 text-center cursor-pointer">
			<MudIcon Icon="@Icons.Material.Filled.CloudUpload" Size="Size.Large" />
			<MudText Typo="Typo.h6">Click or drag file here</MudText>
		</MudPaper>
	</ActivatorContent>
</MudFileUpload>

@code {
	private void OnFileSelected(InputFileChangeEventArgs e)
	{
		var file = e.File;
		// Handle file
	}
}
```

## Quick Reference Checklist

When creating a **MudDialog**:
- [ ] `[CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;`
- [ ] `<DialogContent>` and `<DialogActions>` sections
- [ ] Close/Cancel methods call `MudDialog.Close()` or `MudDialog.Cancel()`
- [ ] Caller uses `await DialogService.ShowAsync<T>()`
- [ ] `@using MudBlazor` is in _Imports.razor

When creating a **MudChip**:
- [ ] `T="string"` (or appropriate type)
- [ ] Color, Size, Variant specified if not using defaults

When creating a **MudSelect**:
- [ ] `T="type"` matches bound value type
- [ ] `@bind-Value` for two-way binding
- [ ] Literal values cast: `Value="@((string)"value")"`

---

**Last Updated**: 2026-08-26  
**MudBlazor Version**: 9.6.0
