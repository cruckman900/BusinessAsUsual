using System;
using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Services.Tests.Integration;

public class ServicesApiTests : IClassFixture<WebApplicationFactory<Services.API.Program>>
{
    private readonly WebApplicationFactory<Services.API.Program> _factory;

    public ServicesApiTests(WebApplicationFactory<Services.API.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetServices_ReturnsOkAndArray()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/services");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var list = await res.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(list);
    }
}
