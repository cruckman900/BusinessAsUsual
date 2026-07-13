namespace HR.Contracts.Specifications;

/// <summary>
/// Defines how mobile apps should render a chart / analytics screen.
/// A chart screen contains one or more chart cards (line, bar, pie, donut, sparkline).
/// </summary>
public class ChartScreenSpec
{
    public string Type { get; set; } = "chart";
    public string Title { get; set; } = "Reports";
    public List<ChartSpec> Charts { get; set; } = new();
    public string EmptyStateMessage { get; set; } = "No analytics available.";
}

/// <summary>
/// A single chart definition. The <see cref="ChartType"/> selects the renderer.
/// </summary>
public class ChartSpec
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ChartType { get; set; } = "line"; // line, bar, pie, donut, sparkline
    /// <summary>Optional axis labels shared across series (e.g. months) for line/bar charts.</summary>
    public List<string> Labels { get; set; } = new();
    public List<ChartSeries> Series { get; set; } = new();
}

/// <summary>
/// A named series of data points. Pie/donut charts use a single series whose
/// points carry their own labels; line/bar charts may have multiple series.
/// </summary>
public class ChartSeries
{
    public string Name { get; set; } = string.Empty;
    /// <summary>Optional hex color (e.g. "#1B5E20"); renderers fall back to theme colors.</summary>
    public string? Color { get; set; }
    public List<ChartDataPoint> Points { get; set; } = new();
}

/// <summary>
/// A single data point. <see cref="Label"/> is used for categorical charts
/// (pie/donut slices, bar categories); <see cref="Value"/> is the magnitude.
/// </summary>
public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    /// <summary>Optional per-point hex color, primarily for pie/donut slices.</summary>
    public string? Color { get; set; }
}
