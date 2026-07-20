namespace Finance.Contracts.Specifications;

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

/// <summary>A single chart definition. ChartType selects the renderer.</summary>
public class ChartSpec
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ChartType { get; set; } = "line"; // line, bar, pie, donut, sparkline
    public List<string> Labels { get; set; } = new();
    public List<ChartSeries> Series { get; set; } = new();
}

/// <summary>A named series of data points.</summary>
public class ChartSeries
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public List<ChartDataPoint> Points { get; set; } = new();
}

/// <summary>A single data point (Label for categorical charts, Value for magnitude).</summary>
public class ChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    public string? Color { get; set; }
}
