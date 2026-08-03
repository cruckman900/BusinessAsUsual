using Bunit;
using BusinessAsUsual.Web.Components;
using FluentAssertions;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace BusinessAsUsual.Web.Tests.Components;

public class TenantSelectorTests : Bunit.TestContext, IAsyncLifetime
{
    public TenantSelectorTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose; // Allow MudBlazor JSInterop calls
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await Services.DisposeAsync();
        Dispose();
    }

    [Fact]
    public void Render_DisplaysTenantDropdown()
    {
        // Arrange
        var tenants = new[] { "Acme Corp", "Contoso Ltd", "Fabrikam Inc" };

        // Act
        var cut = Render<TenantSelector>(parameters => parameters
            .Add(p => p.Tenants, tenants)
            .Add(p => p.CurrentTenant, "Acme Corp"));

        // Assert
        cut.Markup.Should().Contain("Client");
        cut.FindComponents<MudSelect<string>>().Should().HaveCount(1);
    }

    [Fact]
    public void SelectedTenant_BindsToCurrentTenant()
    {
        // Arrange
        var tenants = new[] { "Tenant A", "Tenant B", "Tenant C" };
        var currentTenant = "Tenant B";

        // Act
        var cut = Render<TenantSelector>(parameters => parameters
            .Add(p => p.Tenants, tenants)
            .Add(p => p.CurrentTenant, currentTenant));

        // Assert
        var select = cut.FindComponent<MudSelect<string>>();
        select.Instance.Value.Should().Be(currentTenant);
    }

    [Fact]
    public void TenantList_DisplaysAllProvidedTenants()
    {
        // Arrange
        var tenants = new[] { "Alpha", "Beta", "Gamma" };

        // Act
        var cut = Render<TenantSelector>(parameters => parameters
            .Add(p => p.Tenants, tenants));

        // Assert
        var items = cut.FindComponents<MudSelectItem<string>>();
        items.Should().HaveCount(3);
    }

    [Fact]
    public void CompactMode_AppliesDensePropertyToSelect()
    {
        // Arrange
        var tenants = new[] { "Test Tenant" };

        // Act
        var cut = Render<TenantSelector>(parameters => parameters
            .Add(p => p.Tenants, tenants)
            .Add(p => p.Compact, true));

        // Assert
        var select = cut.FindComponent<MudSelect<string>>();
        select.Instance.Dense.Should().BeTrue();
    }

    [Fact]
    public void EmptyTenantList_RendersWithoutError()
    {
        // Arrange & Act
        var cut = Render<TenantSelector>(parameters => parameters
            .Add(p => p.Tenants, Array.Empty<string>()));

        // Assert
        cut.Should().NotBeNull();
        var select = cut.FindComponent<MudSelect<string>>();
        select.Should().NotBeNull();
    }
}
