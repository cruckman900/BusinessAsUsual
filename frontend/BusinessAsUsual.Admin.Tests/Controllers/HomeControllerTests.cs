using BusinessAsUsual.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        var controller = new HomeController(httpClientFactory.Object, logger.Object);

        // Mock HttpContext and Session
        var httpContext = new DefaultHttpContext();
        var session = new Mock<ISession>();

        // Setup session TryGetValue to return false (no session data)
        byte[] value;
        session.Setup(s => s.TryGetValue(It.IsAny<string>(), out value)).Returns(false);

        httpContext.Session = session.Object;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
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
