using Finance.Application.DTOs;
using Finance.Application.Services;
using FluentAssertions;

namespace Finance.Tests.Unit;

public class BankingServiceTests
{
    private static MockBankingService NewService()
    {
        return new MockBankingService();
    }

    [Fact]
    public async Task GetAllBankAccountsAsync_ShouldReturnSeededData()
    {
        // Arrange
        var service = NewService();

        // Act
        var accounts = (await service.GetAllBankAccountsAsync()).ToList();

        // Assert
        accounts.Should().NotBeEmpty();
        accounts.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetTransactionsByAccountAsync_ShouldReturnSeededData()
    {
        // Arrange
        var service = NewService();
        var accounts = (await service.GetAllBankAccountsAsync()).ToList();
        var firstAccount = accounts.First();

        // Act
        var transactions = (await service.GetTransactionsByAccountAsync(firstAccount.Id)).ToList();

        // Assert
        transactions.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateBankAccountAsync_ShouldCreateWithOpeningBalance()
    {
        // Arrange
        var service = NewService();
        var request = new CreateBankAccountRequest
        {
            AccountName = "Test Account",
            AccountType = "Checking",
            Currency = "USD",
            OpeningBalance = 1000m,
            AccountNumber = "TEST-12345",
            BankName = "Test Bank"
        };

        // Act
        var result = await service.CreateBankAccountAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountName.Should().Be("Test Account");
        result.CurrentBalance.Should().Be(1000m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldUpdateAccountBalance()
    {
        // Arrange
        var service = NewService();

        // Create a fresh account to avoid interference from seeded transactions
        var createAccountRequest = new CreateBankAccountRequest
        {
            AccountName = "Test Transaction Account",
            AccountType = "Checking",
            Currency = "USD",
            OpeningBalance = 1000m,
            AccountNumber = "TEST-9999",
            BankName = "Test Bank"
        };
        var account = await service.CreateBankAccountAsync(createAccountRequest);
        var initialBalance = account.CurrentBalance;

        var request = new CreateBankTransactionRequest
        {
            BankAccountId = account.Id,
            Type = "Credit",
            Amount = 500m,
            Description = "Test deposit",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await service.CreateTransactionAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(500m);

        // Verify account balance updated
        var updatedAccount = await service.GetBankAccountByIdAsync(account.Id);
        updatedAccount!.CurrentBalance.Should().Be(initialBalance + 500m);
    }

    [Fact]
    public async Task ReconcileTransactionAsync_ShouldMarkAsReconciled()
    {
        // Arrange
        var service = NewService();
        var accounts = (await service.GetAllBankAccountsAsync()).ToList();
        var account = accounts.First();
        var transactions = (await service.GetTransactionsByAccountAsync(account.Id)).ToList();
        var unreconciledTx = transactions.FirstOrDefault(t => !t.IsReconciled);

        if (unreconciledTx == null)
        {
            // Create an unreconciled transaction first
            var createRequest = new CreateBankTransactionRequest
            {
                BankAccountId = account.Id,
                Type = "Debit",
                Amount = -100m,
                Description = "Test transaction",
                TransactionDate = DateTime.UtcNow
            };
            unreconciledTx = await service.CreateTransactionAsync(createRequest);
        }

        var reconcileRequest = new ReconcileTransactionRequest
        {
            TransactionId = unreconciledTx.Id
        };

        // Act
        var result = await service.ReconcileTransactionAsync(reconcileRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsReconciled.Should().BeTrue();
    }
}
