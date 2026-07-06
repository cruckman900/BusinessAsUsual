using CRM.Application.DTOs;
using CRM.Application.Services;
using CRM.Domain.Enums;

namespace CRM.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="MockLeadService"/> covering CRUD and conversion behavior.
/// </summary>
public class LeadServiceTests
{
    private static CreateLeadRequest NewLeadRequest(string email = "new.lead@example.com") => new()
    {
        FirstName = "New",
        LastName = "Lead",
        Email = email,
        Company = "Example Co",
        JobTitle = "Buyer",
        Source = LeadSource.Website,
        EstimatedValue = 12345,
        Tags = new List<string> { "Test" }
    };

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllLeadsAsync_ReturnsSeededLeads()
    {
        var service = new MockLeadService();

        var leads = (await service.GetAllLeadsAsync()).ToList();

        Assert.NotEmpty(leads);
        Assert.All(leads, l => Assert.False(string.IsNullOrWhiteSpace(l.Id)));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetLeadByIdAsync_ReturnsLead_WhenExists()
    {
        var service = new MockLeadService();

        var lead = await service.GetLeadByIdAsync("1");

        Assert.NotNull(lead);
        Assert.Equal("1", lead!.Id);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetLeadByIdAsync_ReturnsNull_WhenMissing()
    {
        var service = new MockLeadService();

        var lead = await service.GetLeadByIdAsync("does-not-exist");

        Assert.Null(lead);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateLeadAsync_AddsLead_WithNewStatusAndId()
    {
        var service = new MockLeadService();

        var created = await service.CreateLeadAsync(NewLeadRequest($"create-{Guid.NewGuid():N}@example.com"));

        Assert.False(string.IsNullOrWhiteSpace(created.Id));
        Assert.Equal(nameof(LeadStatus.New), created.Status);
        Assert.NotNull(await service.GetLeadByIdAsync(created.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateLeadAsync_ChangesFields_WhenExists()
    {
        var service = new MockLeadService();
        var created = await service.CreateLeadAsync(NewLeadRequest($"update-{Guid.NewGuid():N}@example.com"));

        var updated = await service.UpdateLeadAsync(created.Id, new UpdateLeadRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = created.Email,
            Status = LeadStatus.Qualified,
            Source = LeadSource.Referral,
            EstimatedValue = 999
        });

        Assert.Equal("Updated", updated.FirstName);
        Assert.Equal(nameof(LeadStatus.Qualified), updated.Status);
        Assert.Equal(999, updated.EstimatedValue);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateLeadAsync_Throws_WhenMissing()
    {
        var service = new MockLeadService();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateLeadAsync("missing", new UpdateLeadRequest
            {
                FirstName = "X",
                LastName = "Y",
                Email = "x@y.com"
            }));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteLeadAsync_RemovesLead()
    {
        var service = new MockLeadService();
        var created = await service.CreateLeadAsync(NewLeadRequest($"delete-{Guid.NewGuid():N}@example.com"));

        await service.DeleteLeadAsync(created.Id);

        Assert.Null(await service.GetLeadByIdAsync(created.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteLeadAsync_IsNoOp_WhenMissing()
    {
        var service = new MockLeadService();

        var ex = await Record.ExceptionAsync(() => service.DeleteLeadAsync("missing"));

        Assert.Null(ex);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ConvertLeadToCustomerAsync_SetsConvertedState()
    {
        var service = new MockLeadService();
        var created = await service.CreateLeadAsync(NewLeadRequest($"convert-{Guid.NewGuid():N}@example.com"));

        var converted = await service.ConvertLeadToCustomerAsync(created.Id);

        Assert.Equal(nameof(LeadStatus.Converted), converted.Status);
        Assert.NotNull(converted.ConvertedDate);
        Assert.False(string.IsNullOrWhiteSpace(converted.ConvertedToCustomerId));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ConvertLeadToCustomerAsync_Throws_WhenMissing()
    {
        var service = new MockLeadService();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.ConvertLeadToCustomerAsync("missing"));
    }
}
