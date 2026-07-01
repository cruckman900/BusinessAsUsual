using CRM.Application.DTOs;
using CRM.Application.Interfaces;

namespace CRM.Application.Services;

/// <summary>
/// Implementation of report service aggregating data from Lead, Opportunity, and Customer services
/// </summary>
public class ReportService : IReportService
{
    private readonly ILeadService _leadService;
    private readonly IOpportunityService _opportunityService;
    private readonly ICustomerService _customerService;

    public ReportService(
        ILeadService leadService,
        IOpportunityService opportunityService,
        ICustomerService customerService)
    {
        _leadService = leadService;
        _opportunityService = opportunityService;
        _customerService = customerService;
    }

    public async Task<SalesPipelineData> GetSalesPipelineDataAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var opportunities = await _opportunityService.GetAllOpportunitiesAsync();

        // Filter by date if specified
        if (startDate.HasValue)
            opportunities = opportunities.Where(o => o.CreatedDate >= startDate.Value);
        if (endDate.HasValue)
            opportunities = opportunities.Where(o => o.CreatedDate <= endDate.Value);

        var opportunitiesList = opportunities.ToList();

        // Define stage order
        var stageOrder = new[] { "Prospecting", "Qualification", "Proposal", "Negotiation", "Closed Won", "Closed Lost" };

        var stages = new List<PipelineStage>();
        var previousCount = 0;

        foreach (var stageName in stageOrder)
        {
            var stageOpportunities = opportunitiesList.Where(o => o.Stage == stageName).ToList();
            var count = stageOpportunities.Count;
            var totalValue = stageOpportunities.Sum(o => o.Amount);
            var weightedValue = stageOpportunities.Sum(o => o.WeightedAmount);

            // Calculate conversion rate (percentage that moved to this stage from previous)
            var conversionRate = previousCount > 0 ? (double)count / previousCount * 100 : 100;

            stages.Add(new PipelineStage
            {
                StageName = stageName,
                Count = count,
                TotalValue = totalValue,
                WeightedValue = weightedValue,
                ConversionRate = Math.Round(conversionRate, 1)
            });

            if (stageName != "Closed Lost") // Don't count closed lost in the progression
                previousCount = count > 0 ? count : previousCount;
        }

        return new SalesPipelineData
        {
            Stages = stages,
            TotalValue = opportunitiesList.Sum(o => o.Amount),
            WeightedValue = opportunitiesList.Sum(o => o.WeightedAmount)
        };
    }

    public async Task<LeadSourceData> GetLeadSourceDataAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var leads = await _leadService.GetAllLeadsAsync();

        // Filter by date if specified
        if (startDate.HasValue)
            leads = leads.Where(l => l.CreatedDate >= startDate.Value);
        if (endDate.HasValue)
            leads = leads.Where(l => l.CreatedDate <= endDate.Value);

        var leadsList = leads.ToList();
        var totalLeads = leadsList.Count;

        var sources = leadsList
            .GroupBy(l => l.Source)
            .Select(g => new LeadSourceItem
            {
                Source = g.Key,
                Count = g.Count(),
                Percentage = totalLeads > 0 ? Math.Round((double)g.Count() / totalLeads * 100, 1) : 0,
                ConvertedCount = g.Count(l => l.Status == "Converted"),
                ConversionRate = g.Any() ? Math.Round((double)g.Count(l => l.Status == "Converted") / g.Count() * 100, 1) : 0
            })
            .OrderByDescending(s => s.Count)
            .ToList();

        return new LeadSourceData
        {
            Sources = sources,
            TotalLeads = totalLeads
        };
    }

    public async Task<WinLossData> GetWinLossDataAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var opportunities = await _opportunityService.GetAllOpportunitiesAsync();

        // Filter by date if specified
        if (startDate.HasValue)
            opportunities = opportunities.Where(o => o.CreatedDate >= startDate.Value);
        if (endDate.HasValue)
            opportunities = opportunities.Where(o => o.CreatedDate <= endDate.Value);

        var opportunitiesList = opportunities.ToList();

        var closedWon = opportunitiesList.Where(o => o.IsWon).ToList();
        var closedLost = opportunitiesList.Where(o => o.IsLost).ToList();

        var wonCount = closedWon.Count;
        var lostCount = closedLost.Count;
        var wonRevenue = closedWon.Sum(o => o.Amount);
        var lostRevenue = closedLost.Sum(o => o.Amount);
        var totalClosed = wonCount + lostCount;

        return new WinLossData
        {
            ClosedWon = wonCount,
            ClosedLost = lostCount,
            WonRevenue = wonRevenue,
            LostRevenue = lostRevenue,
            WinRate = totalClosed > 0 ? Math.Round((double)wonCount / totalClosed * 100, 1) : 0,
            AverageDealSize = wonCount > 0 ? Math.Round(wonRevenue / wonCount, 2) : 0
        };
    }

    public async Task<RevenueForecastData> GetRevenueForecastDataAsync(int months = 6)
    {
        var opportunities = await _opportunityService.GetAllOpportunitiesAsync();
        var opportunitiesList = opportunities.ToList();

        var dataPoints = new List<RevenueDataPoint>();
        var startDate = DateTime.Now.Date;

        for (int i = 0; i < months; i++)
        {
            var monthDate = startDate.AddMonths(i);
            var monthOpportunities = opportunitiesList
                .Where(o => o.ExpectedCloseDate.HasValue &&
                           o.ExpectedCloseDate.Value.Year == monthDate.Year &&
                           o.ExpectedCloseDate.Value.Month == monthDate.Month)
                .ToList();

            var weightedPipeline = monthOpportunities.Sum(o => o.WeightedAmount);
            var actualRevenue = monthOpportunities.Where(o => o.IsWon).Sum(o => o.Amount);

            // Simple forecast: use weighted pipeline for future, actual for past
            var forecast = monthDate <= DateTime.Now ? actualRevenue : weightedPipeline;

            dataPoints.Add(new RevenueDataPoint
            {
                Date = new DateTime(monthDate.Year, monthDate.Month, 1),
                WeightedPipeline = weightedPipeline,
                ActualRevenue = actualRevenue,
                Forecast = forecast
            });
        }

        return new RevenueForecastData
        {
            DataPoints = dataPoints,
            TotalForecast = dataPoints.Sum(d => d.Forecast),
            TotalActual = dataPoints.Sum(d => d.ActualRevenue)
        };
    }

    public async Task<TopCustomersData> GetTopCustomersDataAsync(int topCount = 10)
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var opportunities = await _opportunityService.GetAllOpportunitiesAsync();

        var customersList = customers.ToList();
        var opportunitiesList = opportunities.ToList();

        var topCustomers = customersList
            .OrderByDescending(c => c.LifetimeValue)
            .Take(topCount)
            .Select(c => new CustomerRevenueItem
            {
                CustomerId = c.Id,
                CustomerName = c.CompanyName ?? c.Name,
                LifetimeValue = c.LifetimeValue,
                OpportunityCount = opportunitiesList.Count(o => o.CustomerId == c.Id),
                ActiveOpportunities = opportunitiesList.Count(o => o.CustomerId == c.Id && !o.IsWon && !o.IsLost)
            })
            .ToList();

        return new TopCustomersData
        {
            Customers = topCustomers
        };
    }

    public async Task<ConversionMetrics> GetConversionMetricsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var leads = await _leadService.GetAllLeadsAsync();
        var opportunities = await _opportunityService.GetAllOpportunitiesAsync();
        var customers = await _customerService.GetAllCustomersAsync();

        // Filter by date if specified
        if (startDate.HasValue)
        {
            leads = leads.Where(l => l.CreatedDate >= startDate.Value);
            opportunities = opportunities.Where(o => o.CreatedDate >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            leads = leads.Where(l => l.CreatedDate <= endDate.Value);
            opportunities = opportunities.Where(o => o.CreatedDate <= endDate.Value);
        }

        var leadsList = leads.ToList();
        var opportunitiesList = opportunities.ToList();
        var customersList = customers.ToList();

        var totalLeads = leadsList.Count;
        var convertedLeads = leadsList.Count(l => l.Status == "Converted");
        var totalOpportunities = opportunitiesList.Count;
        var wonOpportunities = opportunitiesList.Count(o => o.IsWon);
        var totalCustomers = customersList.Count;

        var leadToOppRate = totalLeads > 0 ? Math.Round((double)convertedLeads / totalLeads * 100, 1) : 0;
        var oppToCustomerRate = totalOpportunities > 0 ? Math.Round((double)wonOpportunities / totalOpportunities * 100, 1) : 0;
        var overallRate = totalLeads > 0 ? Math.Round((double)wonOpportunities / totalLeads * 100, 1) : 0;

        // Calculate average days to convert (simplified - using created date differences)
        var avgDaysToConvert = 0.0;
        if (wonOpportunities > 0)
        {
            var wonOpps = opportunitiesList.Where(o => o.IsWon && o.ExpectedCloseDate.HasValue).ToList();
            if (wonOpps.Any())
            {
                avgDaysToConvert = wonOpps.Average(o => (o.ExpectedCloseDate!.Value - o.CreatedDate).TotalDays);
            }
        }

        return new ConversionMetrics
        {
            LeadToOpportunityRate = leadToOppRate,
            OpportunityToCustomerRate = oppToCustomerRate,
            OverallConversionRate = overallRate,
            AverageDaysToConvert = Math.Round(avgDaysToConvert, 0),
            TotalLeads = totalLeads,
            TotalOpportunities = totalOpportunities,
            TotalCustomers = totalCustomers
        };
    }

    public async Task<ReportSummaryKPIs> GetSummaryKPIsAsync()
    {
        var opportunities = await _opportunityService.GetAllOpportunitiesAsync();
        var leads = await _leadService.GetAllLeadsAsync();
        var customers = await _customerService.GetAllCustomersAsync();

        var opportunitiesList = opportunities.ToList();
        var leadsList = leads.ToList();
        var customersList = customers.ToList();

        var activeOpportunities = opportunitiesList.Where(o => !o.IsWon && !o.IsLost).ToList();
        var closedOpportunities = opportunitiesList.Where(o => o.IsWon || o.IsLost).ToList();
        var wonOpportunities = closedOpportunities.Where(o => o.IsWon).ToList();

        var totalPipeline = activeOpportunities.Sum(o => o.Amount);
        var weightedPipeline = activeOpportunities.Sum(o => o.WeightedAmount);
        var winRate = closedOpportunities.Any() ? 
            Math.Round((double)wonOpportunities.Count / closedOpportunities.Count * 100, 1) : 0;
        var avgDealSize = wonOpportunities.Any() ? 
            Math.Round(wonOpportunities.Average(o => o.Amount), 2) : 0;
        var leadConversion = leadsList.Any() ? 
            Math.Round((double)leadsList.Count(l => l.Status == "Converted") / leadsList.Count * 100, 1) : 0;

        return new ReportSummaryKPIs
        {
            TotalPipelineValue = totalPipeline,
            WeightedPipelineValue = weightedPipeline,
            ActiveOpportunities = activeOpportunities.Count,
            TotalLeads = leadsList.Count,
            TotalCustomers = customersList.Count,
            WinRate = winRate,
            AverageDealSize = avgDealSize,
            LeadConversionRate = leadConversion
        };
    }
}
