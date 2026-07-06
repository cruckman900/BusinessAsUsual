using System.Net;
using System.Net.Http.Json;
using ModuleRegistry.Application.DTOs;

namespace ModuleRegistry.Tests.Functional;

/// <summary>
/// Functional tests exercising the Module Registry API HTTP surface end-to-end via
/// <see cref="ModuleRegistryApiFactory"/> (in-memory repository, no real database).
/// </summary>
public class ModulesApiTests : IClassFixture<ModuleRegistryApiFactory>
{
    private readonly ModuleRegistryApiFactory _factory;

    public ModulesApiTests(ModuleRegistryApiFactory factory)
    {
        _factory = factory;
    }

    private static RegisterModuleRequest NewModule(string moduleId) => new()
    {
        ModuleId = moduleId,
        Key = moduleId,
        DisplayName = moduleId.ToUpperInvariant(),
        Description = "Functional test module",
        Version = "1.0.0",
        ApiBaseUrl = $"http://localhost/{moduleId}",
        UiEntryPoint = $"http://localhost/{moduleId}/ui",
        HealthUrl = $"http://localhost/{moduleId}/health",
        SupportsMobile = true,
        MobileUISpecUrl = $"http://localhost/{moduleId}/spec",
        NavigationItems = new List<NavigationItemDto>
        {
            new() { Label = "Home", Route = $"/{moduleId}", Icon = "home" }
        }
    };

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllModules_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/modules");

        response.EnsureSuccessStatusCode();
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetModule_ReturnsNotFound_WhenMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/modules/{Guid.NewGuid():N}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Register_Then_Get_RoundTrips()
    {
        var client = _factory.CreateClient();
        var moduleId = $"mod-{Guid.NewGuid():N}";

        var register = await client.PostAsJsonAsync("/api/modules/register", NewModule(moduleId));
        register.EnsureSuccessStatusCode();

        var get = await client.GetAsync($"/api/modules/{moduleId}");
        get.EnsureSuccessStatusCode();
        var module = await get.Content.ReadFromJsonAsync<ModuleDto>();
        Assert.Equal(moduleId, module!.ModuleId);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Register_ExposesModuleViaActiveUiAndMobileEndpoints()
    {
        var client = _factory.CreateClient();
        var moduleId = $"mod-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/api/modules/register", NewModule(moduleId));

        var active = await client.GetFromJsonAsync<List<ModuleDto>>("/api/modules/active");
        var ui = await client.GetFromJsonAsync<List<ModuleDto>>("/api/modules/ui");
        var mobile = await client.GetFromJsonAsync<List<ModuleDto>>("/api/modules/mobile");

        Assert.Contains(active!, m => m.ModuleId == moduleId);
        Assert.Contains(ui!, m => m.ModuleId == moduleId);
        Assert.Contains(mobile!, m => m.ModuleId == moduleId);
    }
}
