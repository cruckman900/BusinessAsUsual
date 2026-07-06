using CRM.Application.DTOs;
using CRM.Application.Services;
using CRM.Domain.Enums;

namespace CRM.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="MockActivityService"/> covering querying, lifecycle, and filters.
/// </summary>
public class ActivityServiceTests
{
    private static CreateActivityRequest NewActivity(
        string? leadId = null,
        string? opportunityId = null,
        string? customerId = null,
        DateTime? dueDate = null) => new()
    {
        Type = ActivityType.Call,
        Subject = "Test activity",
        Description = "Created by unit test",
        ActivityDate = DateTime.UtcNow,
        DueDate = dueDate ?? DateTime.UtcNow.AddDays(1),
        Priority = Priority.Medium,
        DurationMinutes = 30,
        LeadId = leadId,
        OpportunityId = opportunityId,
        CustomerId = customerId,
        AssignedToEmployeeId = "EMP-001"
    };

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllActivitiesAsync_ReturnsSeededData()
    {
        var service = new MockActivityService();

        Assert.NotEmpty(await service.GetAllActivitiesAsync());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetActivityByIdAsync_ReturnsNull_WhenMissing()
    {
        var service = new MockActivityService();

        Assert.Null(await service.GetActivityByIdAsync("missing"));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateActivityAsync_AssignsIdAndPersists()
    {
        var service = new MockActivityService();

        var created = await service.CreateActivityAsync(NewActivity(leadId: "LEAD-XYZ"));

        Assert.False(string.IsNullOrWhiteSpace(created.Id));
        Assert.NotNull(await service.GetActivityByIdAsync(created.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetActivitiesByLeadIdAsync_FiltersByLead()
    {
        var service = new MockActivityService();
        var created = await service.CreateActivityAsync(NewActivity(leadId: "LEAD-FILTER"));

        var results = (await service.GetActivitiesByLeadIdAsync("LEAD-FILTER")).ToList();

        Assert.Contains(results, a => a.Id == created.Id);
        Assert.All(results, a => Assert.Equal("LEAD-FILTER", a.LeadId));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetActivitiesByOpportunityIdAsync_FiltersByOpportunity()
    {
        var service = new MockActivityService();
        var created = await service.CreateActivityAsync(NewActivity(opportunityId: "OPP-FILTER"));

        var results = (await service.GetActivitiesByOpportunityIdAsync("OPP-FILTER")).ToList();

        Assert.Contains(results, a => a.Id == created.Id);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetActivitiesByCustomerIdAsync_FiltersByCustomer()
    {
        var service = new MockActivityService();
        var created = await service.CreateActivityAsync(NewActivity(customerId: "CUST-FILTER"));

        var results = (await service.GetActivitiesByCustomerIdAsync("CUST-FILTER")).ToList();

        Assert.Contains(results, a => a.Id == created.Id);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUpcomingActivitiesAsync_ReturnsFutureIncompleteOnly()
    {
        var service = new MockActivityService();

        var upcoming = (await service.GetUpcomingActivitiesAsync(30)).ToList();

        Assert.All(upcoming, a => Assert.False(a.IsCompleted));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetOverdueActivitiesAsync_ReturnsPastIncompleteOnly()
    {
        var service = new MockActivityService();

        var overdue = (await service.GetOverdueActivitiesAsync()).ToList();

        Assert.All(overdue, a =>
        {
            Assert.False(a.IsCompleted);
            Assert.True(a.DueDate < DateTime.UtcNow);
        });
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateActivityAsync_ChangesFields()
    {
        var service = new MockActivityService();
        var created = await service.CreateActivityAsync(NewActivity(leadId: "LEAD-UPD"));

        var updated = await service.UpdateActivityAsync(created.Id, new UpdateActivityRequest
        {
            Type = ActivityType.Email,
            Subject = "Updated subject",
            ActivityDate = created.ActivityDate,
            IsCompleted = false,
            Priority = Priority.High,
            DurationMinutes = 15
        });

        Assert.Equal("Updated subject", updated.Subject);
        Assert.Equal(nameof(ActivityType.Email), updated.Type);
        Assert.Equal(nameof(Priority.High), updated.Priority);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateActivityAsync_Throws_WhenMissing()
    {
        var service = new MockActivityService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateActivityAsync("missing", new UpdateActivityRequest
            {
                Type = ActivityType.Call,
                Subject = "X",
                ActivityDate = DateTime.UtcNow,
                Priority = Priority.Low
            }));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CompleteActivityAsync_MarksCompleted()
    {
        var service = new MockActivityService();
        var created = await service.CreateActivityAsync(NewActivity(leadId: "LEAD-DONE"));

        var completed = await service.CompleteActivityAsync(created.Id, "Won", "Follow up");

        Assert.True(completed.IsCompleted);
        Assert.NotNull(completed.CompletedDate);
        Assert.Equal("Won", completed.Outcome);
        Assert.Equal("Follow up", completed.NextSteps);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CompleteActivityAsync_Throws_WhenMissing()
    {
        var service = new MockActivityService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CompleteActivityAsync("missing"));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteActivityAsync_RemovesActivity()
    {
        var service = new MockActivityService();
        var created = await service.CreateActivityAsync(NewActivity(leadId: "LEAD-DEL"));

        await service.DeleteActivityAsync(created.Id);

        Assert.Null(await service.GetActivityByIdAsync(created.Id));
    }
}
