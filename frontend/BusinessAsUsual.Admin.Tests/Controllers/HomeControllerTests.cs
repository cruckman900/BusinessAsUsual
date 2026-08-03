using BusinessAsUsual.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessAsUsual.Admin.Tests.Controllers;

public class HomeControllerTests
{
    private HomeController NewController()
    {
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<HomeController>>();
        return new HomeController(httpClientFactory.Object, logger.Object);
    }

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
