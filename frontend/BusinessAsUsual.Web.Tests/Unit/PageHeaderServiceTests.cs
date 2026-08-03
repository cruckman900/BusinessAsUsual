using BusinessAsUsual.Web.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Components;

namespace BusinessAsUsual.Web.Tests.Unit;

public class PageHeaderServiceTests
{
    private PageHeaderService NewService() => new();

    [Fact]
    public void SetHeader_UpdatesHeader()
    {
        // Arrange
        var service = NewService();
        RenderFragment testHeader = builder => builder.AddContent(0, "Test Dashboard");

        // Act
        service.SetHeader(testHeader);

        // Assert
        service.Header.Should().Be(testHeader);
    }

    [Fact]
    public void SetHeader_WithNull_ClearsHeader()
    {
        // Arrange
        var service = NewService();
        service.SetHeader(builder => builder.AddContent(0, "Initial"));

        // Act
        service.SetHeader(null);

        // Assert
        service.Header.Should().BeNull();
    }

    [Fact]
    public void Header_InitiallyNull()
    {
        // Arrange & Act
        var service = NewService();

        // Assert
        service.Header.Should().BeNull();
    }

    [Fact]
    public void OnChange_RaisesEvent_WhenHeaderChanges()
    {
        // Arrange
        var service = NewService();
        var eventRaised = false;
        service.OnChange += () => eventRaised = true;

        // Act
        service.SetHeader(builder => builder.AddContent(0, "New Header"));

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void OnChange_RaisesEvent_WhenHeaderCleared()
    {
        // Arrange
        var service = NewService();
        service.SetHeader(builder => builder.AddContent(0, "Initial"));
        var eventRaised = false;
        service.OnChange += () => eventRaised = true;

        // Act
        service.SetHeader(null);

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void OnChange_DoesNotFire_WhenNotSubscribed()
    {
        // Arrange
        var service = NewService();

        // Act - should not throw even without subscribers
        Action act = () => service.SetHeader(builder => builder.AddContent(0, "Test"));

        // Assert
        act.Should().NotThrow();
    }


}
