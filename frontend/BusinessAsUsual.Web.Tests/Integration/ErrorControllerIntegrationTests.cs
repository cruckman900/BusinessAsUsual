using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using BusinessAsUsual.Web;

namespace BusinessAsUsual.Web.Tests.Integration;

public class ErrorControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ErrorControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ErrorPage_Returns_Success_For403()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Error/403");

        // Assert - 403 error pages return 200 OK with error content
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ErrorPage_Returns_InternalServerError_For500()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Error/500");

        // Assert - 500 error pages return actual 500 status
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ErrorPage_Returns_NotFound_For404()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Error/404");

        // Assert - 404 error pages return actual 404 status
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ErrorPage_DefaultRoute_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Error");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
