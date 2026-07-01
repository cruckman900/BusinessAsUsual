namespace CRM.Application.DTOs;

/// <summary>
/// Sales pipeline data by stage for funnel chart
/// </summary>
public class SalesPipelineData
{
    public List<PipelineStage> Stages { get; set; } = new();
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
}

public class PipelineStage
{
    public string StageName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
    public double ConversionRate { get; set; } // Percentage to next stage
}

/// <summary>
/// Lead source distribution for donut chart
/// </summary>
public class LeadSourceData
{
    public List<LeadSourceItem> Sources { get; set; } = new();
    public int TotalLeads { get; set; }
}

public class LeadSourceItem
{
    public string Source { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public int ConvertedCount { get; set; }
    public double ConversionRate { get; set; }
}

/// <summary>
/// Win/Loss analysis for bar chart
/// </summary>
public class WinLossData
{
    public int ClosedWon { get; set; }
    public int ClosedLost { get; set; }
    public decimal WonRevenue { get; set; }
    public decimal LostRevenue { get; set; }
    public double WinRate { get; set; }
    public decimal AverageDealSize { get; set; }
}

/// <summary>
/// Revenue forecast over time for line chart
/// </summary>
public class RevenueForecastData
{
    public List<RevenueDataPoint> DataPoints { get; set; } = new();
    public decimal TotalForecast { get; set; }
    public decimal TotalActual { get; set; }
}

public class RevenueDataPoint
{
    public DateTime Date { get; set; }
    public decimal WeightedPipeline { get; set; }
    public decimal ActualRevenue { get; set; }
    public decimal Forecast { get; set; }
}

/// <summary>
/// Top customers by revenue for horizontal bar chart
/// </summary>
public class TopCustomersData
{
    public List<CustomerRevenueItem> Customers { get; set; } = new();
}

public class CustomerRevenueItem
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal LifetimeValue { get; set; }
    public int OpportunityCount { get; set; }
    public int ActiveOpportunities { get; set; }
}

/// <summary>
/// Conversion rate metrics for gauge/card display
/// </summary>
public class ConversionMetrics
{
    public double LeadToOpportunityRate { get; set; }
    public double OpportunityToCustomerRate { get; set; }
    public double OverallConversionRate { get; set; }
    public double AverageDaysToConvert { get; set; }
    public int TotalLeads { get; set; }
    public int TotalOpportunities { get; set; }
    public int TotalCustomers { get; set; }
}

/// <summary>
/// Summary KPIs for dashboard cards
/// </summary>
public class ReportSummaryKPIs
{
    public decimal TotalPipelineValue { get; set; }
    public decimal WeightedPipelineValue { get; set; }
    public int ActiveOpportunities { get; set; }
    public int TotalLeads { get; set; }
    public int TotalCustomers { get; set; }
    public double WinRate { get; set; }
    public decimal AverageDealSize { get; set; }
    public double LeadConversionRate { get; set; }
}
