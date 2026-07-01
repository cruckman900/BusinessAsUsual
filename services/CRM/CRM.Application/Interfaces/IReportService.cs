using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

/// <summary>
/// Service for generating CRM reports and analytics data
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Gets sales pipeline data for funnel chart
    /// </summary>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    Task<SalesPipelineData> GetSalesPipelineDataAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets lead source distribution data for donut chart
    /// </summary>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    Task<LeadSourceData> GetLeadSourceDataAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets win/loss analysis data for bar chart
    /// </summary>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    Task<WinLossData> GetWinLossDataAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets revenue forecast data for line chart
    /// </summary>
    /// <param name="months">Number of months to forecast (default 6)</param>
    Task<RevenueForecastData> GetRevenueForecastDataAsync(int months = 6);

    /// <summary>
    /// Gets top customers by revenue for horizontal bar chart
    /// </summary>
    /// <param name="topCount">Number of top customers to return (default 10)</param>
    Task<TopCustomersData> GetTopCustomersDataAsync(int topCount = 10);

    /// <summary>
    /// Gets conversion rate metrics for dashboard cards
    /// </summary>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    Task<ConversionMetrics> GetConversionMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Gets summary KPIs for dashboard header cards
    /// </summary>
    Task<ReportSummaryKPIs> GetSummaryKPIsAsync();
}
