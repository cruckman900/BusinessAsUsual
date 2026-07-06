using ModuleRegistry.Application.DTOs;
using ModuleRegistry.Application.Services;
using ModuleRegistry.Domain.Entities;
using ModuleRegistry.Domain.Repositories;
using Moq;

namespace ModuleRegistry.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ModuleRegistryService"/> using a mocked repository.
/// </summary>
public class ModuleRegistryServiceTests
{
    private static ModuleMetadata SampleModule(
        string moduleId = "hr",
        bool isActive = true,
        string? uiEntryPoint = "http://localhost/hr",
        bool supportsMobile = true,
        string? mobileSpec = "http://localhost/hr/spec") => new()
    {
        Id = Guid.NewGuid(),
        ModuleId = moduleId,
        Key = moduleId,
        DisplayName = moduleId.ToUpperInvariant(),
        Version = "1.0.0",
        ApiBaseUrl = "http://localhost/api",
        UiEntryPoint = uiEntryPoint,
        IsActive = isActive,
        SupportsMobile = supportsMobile,
        MobileUISpecUrl = mobileSpec,
        NavigationItems = new List<NavigationItem>
        {
            new() { Label = "Dashboard", Route = "/hr", Icon = "dashboard" }
        }
    };

    private static RegisterModuleRequest SampleRequest(string moduleId = "crm") => new()
    {
        ModuleId = moduleId,
        Key = moduleId,
        DisplayName = "CRM",
        Description = "Customer Relationship Management",
        Version = "1.0.0",
        ApiBaseUrl = "http://localhost/crm",
        UiEntryPoint = "http://localhost/crm/ui",
        HealthUrl = "http://localhost/crm/health",
        SupportsMobile = true,
        MobileUISpecUrl = "http://localhost/crm/spec",
        Permissions = new List<string> { "crm.read" },
        Capabilities = new List<string> { "leads" },
        NavigationItems = new List<NavigationItemDto>
        {
            new() { Label = "Leads", Route = "/crm/leads", Icon = "people" }
        }
    };

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllModulesAsync_MapsEntitiesToDtos()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { SampleModule("hr"), SampleModule("crm") });
        var service = new ModuleRegistryService(repo.Object);

        var result = (await service.GetAllModulesAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, m => m.ModuleId == "hr");
        Assert.Single(result.First().NavigationItems);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetModuleByIdAsync_ReturnsDto_WhenFound()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetByModuleIdAsync("hr")).ReturnsAsync(SampleModule("hr"));
        var service = new ModuleRegistryService(repo.Object);

        var result = await service.GetModuleByIdAsync("hr");

        Assert.NotNull(result);
        Assert.Equal("hr", result!.ModuleId);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetModuleByIdAsync_ReturnsNull_WhenMissing()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetByModuleIdAsync(It.IsAny<string>())).ReturnsAsync((ModuleMetadata?)null);
        var service = new ModuleRegistryService(repo.Object);

        Assert.Null(await service.GetModuleByIdAsync("missing"));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetActiveModulesAsync_DelegatesToRepository()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetActiveAsync()).ReturnsAsync(new[] { SampleModule("hr") });
        var service = new ModuleRegistryService(repo.Object);

        var result = (await service.GetActiveModulesAsync()).ToList();

        Assert.Single(result);
        repo.Verify(r => r.GetActiveAsync(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetModulesWithUiAsync_DelegatesToRepository()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModulesWithUiAsync()).ReturnsAsync(new[] { SampleModule("hr") });
        var service = new ModuleRegistryService(repo.Object);

        var result = (await service.GetModulesWithUiAsync()).ToList();

        Assert.Single(result);
        repo.Verify(r => r.GetModulesWithUiAsync(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetModulesWithMobileAsync_DelegatesToRepository()
    {
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.GetModulesWithMobileAsync()).ReturnsAsync(new[] { SampleModule("hr") });
        var service = new ModuleRegistryService(repo.Object);

        var result = (await service.GetModulesWithMobileAsync()).ToList();

        Assert.Single(result);
        repo.Verify(r => r.GetModulesWithMobileAsync(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterModuleAsync_BuildsMetadataAndPersists()
    {
        ModuleMetadata? captured = null;
        var repo = new Mock<IModuleRepository>();
        repo.Setup(r => r.AddOrUpdateAsync(It.IsAny<ModuleMetadata>()))
            .Callback<ModuleMetadata>(m => captured = m)
            .Returns(Task.CompletedTask);
        var service = new ModuleRegistryService(repo.Object);

        await service.RegisterModuleAsync(SampleRequest("crm"));

        Assert.NotNull(captured);
        Assert.Equal("crm", captured!.ModuleId);
        Assert.True(captured.IsActive);
        Assert.Equal("Unknown", captured.HealthStatus);
        Assert.Single(captured.NavigationItems);
        Assert.Equal("Leads", captured.NavigationItems[0].Label);
        repo.Verify(r => r.AddOrUpdateAsync(It.IsAny<ModuleMetadata>()), Times.Once);
    }
}
