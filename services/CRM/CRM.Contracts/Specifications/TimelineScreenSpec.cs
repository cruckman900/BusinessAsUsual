namespace CRM.Contracts.Specifications;

/// <summary>
/// Defines how mobile apps should render a vertical activity timeline
/// (mirrors the web MudTimeline). Rows are fetched from the same data
/// endpoint as list screens; the field map tells the client which row
/// keys drive each part of a timeline item.
/// </summary>
public class TimelineScreenSpec
{
    public string Type { get; set; } = "timeline";
    public string Title { get; set; } = string.Empty;
    public string SearchPlaceholder { get; set; } = "Search...";
    public bool EnableSearch { get; set; } = true;

    /// <summary>Optional stat tiles rendered above the timeline.</summary>
    public List<StatCard> Stats { get; set; } = new();

    /// <summary>Maps timeline item parts to row keys.</summary>
    public TimelineItemFieldMap ItemFields { get; set; } = new();

    public List<ActionButton> Actions { get; set; } = new();
    public string EmptyStateMessage { get; set; } = "No activities found.";
}

/// <summary>Maps a data row's keys onto the parts of a timeline item.</summary>
public class TimelineItemFieldMap
{
    public string TitleField { get; set; } = "subject";
    public string SubtitleField { get; set; } = "relatedTo";
    public string DescriptionField { get; set; } = "description";
    public string TimestampField { get; set; } = "dueDate";
    public string StatusField { get; set; } = "status";
    public string TypeField { get; set; } = "type";
    public string OwnerField { get; set; } = "owner";
    public string IconField { get; set; } = "icon";
}

/// <summary>
/// A small stat tile (mirrors the web MudPaper stat cards): a large value
/// with a caption label and a semantic tone that drives its accent color.
/// </summary>
public class StatCard
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    /// <summary>positive | warning | negative | info | neutral</summary>
    public string Tone { get; set; } = "neutral";
}
