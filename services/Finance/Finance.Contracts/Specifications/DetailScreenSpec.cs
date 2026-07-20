namespace Finance.Contracts.Specifications;

/// <summary>Defines how mobile apps should render a read-only detail screen.</summary>
public class DetailScreenSpec
{
    public string Type { get; set; } = "detail";
    public string Title { get; set; } = string.Empty;
    public List<SectionDefinition> Sections { get; set; } = new();
    public List<ActionButton> Actions { get; set; } = new();
}

/// <summary>A section within the detail view.</summary>
public class SectionDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<FieldDefinition> Fields { get; set; } = new();
    public bool Collapsible { get; set; } = false;
    public bool DefaultCollapsed { get; set; } = false;
}

/// <summary>A field within a section (text, email, date, badge, currency).</summary>
public class FieldDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
    public bool ReadOnly { get; set; } = true;
    public string? Icon { get; set; }
    public string? Format { get; set; }
}
