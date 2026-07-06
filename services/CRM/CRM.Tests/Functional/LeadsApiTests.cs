using System.Net;
using System.Net.Http.Json;
using CRM.Application.DTOs;
using CRM.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CRM.Tests.Functional;

/// <summary>
/// Functional tests that exercise the CRM API HTTP surface end-to-end through the
/// in-memory <see cref="WebApplicationFactory{TEntryPoint}"/> host (mock services).
/// </summary>
public class LeadsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LeadsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ReturnsOkAndLeads()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/leads");

        response.EnsureSuccessStatusCode();
        var leads = await response.Content.ReadFromJsonAsync<List<LeadDto>>();
        Assert.NotNull(leads);
        Assert.NotEmpty(leads!);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetById_ReturnsNotFound_ForMissingLead()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/leads/{Guid.NewGuid():N}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Create_ThenGet_RoundTripsLead()
    {
        var client = _factory.CreateClient();

        var create = await client.PostAsJsonAsync("/api/leads", new CreateLeadRequest
        {
            FirstName = "Api",
            LastName = "Tester",
            Email = $"api-{Guid.NewGuid():N}@example.com",
            Company = "Functional Co",
            Source = LeadSource.Website
        });

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<LeadDto>();
        Assert.NotNull(created);

        var get = await client.GetAsync($"/api/leads/{created!.Id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<LeadDto>();
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var create = await client.PostAsJsonAsync("/api/leads", new CreateLeadRequest
        {
            FirstName = "Delete",
            LastName = "Me",
            Email = $"del-{Guid.NewGuid():N}@example.com",
            Source = LeadSource.Referral
        });
        var created = await create.Content.ReadFromJsonAsync<LeadDto>();

        var response = await client.DeleteAsync($"/api/leads/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
