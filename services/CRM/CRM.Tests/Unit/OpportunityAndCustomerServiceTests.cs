using CRM.Application.DTOs;
using CRM.Application.Services;
using CRM.Domain.Enums;

namespace CRM.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="MockOpportunityService"/> and <see cref="MockCustomerService"/>.
/// </summary>
public class OpportunityAndCustomerServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllOpportunitiesAsync_ReturnsSeededData()
    {
        var service = new MockOpportunityService();

        var opportunities = (await service.GetAllOpportunitiesAsync()).ToList();

        Assert.NotEmpty(opportunities);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetOpportunityByIdAsync_ReturnsNull_WhenMissing()
    {
        var service = new MockOpportunityService();

        Assert.Null(await service.GetOpportunityByIdAsync("nope"));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetOpportunitiesByCustomerAsync_FiltersByCustomer()
    {
        var service = new MockOpportunityService();

        var results = (await service.GetOpportunitiesByCustomerAsync("C1")).ToList();

        Assert.All(results, o => Assert.Equal("C1", o.CustomerId));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateOpportunityAsync_AddsWeightedAmount()
    {
        var service = new MockOpportunityService();

        var created = await service.CreateOpportunityAsync(new CreateOpportunityRequest
        {
            Name = "Test Opp",
            CustomerId = "C1",
            Stage = OpportunityStage.Proposal,
            Amount = 1000m,
            Probability = 50m,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(30)
        });

        Assert.Equal(500m, created.WeightedAmount);
        Assert.NotNull(await service.GetOpportunityByIdAsync(created.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateOpportunityAsync_SetsActualCloseDate_WhenClosed()
    {
        var service = new MockOpportunityService();
        var created = await service.CreateOpportunityAsync(new CreateOpportunityRequest
        {
            Name = "To Close",
            CustomerId = "C2",
            Stage = OpportunityStage.Negotiation,
            Amount = 5000m,
            Probability = 80m
        });

        var updated = await service.UpdateOpportunityAsync(created.Id, new UpdateOpportunityRequest
        {
            Name = created.Name,
            CustomerId = created.CustomerId,
            Stage = OpportunityStage.ClosedWon,
            Amount = 5000m,
            Probability = 100m
        });

        Assert.True(updated.IsWon);
        Assert.NotNull(updated.ActualCloseDate);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateOpportunityAsync_Throws_WhenMissing()
    {
        var service = new MockOpportunityService();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateOpportunityAsync("missing", new UpdateOpportunityRequest
            {
                Name = "X",
                Stage = OpportunityStage.Prospecting,
                Amount = 1,
                Probability = 1
            }));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteOpportunityAsync_RemovesOpportunity()
    {
        var service = new MockOpportunityService();
        var created = await service.CreateOpportunityAsync(new CreateOpportunityRequest
        {
            Name = "Delete Me",
            CustomerId = "C3",
            Stage = OpportunityStage.Prospecting,
            Amount = 100m,
            Probability = 10m
        });

        await service.DeleteOpportunityAsync(created.Id);

        Assert.Null(await service.GetOpportunityByIdAsync(created.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CustomerService_Crud_Roundtrips()
    {
        var service = new MockCustomerService();

        var created = await service.CreateCustomerAsync(new CreateCustomerRequest
        {
            Name = "Acme",
            CompanyName = "Acme Corp",
            Email = "info@acme.test",
            CustomerSegment = "SMB"
        });
        Assert.Equal("Active", created.CustomerStatus);

        var fetched = await service.GetCustomerByIdAsync(created.Id);
        Assert.NotNull(fetched);

        var updated = await service.UpdateCustomerAsync(created.Id, new UpdateCustomerRequest
        {
            Name = "Acme Updated",
            CompanyName = "Acme Corp",
            Email = "info@acme.test",
            CustomerSegment = "Mid-Market",
            CustomerStatus = "Active",
            LifetimeValue = 1000
        });
        Assert.Equal("Acme Updated", updated.Name);
        Assert.Equal(1000, updated.LifetimeValue);

        await service.DeleteCustomerAsync(created.Id);
        Assert.Null(await service.GetCustomerByIdAsync(created.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CustomerService_Update_Throws_WhenMissing()
    {
        var service = new MockCustomerService();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateCustomerAsync("missing", new UpdateCustomerRequest
            {
                Name = "X",
                Email = "x@y.com"
            }));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CustomerService_GetAll_ReturnsSeededData()
    {
        var service = new MockCustomerService();

        Assert.NotEmpty(await service.GetAllCustomersAsync());
    }
}
