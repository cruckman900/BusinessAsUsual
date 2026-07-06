using System.Net;
using System.Net.Http.Json;
using HR.Application.DTOs;

namespace HR.Tests.Functional;

/// <summary>
/// Functional tests exercising the HR API HTTP surface end-to-end via
/// <see cref="HrApiFactory"/> (in-memory database, stubbed module registration).
/// </summary>
public class EmployeesApiTests : IClassFixture<HrApiFactory>
{
    private readonly HrApiFactory _factory;

    public EmployeesApiTests(HrApiFactory factory)
    {
        _factory = factory;
    }

    private static CreateEmployeeRequest NewEmployee(string email) => new()
    {
        FirstName = "Api",
        LastName = "Tester",
        Email = email,
        JobTitle = "QA",
        EmploymentType = "FullTime",
        Status = "Active",
        HireDate = DateTime.UtcNow
    };

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetEmployees_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/hr/employees");

        response.EnsureSuccessStatusCode();
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetEmployee_ReturnsNotFound_WhenMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/hr/employees/{Guid.NewGuid():N}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Create_Then_Get_RoundTrips()
    {
        var client = _factory.CreateClient();

        var create = await client.PostAsJsonAsync("/api/hr/employees", NewEmployee($"api-{Guid.NewGuid():N}@example.com"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.NotNull(created);

        var get = await client.GetAsync($"/api/hr/employees/{created!.Id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Update_ReturnsNotFound_ForMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/hr/employees/{Guid.NewGuid():N}", new UpdateEmployeeRequest
        {
            FirstName = "X",
            LastName = "Y",
            Email = "x@y.com",
            EmploymentType = "FullTime",
            Status = "Active"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var create = await client.PostAsJsonAsync("/api/hr/employees", NewEmployee($"del-{Guid.NewGuid():N}@example.com"));
        var created = await create.Content.ReadFromJsonAsync<EmployeeDto>();

        var response = await client.DeleteAsync($"/api/hr/employees/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Search_ReturnsOk()
    {
        var client = _factory.CreateClient();
        await client.PostAsJsonAsync("/api/hr/employees", NewEmployee($"search-{Guid.NewGuid():N}@example.com"));

        var response = await client.GetAsync("/api/hr/employees/search?q=Tester");

        response.EnsureSuccessStatusCode();
        var results = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
        Assert.NotNull(results);
    }
}
