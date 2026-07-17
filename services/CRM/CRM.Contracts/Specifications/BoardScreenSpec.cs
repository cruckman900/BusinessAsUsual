namespace CRM.Contracts.Specifications;

/// <summary>
/// Defines how mobile apps should render a kanban / board screen (e.g. the
/// sales pipeline grouped by stage). Each board column represents a group and
/// holds cards. The <see cref="FallbackColumns"/> field lets older mobile
/// builds that do not understand the "board" type degrade gracefully to a list.
/// </summary>
public class BoardScreenSpec
{
    public string Type { get; set; } = "board";
    public string Title { get; set; } = string.Empty;
    public string SearchPlaceholder { get; set; } = "Search...";
    public bool EnableSearch { get; set; } = true;

    /// <summary>
    /// The field on each card used to place it into a column (e.g. "stage").
    /// </summary>
    public string GroupByField { get; set; } = string.Empty;

    /// <summary>Ordered board columns (the swim lanes / stages).</summary>
    public List<BoardColumn> Columns { get; set; } = new();

    /// <summary>Describes how to render each card's content.</summary>
    public BoardCardLayout CardLayout { get; set; } = new();

    /// <summary>Screen-level actions (e.g. "New Opportunity").</summary>
    public List<ActionButton> Actions { get; set; } = new();

    /// <summary>
    /// When true, cards may be dragged between columns and the client should
    /// call <see cref="MoveEndpoint"/> to persist the new group value.
    /// </summary>
    public bool EnableDragToMove { get; set; } = false;

    /// <summary>
    /// API endpoint template used to move a card to a new column. Supports the
    /// tokens {id} and {group}, e.g. "/api/crm/opportunities/{id}/stage/{group}".
    /// </summary>
    public string? MoveEndpoint { get; set; }

    public string EmptyStateMessage { get; set; } = "Nothing to show yet.";

    /// <summary>
    /// Backward-compatible list rendering for clients that do not yet support
    /// the "board" type. Mirrors <see cref="ListScreenSpec.Columns"/>.
    /// </summary>
    public List<ColumnDefinition> FallbackColumns { get; set; } = new();
}

/// <summary>A single board column (group / stage).</summary>
public class BoardColumn
{
    /// <summary>Stable id matching the card's group value (e.g. "Prospecting").</summary>
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    /// <summary>Optional accent color (hex) for the column header.</summary>
    public string? Color { get; set; }

    /// <summary>Optional aggregate shown in the header (e.g. total amount).</summary>
    public string? SummaryLabel { get; set; }
}

/// <summary>
/// Describes which fields of a card map to the prominent card slots so the
/// mobile app can render a rich card instead of a generic row.
/// </summary>
public class BoardCardLayout
{
    /// <summary>Field used as the card's primary title (e.g. "name").</summary>
    public string TitleField { get; set; } = string.Empty;

    /// <summary>Field used as the card's subtitle (e.g. "customerName").</summary>
    public string? SubtitleField { get; set; }

    /// <summary>Field rendered as a prominent value/amount (e.g. "amount").</summary>
    public string? ValueField { get; set; }

    /// <summary>Field rendered as a progress indicator 0-100 (e.g. "probability").</summary>
    public string? ProgressField { get; set; }

    /// <summary>Field rendered as a badge (e.g. "stage").</summary>
    public string? BadgeField { get; set; }

    /// <summary>Field rendered as a footer/meta line (e.g. "expectedCloseDate").</summary>
    public string? MetaField { get; set; }
}

/// <summary>
/// Grouped board data returned by the board data endpoint. Rows are bucketed
/// into ordered groups so the client can render columns directly.
/// </summary>
public class BoardData
{
    /// <summary>The field used to bucket rows (mirrors the spec's GroupByField).</summary>
    public string GroupByField { get; set; } = string.Empty;
    public List<BoardGroup> Groups { get; set; } = new();
}

/// <summary>A single populated board group (column) with its cards and aggregates.</summary>
public class BoardGroup
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    /// <summary>Number of cards in this group.</summary>
    public int Count { get; set; }

    /// <summary>Display-formatted aggregate for the group (e.g. summed amount).</summary>
    public string Total { get; set; } = string.Empty;

    /// <summary>The cards in this group, keyed by column/field name.</summary>
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}
