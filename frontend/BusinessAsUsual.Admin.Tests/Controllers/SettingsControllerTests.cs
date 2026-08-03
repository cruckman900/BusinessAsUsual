using BusinessAsUsual.Admin.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Tests.Controllers;

public class SettingsControllerTests
{
    private SettingsController NewController() => new();

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Arrange
        var controller = NewController();

        // Act
        var result = controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Index_ReturnsDefaultView()
    {
        // Arrange
        var controller = NewController();

        // Act
        var result = controller.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.ViewName.Should().BeNullOrEmpty(); // Default view name
    }
}
