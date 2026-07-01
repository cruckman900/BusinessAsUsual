namespace HR.Contracts.Specifications;

/// <summary>
/// Defines how mobile apps should render the Employee List screen
/// </summary>
public class EmployeeListSpec
{
    public string Title { get; set; } = "Employees";
    public string SearchPlaceholder { get; set; } = "Search employees...";
    public bool EnableSearch { get; set; } = true;
    public bool EnableFilter { get; set; } = true;
    public List<ColumnDefinition> Columns { get; set; } = new();
    public List<ActionButton> Actions { get; set; } = new();
    public List<FilterOption> Filters { get; set; } = new();
    public string EmptyStateMessage { get; set; } = "No employees found";
}

/// <summary>
/// Defines a column in the list view
/// </summary>
public class ColumnDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text"; // text, image, date, badge
    public bool Sortable { get; set; } = false;
    public int Width { get; set; } = 100; // percentage or pixels
}

/// <summary>
/// Defines an action button
/// </summary>
public class ActionButton
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // navigate, api-call, custom
    public string? NavigateTo { get; set; }
    public string? ApiEndpoint { get; set; }
    public bool RequiresConfirmation { get; set; } = false;
    public string? ConfirmationMessage { get; set; }
}

/// <summary>
/// Defines a filter option
/// </summary>
public class FilterOption
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "select"; // select, checkbox, date-range
    public List<FilterValue> Values { get; set; } = new();
}

/// <summary>
/// Filter value option
/// </summary>
public class FilterValue
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
