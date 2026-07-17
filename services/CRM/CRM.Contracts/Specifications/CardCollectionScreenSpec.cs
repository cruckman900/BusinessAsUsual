namespace CRM.Contracts.Specifications;

/// <summary>
/// Defines how mobile apps should render a collection of rich cards (e.g. email
/// templates shown as preview cards rather than table rows). Provides a
/// <see cref="FallbackColumns"/> list so clients that do not support the
/// "card-collection" type degrade gracefully to a list.
/// </summary>
public class CardCollectionScreenSpec
{
    public string Type { get; set; } = "card-collection";
    public string Title { get; set; } = string.Empty;
    public string SearchPlaceholder { get; set; } = "Search...";
    public bool EnableSearch { get; set; } = true;
    public bool EnableFilter { get; set; } = false;

    /// <summary>
    /// Preferred number of columns in the card grid on wide screens. Phones
    /// typically collapse to a single column regardless.
    /// </summary>
    public int PreferredColumns { get; set; } = 1;

    /// <summary>Describes how to render each card's content.</summary>
    public CardLayout CardLayout { get; set; } = new();

    /// <summary>Screen-level actions (e.g. "New Template").</summary>
    public List<ActionButton> Actions { get; set; } = new();

    /// <summary>Per-card actions rendered in an overflow / context menu.</summary>
    public List<ActionButton> CardActions { get; set; } = new();

    /// <summary>Optional filters (reuses the list filter model).</summary>
    public List<FilterOption> Filters { get; set; } = new();

    public string EmptyStateMessage { get; set; } = "No records found.";

    /// <summary>
    /// Backward-compatible list rendering for clients that do not yet support
    /// the "card-collection" type.
    /// </summary>
    public List<ColumnDefinition> FallbackColumns { get; set; } = new();
}

/// <summary>
/// Maps record fields onto the visual slots of a rich card so the mobile app
/// can render a preview card instead of a generic row.
/// </summary>
public class CardLayout
{
    /// <summary>Field used as the card title (e.g. "name").</summary>
    public string TitleField { get; set; } = string.Empty;

    /// <summary>Field used as the card subtitle (e.g. "subject").</summary>
    public string? SubtitleField { get; set; }

    /// <summary>Field rendered as a short preview / body snippet.</summary>
    public string? PreviewField { get; set; }

    /// <summary>Field rendered as a badge / pill (e.g. "category").</summary>
    public string? BadgeField { get; set; }

    /// <summary>Field rendered as a status chip (e.g. "status").</summary>
    public string? StatusField { get; set; }

    /// <summary>Field rendered as a footer/meta line (e.g. "updatedAt").</summary>
    public string? MetaField { get; set; }

    /// <summary>Optional field providing an icon or thumbnail hint.</summary>
    public string? IconField { get; set; }
}
