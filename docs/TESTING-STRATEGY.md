# BusinessAsUsual - Comprehensive Testing Strategy & Implementation Guide

## Executive Summary
This document outlines the complete testing strategy for the BusinessAsUsual solution, targeting >90% code coverage across all modules.

## Current Testing Status

### ✅ Completed: Sales Module
- **Sales.Tests**: 23 unit tests (all passing)
- **Sales.IntegrationTests**: 17 integration tests (all passing)
- **Total**: 40 tests
- **Coverage**: Services, API controllers, event publishing
- **Pattern**: Repository-based services with Moq

### 🚧 In Progress: Finance Module
- **Finance.Tests**: 9 existing tests (all passing)
- **Pattern**: In-memory Mock services (no repositories)
- **Services**: MockInvoiceService, MockBillService, MockBankingService, MockGeneralLedgerService, MockPaymentService, MockVendorPaymentService, PayrollService
- **Status**: Need integration tests for Finance.API

### 📋 Modules Requiring Test Coverage

| Module | Has Tests | Test Count | Integration Tests | Priority |
|--------|-----------|------------|-------------------|----------|
| ✅ Sales | Yes | 40 | Yes | Complete |
| 🟡 Finance | Yes | 9 | No | High |
| 🟡 CRM | Yes | ? | No | High |
| 🟡 HR | Yes | ? | No | High |
| 🟡 Inventory | Yes | ? | No | Medium |
| 🟡 ModuleRegistry | Yes | ? | No | Medium |
| 🟡 AI | Yes | ? | No | Medium |
| ❌ Backend API | No | 0 | No | High |

## Testing Architecture

### 1. Unit Tests
**Purpose**: Test business logic in isolation
**Tools**: xUnit, Moq, FluentAssertions
**Pattern**:
```csharp
public class ServiceTests
{
	private readonly Mock<IRepository> _repositoryMock;
	private readonly Mock<IEventBus> _eventBusMock;
	private readonly Service _service;

	[Fact]
	public async Task MethodName_Scenario_ExpectedResult()
	{
		// Arrange
		_repositoryMock.Setup(x => x.Method()).ReturnsAsync(data);

		// Act
		var result = await _service.Method();

		// Assert
		result.Should().NotBeNull();
		result.Property.Should().Be(expected);
	}
}
```

### 2. Integration Tests
**Purpose**: Test API endpoints end-to-end
**Tools**: xUnit, Microsoft.AspNetCore.Mvc.Testing, FluentAssertions
**Pattern**:
```csharp
public class ControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public ControllerTests(WebApplicationFactory<Program> factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task PostEndpoint_ValidRequest_ReturnsCreated()
	{
		// Arrange
		var request = new CreateDto { /* ... */ };

		// Act
		var response = await _client.PostAsJsonAsync("/api/resource", request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var result = await response.Content.ReadFromJsonAsync<Dto>();
		result.Should().NotBeNull();
	}
}
```

### 3. Component Tests (Blazor)
**Purpose**: Test UI components
**Tools**: bUnit, xUnit
**Status**: Deferred to Phase 2

## Implementation Roadmap

### Phase 1: Core Module Testing (High Priority)
1. **Finance** - Create Finance.IntegrationTests
   - Invoice API endpoints
   - Bill API endpoints
   - Banking API endpoints
   - Payment API endpoints

2. **CRM** - Audit & Expand
   - Customer/Opportunity services
   - Lead scoring services
   - Email template services
   - Create CRM.IntegrationTests

3. **HR** - Audit & Expand
   - Employee services
   - Department services
   - Payroll services
   - Create HR.IntegrationTests

4. **Inventory** - Audit & Expand
   - Product services
   - Purchase order services
   - Create Inventory.IntegrationTests

### Phase 2: Supporting Modules
5. **ModuleRegistry** - Expand existing tests
6. **AI** - Expand existing tests
7. **Backend API** - Create integration tests for platform services

### Phase 3: UI & E2E
8. **Blazor Components** - bUnit tests for critical forms/components
9. **End-to-End** - Playwright/Selenium for critical user workflows

## Quick Start Templates

### Creating Integration Tests for a Module

1. **Create Integration Test Project**:
```bash
dotnet new xunit -n Module.IntegrationTests -o services/Module/Module.IntegrationTests
cd services/Module/Module.IntegrationTests
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package FluentAssertions
dotnet add reference ../Module.API/Module.API.csproj
dotnet sln add Module.IntegrationTests.csproj
```

2. **Add Program marker to Module.API/Program.cs**:
```csharp
// At end of Program.cs
public partial class Program { }
```

3. **Create Controller Test File**:
```csharp
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Module.Application.DTOs;

namespace Module.IntegrationTests.Controllers;

public class ResourceControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public ResourceControllerTests(WebApplicationFactory<Program> factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task GetAll_ShouldReturnOk()
	{
		var response = await _client.GetAsync("/api/module/resources");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task Create_ValidRequest_ShouldReturnCreated()
	{
		var request = new CreateResourceDto { /* ... */ };
		var response = await _client.PostAsJsonAsync("/api/module/resources", request);
		response.StatusCode.Should().Be(HttpStatusCode.Created);
	}
}
```

4. **Run Tests**:
```bash
dotnet test
```

## Coverage Analysis Commands

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Generate HTML Coverage Report
```bash
# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

### View Coverage
Open `coverage-report/index.html` in browser

## Module-Specific Notes

### Finance Module
- Uses in-memory Mock services (no Entity Framework)
- FinanceDataStore holds sample data
- Services: Invoice, Bill, Payment, Banking, GL, Payroll, Collections
- API routes: `/api/finance/*`

### Sales Module (Reference Implementation)
- Uses EF Core repositories
- OrderService, QuoteService
- API routes: `/api/sales/*`
- Events: OrderCreated, OrderConfirmed, OrderShipped

### CRM Module
- MockOpportunityAndCustomerService
- MockLeadService
- MockActivityService
- API routes: `/api/crm/*`

### HR Module
- EmployeeService, DepartmentService, TimekeepingService
- PayrollService (in Finance module)
- API routes: `/api/hr/*`

### Inventory Module
- ProductService, PurchaseOrderService
- API routes: `/api/inventory/*`

## Success Metrics

### Target Coverage Goals
- **Unit Test Coverage**: >90% of Application/Domain logic
- **Integration Test Coverage**: 100% of critical API endpoints
- **Overall Coverage**: >90% solution-wide

### Quality Gates
- All tests must pass before merge
- No decrease in coverage percentage
- New features require tests
- Bug fixes require regression tests

## Next Steps

1. ✅ Complete Sales module (DONE - 40 tests passing)
2. 🔄 Complete Finance integration tests (IN PROGRESS)
3. ⏭️ Audit and expand CRM, HR, Inventory tests
4. ⏭️ Create integration test projects for all modules
5. ⏭️ Run coverage analysis and fill gaps
6. ⏭️ Add bUnit component tests for Blazor UIs
7. ⏭️ Document testing best practices for team

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [bUnit Documentation](https://bunit.dev/)
- [WebApplicationFactory Guide](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)

## Appendix: Test Naming Conventions

### Pattern
`MethodName_Scenario_ExpectedResult`

### Examples
- `GetOrderById_WithValidId_ShouldReturnOrder`
- `CreateInvoice_WithNegativeAmount_ShouldThrowException`
- `SendQuote_WhenDraft_ShouldChangeStatusToSent`

### Categories
Use `[Trait("Category", "X")]` to organize tests:
- `Unit` - Pure unit tests
- `Integration` - API integration tests
- `Functional` - Cross-module workflows
- `Component` - UI component tests
- `E2E` - End-to-end user scenarios

