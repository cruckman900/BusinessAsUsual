using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace BusinessAsUsual.Admin.Tests.Integration;

public class MonitoringIntegrationTests : IClassFixture<CustomAdminApplicationFactory>
{
    private readonly CustomAdminApplicationFactory _factory;

    public MonitoringIntegrationTests(CustomAdminApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task MonitoringPage_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Admin/Monitoring");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MetricsPage_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Admin/Metrics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LogsPage_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Admin/Logs");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
