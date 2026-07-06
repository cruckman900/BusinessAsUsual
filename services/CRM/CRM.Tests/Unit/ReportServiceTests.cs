using CRM.Application.Services;

namespace CRM.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ReportService"/>, which aggregates data from the
/// lead, opportunity, and customer services.
/// </summary>
public class ReportServiceTests
{
    private static ReportService CreateService() =>
        new(new MockLeadService(), new MockOpportunityService(), new MockCustomerService());

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetSalesPipelineDataAsync_ReturnsAllStages()
    {
        var service = CreateService();

        var data = await service.GetSalesPipelineDataAsync();

        Assert.Equal(6, data.Stages.Count);
        Assert.True(data.TotalValue > 0);
        Assert.True(data.WeightedValue >= 0);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetSalesPipelineDataAsync_RespectsDateFilter()
    {
        var service = CreateService();

        // A future start date should exclude all seeded opportunities.
        var data = await service.GetSalesPipelineDataAsync(startDate: DateTime.UtcNow.AddYears(1));

        Assert.Equal(0, data.TotalValue);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetLeadSourceDataAsync_SumsToTotalLeads()
    {
        var service = CreateService();

        var data = await service.GetLeadSourceDataAsync();

        Assert.True(data.TotalLeads > 0);
        Assert.Equal(data.TotalLeads, data.Sources.Sum(s => s.Count));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetWinLossDataAsync_ComputesWinRate()
    {
        var service = CreateService();

        var data = await service.GetWinLossDataAsync();

        Assert.True(data.ClosedWon >= 0);
        Assert.True(data.ClosedLost >= 0);
        Assert.InRange(data.WinRate, 0, 100);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetRevenueForecastDataAsync_ReturnsRequestedMonths()
    {
        var service = CreateService();

        var data = await service.GetRevenueForecastDataAsync(months: 4);

        Assert.Equal(4, data.DataPoints.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetTopCustomersDataAsync_LimitsToTopCount()
    {
        var service = CreateService();

        var data = await service.GetTopCustomersDataAsync(topCount: 3);

        Assert.True(data.Customers.Count <= 3);
        // Verify descending order by lifetime value.
        var values = data.Customers.Select(c => c.LifetimeValue).ToList();
        Assert.Equal(values.OrderByDescending(v => v), values);
    }
}
