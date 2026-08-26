using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using HR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BusinessAsUsual.Application.Services;
using Moq;

namespace HR.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="EmployeeRepository"/> backed by the EF Core in-memory provider.
/// </summary>
public class EmployeeRepositoryTests
{
    private static readonly Guid TestCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static HRDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<HRDbContext>()
            .UseInMemoryDatabase($"HR_Repo_{Guid.NewGuid():N}")
            .Options);

    private static ITenantContext CreateMockTenantContext()
    {
        var mock = new Mock<ITenantContext>();
        mock.Setup(x => x.CompanyId).Returns(TestCompanyId);
        mock.Setup(x => x.TenantDbName).Returns("TestTenant");
        mock.Setup(x => x.IsResolved).Returns(true);
        return mock.Object;
    }

#pragma warning disable CS0618 // Legacy Department field intentionally exercised for search coverage.
    private static Employee NewEmployee(string first, string last, string email, string? department = null) => new()
    {
        Id = Guid.NewGuid(),
        CompanyId = TestCompanyId,
        FirstName = first,
        LastName = last,
        Email = email,
        Department = department,
        HireDate = DateTime.UtcNow
    };
#pragma warning restore CS0618

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateAsync_And_GetById_Roundtrips()
    {
        using var context = CreateContext();
        var tenantContext = CreateMockTenantContext();
        var repo = new EmployeeRepository(context, tenantContext);
        var employee = NewEmployee("Ada", "Lovelace", "ada@example.com");

        await repo.CreateAsync(employee);
        var fetched = await repo.GetByIdAsync(employee.Id);

        Assert.NotNull(fetched);
        Assert.Equal("ada@example.com", fetched!.Email);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllAsync_ReturnsOrderedByLastNameThenFirst()
    {
        using var context = CreateContext();
        var tenantContext = CreateMockTenantContext();
        var repo = new EmployeeRepository(context, tenantContext);
        await repo.CreateAsync(NewEmployee("Zoe", "Adams", "zoe@example.com"));
        await repo.CreateAsync(NewEmployee("Alan", "Adams", "alan@example.com"));
        await repo.CreateAsync(NewEmployee("Bob", "Baker", "bob@example.com"));

        var all = (await repo.GetAllAsync()).ToList();

        Assert.Equal(3, all.Count);
        Assert.Equal("Adams", all[0].LastName);
        Assert.Equal("Alan", all[0].FirstName); // Alan Adams before Zoe Adams
        Assert.Equal("Baker", all[2].LastName);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        using var context = CreateContext();
        var tenantContext = CreateMockTenantContext();
        var repo = new EmployeeRepository(context, tenantContext);
        var employee = NewEmployee("Grace", "Hopper", "grace@example.com");
        await repo.CreateAsync(employee);

        employee.JobTitle = "Rear Admiral";
        await repo.UpdateAsync(employee);

        var fetched = await repo.GetByIdAsync(employee.Id);
        Assert.Equal("Rear Admiral", fetched!.JobTitle);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteAsync_RemovesEmployee()
    {
        using var context = CreateContext();
        var tenantContext = CreateMockTenantContext();
        var repo = new EmployeeRepository(context, tenantContext);
        var employee = NewEmployee("Delete", "Me", "del@example.com");
        await repo.CreateAsync(employee);

        await repo.DeleteAsync(employee.Id);

        Assert.Null(await repo.GetByIdAsync(employee.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteAsync_IsNoOp_WhenMissing()
    {
        using var context = CreateContext();
        var tenantContext = CreateMockTenantContext();
        var repo = new EmployeeRepository(context, tenantContext);

        var ex = await Record.ExceptionAsync(() => repo.DeleteAsync(Guid.NewGuid()));

        Assert.Null(ex);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SearchAsync_MatchesNameEmailOrDepartment()
    {
        using var context = CreateContext();
        var tenantContext = CreateMockTenantContext();
        var repo = new EmployeeRepository(context, tenantContext);
        await repo.CreateAsync(NewEmployee("Ada", "Lovelace", "ada@example.com", "Engineering"));
        await repo.CreateAsync(NewEmployee("Bob", "Baker", "bob@example.com", "Finance"));

        var byName = (await repo.SearchAsync("lovelace")).ToList();
        var byDept = (await repo.SearchAsync("finance")).ToList();

        Assert.Single(byName);
        Assert.Equal("ada@example.com", byName[0].Email);
        Assert.Single(byDept);
        Assert.Equal("bob@example.com", byDept[0].Email);
    }
}
