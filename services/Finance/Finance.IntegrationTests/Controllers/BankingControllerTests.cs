using System.Net;
using System.Net.Http.Json;
using Finance.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Finance.IntegrationTests.Controllers;

public class BankingControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BankingControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllAccounts_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/banking/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accounts = await response.Content.ReadFromJsonAsync<List<BankAccountDto>>();
        accounts.Should().NotBeNull();
        accounts.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAccountById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var allResponse = await _client.GetAsync("/api/banking/accounts");
        var accounts = await allResponse.Content.ReadFromJsonAsync<List<BankAccountDto>>();
        var firstAccount = accounts!.First();

        // Act
        var response = await _client.GetAsync($"/api/banking/accounts/{firstAccount.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var account = await response.Content.ReadFromJsonAsync<BankAccountDto>();
        account.Should().NotBeNull();
        account!.Id.Should().Be(firstAccount.Id);
    }

    [Fact]
    public async Task CreateAccount_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateBankAccountRequest
        {
            AccountName = "Integration Test Account",
            AccountType = "Checking",
            Currency = "USD"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/banking/accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var account = await response.Content.ReadFromJsonAsync<BankAccountDto>();
        account.Should().NotBeNull();
        account!.AccountName.Should().Be("Integration Test Account");
    }

    [Fact]
    public async Task GetAllTransactions_ShouldReturnOk()
    {
        // Arrange - Get an account first
        var accountsResponse = await _client.GetAsync("/api/banking/accounts");
        var accounts = await accountsResponse.Content.ReadFromJsonAsync<List<BankAccountDto>>();
        var account = accounts!.First();

        // Act
        var response = await _client.GetAsync($"/api/banking/accounts/{account.Id}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<BankTransactionDto>>();
        transactions.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTransaction_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange - Get an account first
        var accountsResponse = await _client.GetAsync("/api/banking/accounts");
        var accounts = await accountsResponse.Content.ReadFromJsonAsync<List<BankAccountDto>>();
        var account = accounts!.First();

        var request = new CreateBankTransactionRequest
        {
            BankAccountId = account.Id,
            TransactionDate = DateTime.UtcNow,
            Type = "Deposit",
            Amount = 1000m,
            Description = "Integration test deposit"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/banking/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var transaction = await response.Content.ReadFromJsonAsync<BankTransactionDto>();
        transaction.Should().NotBeNull();
        transaction!.Amount.Should().Be(1000m);
    }

    [Fact]
    public async Task ReconcileAccount_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var accountsResponse = await _client.GetAsync("/api/banking/accounts");
        var accounts = await accountsResponse.Content.ReadFromJsonAsync<List<BankAccountDto>>();
        var account = accounts!.First();

        // Act
        var response = await _client.PostAsync(
            $"/api/banking/accounts/{account.Id}/reconcile?reconciledDate={DateTime.UtcNow:O}",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
