using Bunit;
using BusinessAsUsual.Web.Components;
using FluentAssertions;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace BusinessAsUsual.Web.Tests.Components;

public class PageHeaderTests : BunitContext, IAsyncLifetime
{
    public PageHeaderTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose; // Allow MudBlazor JSInterop calls
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new async Task DisposeAsync()
    {
        await Services.DisposeAsync();
        Dispose();
    }

    [Fact]
    public void Render_DisplaysTitle()
    {
        // Arrange
        var title = "Dashboard";

        // Act
        var cut = Render<PageHeader>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert
        cut.Markup.Should().Contain(title);
        cut.FindComponents<MudText>().Should().Contain(t => t.Instance.Typo == Typo.h4);
    }

    [Fact]
    public void Render_DisplaysDescription_WhenProvided()
    {
        // Arrange
        var title = "Reports";
        var description = "View all system reports";

        // Act
        var cut = Render<PageHeader>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.Description, description));

        // Assert
        cut.Markup.Should().Contain(description);
    }

    [Fact]
    public void Render_DoesNotDisplayDescription_WhenNull()
    {
        // Arrange & Act
        var cut = Render<PageHeader>(parameters => parameters
            .Add(p => p.Title, "Test")
            .Add(p => p.Description, (string?)null));

        // Assert
        var textComponents = cut.FindComponents<MudText>();
        textComponents.Should().HaveCount(1); // Only title, no description
    }

    [Fact]
    public void Render_DoesNotDisplayDescription_WhenEmpty()
    {
        // Arrange & Act
        var cut = Render<PageHeader>(parameters => parameters
            .Add(p => p.Title, "Test")
            .Add(p => p.Description, ""));

        // Assert
        var textComponents = cut.FindComponents<MudText>();
        textComponents.Should().HaveCount(1); // Only title, no description
    }

    [Fact]
    public void Render_DisplaysBreadcrumbs_WhenProvided()
    {
        // Arrange
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new("Home", "/"),
            new("Admin", "/admin"),
            new("Settings", "/admin/settings")
        };

        // Act
        var cut = Render<PageHeader>(parameters => parameters
            .Add(p => p.Title, "Settings")
            .Add(p => p.Breadcrumbs, breadcrumbs));

        // Assert
        cut.FindComponents<MudBreadcrumbs>().Should().HaveCount(1);
    }

    [Fact]
    public void Render_DoesNotDisplayBreadcrumbs_WhenNull()
    {
        // Arrange & Act
        var cut = Render<PageHeader>(parameters => parameters
            .Add(p => p.Title, "Test")
            .Add(p => p.Breadcrumbs, (List<BreadcrumbItem>?)null));

        // Assert
        cut.FindComponents<MudBreadcrumbs>().Should().BeEmpty();
    }

    [Fact]
    public void Render_WithAllParameters_DisplaysAllContent()
    {
        // Arrange
        var title = "User Management";
        var description = "Manage system users and permissions";
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new("Home", "/"),
            new("Admin", "/admin")
        };

        // Act
        var cut = Render<PageHeader>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.Description, description)
            .Add(p => p.Breadcrumbs, breadcrumbs));

        // Assert
        cut.Markup.Should().Contain(title);
        cut.Markup.Should().Contain(description);
        cut.FindComponents<MudBreadcrumbs>().Should().HaveCount(1);
    }
}
