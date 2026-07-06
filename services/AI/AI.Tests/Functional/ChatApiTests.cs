using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AI.Tests.Functional;

/// <summary>
/// Functional tests for the AI chat endpoint. Uses <see cref="AiApiFactory"/> to run the
/// full HTTP pipeline (controller, guardrails, tier routing) against fake AI clients.
/// Each test issues a single request to stay under the 10/min per-IP rate limit.
/// </summary>
public class ChatApiTests : IClassFixture<AiApiFactory>
{
    private readonly AiApiFactory _factory;

    public ChatApiTests(AiApiFactory factory)
    {
        _factory = factory;
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Post_ReturnsBadRequest_WhenQueryMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/chat", new { query = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Post_ReturnsDemoAnswer_WhenNoCompanyId()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/chat", new { query = "hello there" });

        response.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        Assert.Equal("demo answer", root.GetProperty("answer").GetString());
        Assert.Equal("Demo", root.GetProperty("tier").GetString());
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Post_ReturnsPaidAnswer_WhenCompanyProvisioned()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/ai/chat",
            new { query = "hello there", companyId = _factory.ProvisionedCompanyId });

        response.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        Assert.Equal("paid answer", root.GetProperty("answer").GetString());
        Assert.Equal("Paid", root.GetProperty("tier").GetString());
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Health_ReportsConfiguredTiers()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("healthy", doc.RootElement.GetProperty("status").GetString());
    }
}
