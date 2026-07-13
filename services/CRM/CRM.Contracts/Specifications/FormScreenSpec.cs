namespace CRM.Contracts.Specifications;

/// <summary>
/// Defines how mobile apps should render a create/edit form screen.
/// </summary>
public class FormScreenSpec
{
    public string Type { get; set; } = "form";
    public string Title { get; set; } = string.Empty;
    public List<FormSectionDefinition> Sections { get; set; } = new();
    public List<ActionButton> Actions { get; set; } = new();
    public ValidationRules Validation { get; set; } = new();
}

/// <summary>A section within the form.</summary>
public class FormSectionDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<FormFieldDefinition> Fields { get; set; } = new();
}

/// <summary>A form field definition (text, email, phone, date, select, number, ...).</summary>
public class FormFieldDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
    public bool Required { get; set; } = false;
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public List<SelectOption>? Options { get; set; }
    public int? MaxLength { get; set; }
    public int? MinLength { get; set; }
    public string? Pattern { get; set; }
    public string? ValidationMessage { get; set; }
}

/// <summary>Select option for dropdown fields.</summary>
public class SelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

/// <summary>Validation rules for the form.</summary>
public class ValidationRules
{
    public Dictionary<string, string> ErrorMessages { get; set; } = new();
    public string? CustomValidationEndpoint { get; set; }
}
