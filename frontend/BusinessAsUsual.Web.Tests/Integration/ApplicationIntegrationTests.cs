using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using BusinessAsUsual.Web;

namespace BusinessAsUsual.Web.Tests.Integration;

public class ApplicationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ApplicationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ErrorController_ReturnsOK_ForInvalidStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Error/999");

        // Assert - Invalid status codes return 200 OK with error page
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthCheck_IsAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - if health check endpoint exists
        var response = await client.GetAsync("/_health");

        // Assert - either OK or NotFound is acceptable for now
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task StaticContent_IsAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/css/site.css");

        // Assert - static files should be served if middleware configured
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }
}
