using Finance.Application.DTOs;
using Finance.Application.Services;
using FluentAssertions;

namespace Finance.Tests.Unit;

public class GeneralLedgerServiceTests
{
    private static MockGeneralLedgerService NewService()
    {
        return new MockGeneralLedgerService();
    }

    [Fact]
    public async Task GetAllAccountsAsync_ShouldReturnSeededAccounts()
    {
        // Arrange
        var service = NewService();

        // Act
        var accounts = (await service.GetAllAccountsAsync()).ToList();

        // Assert
        accounts.Should().NotBeEmpty();
        accounts.Should().HaveCountGreaterThanOrEqualTo(10);
    }

    [Fact]
    public async Task GetAllJournalEntriesAsync_ShouldReturnSeededEntries()
    {
        // Arrange
        var service = NewService();

        // Act
        var entries = (await service.GetAllJournalEntriesAsync()).ToList();

        // Assert
        entries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldCreateAccount()
    {
        // Arrange
        var service = NewService();
        var request = new CreateAccountRequest
        {
            AccountNumber = "7000",
            AccountName = "Test Account",
            AccountType = AccountType.Expense,
            Description = "Test expense account",
            ParentAccountId = null
        };

        // Act
        var result = await service.CreateAccountAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountName.Should().Be("Test Account");
        result.AccountNumber.Should().Be("7000");
    }

    [Fact]
    public async Task CreateJournalEntryAsync_ShouldCreateBalancedEntry()
    {
        // Arrange
        var service = NewService();
        var accounts = (await service.GetAllAccountsAsync()).ToList();
        var assetAccount = accounts.First(a => a.AccountType == AccountType.Asset);
        var revenueAccount = accounts.First(a => a.AccountType == AccountType.Revenue);

        var request = new CreateJournalEntryRequest
        {
            EntryDate = DateTime.UtcNow,
            Description = "Test Entry",
            Reference = "TEST-001",
            Lines = new List<CreateJournalLineRequest>
            {
                new() { AccountId = assetAccount.Id, DebitAmount = 100m, CreditAmount = 0 },
                new() { AccountId = revenueAccount.Id, DebitAmount = 0, CreditAmount = 100m }
            }
        };

        // Act
        var result = await service.CreateJournalEntryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalDebits.Should().Be(100m);
        result.TotalCredits.Should().Be(100m);
        result.Lines.Should().HaveCount(2);
    }

    [Fact]
    public async Task PostJournalEntryAsync_ShouldChangeStatusToPosted()
    {
        // Arrange
        var service = NewService();
        var entries = (await service.GetAllJournalEntriesAsync()).ToList();
        var draftEntry = entries.FirstOrDefault(je => je.Status == JournalEntryStatus.Draft);

        if (draftEntry == null)
        {
            // Create a draft entry first
            var accounts = (await service.GetAllAccountsAsync()).ToList();
            var assetAccount = accounts.First(a => a.AccountType == AccountType.Asset);
            var revenueAccount = accounts.First(a => a.AccountType == AccountType.Revenue);

            var createRequest = new CreateJournalEntryRequest
            {
                EntryDate = DateTime.UtcNow,
                Description = "Test Draft Entry",
                Reference = "DRAFT-001",
                Lines = new List<CreateJournalLineRequest>
                {
                    new() { AccountId = assetAccount.Id, DebitAmount = 50m, CreditAmount = 0 },
                    new() { AccountId = revenueAccount.Id, DebitAmount = 0, CreditAmount = 50m }
                }
            };
            draftEntry = await service.CreateJournalEntryAsync(createRequest);
        }

        // Act
        await service.PostJournalEntryAsync(draftEntry.Id);

        // Assert
        var updatedEntry = await service.GetJournalEntryByIdAsync(draftEntry.Id);
        updatedEntry.Should().NotBeNull();
        updatedEntry!.Status.Should().Be(JournalEntryStatus.Posted);
    }

    [Fact]
    public async Task GetTrialBalanceAsync_ShouldReturnBalancedResults()
    {
        // Arrange
        var service = NewService();
        var asOfDate = DateTime.UtcNow;

        // Act
        var result = await service.GetTrialBalanceAsync(asOfDate);

        // Assert
        result.Should().NotBeNull();
        result.TotalDebits.Should().Be(result.TotalCredits);
    }
}
