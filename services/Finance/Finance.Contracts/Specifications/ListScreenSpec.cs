namespace Finance.Contracts.Specifications;

/// <summary>
/// Defines how mobile apps should render a list screen (the workhorse screen type).
/// </summary>
public class ListScreenSpec
{
    public string Type { get; set; } = "list";
    public string Title { get; set; } = string.Empty;
    public string SearchPlaceholder { get; set; } = "Search...";
    public bool EnableSearch { get; set; } = true;
    public bool EnableFilter { get; set; } = false;
    public List<ColumnDefinition> Columns { get; set; } = new();
    public List<ActionButton> Actions { get; set; } = new();
    public List<FilterOption> Filters { get; set; } = new();
    public List<StatCard> Stats { get; set; } = new();
    public string EmptyStateMessage { get; set; } = "No records found";
}

/// <summary>Defines a column in the list view.</summary>
public class ColumnDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
    public bool Sortable { get; set; } = false;
    public int Width { get; set; } = 150;
}

/// <summary>Defines an action button (list-level Id = "add"; others are per-row).</summary>
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

/// <summary>Defines a filter option (select, checkbox, date-range).</summary>
public class FilterOption
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "select";
    public List<FilterValue> Values { get; set; } = new();
}

/// <summary>Filter value option.</summary>
public class FilterValue
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

/// <summary>A summary stat card shown above a list.</summary>
public class StatCard
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
}
