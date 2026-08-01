using Finance.Application.DTOs;

namespace Finance.Application.Services;

public class MockGeneralLedgerService : IGeneralLedgerService
{
    private readonly List<AccountDto> _accounts = new();
    private readonly List<JournalEntryDto> _journalEntries = new();
    private int _nextAccountId = 1;
    private int _nextJournalEntryId = 1;
    private int _nextJournalLineId = 1;

    public MockGeneralLedgerService()
    {
        SeedChartOfAccounts();
        SeedJournalEntries();
    }

    private void SeedChartOfAccounts()
    {
        // Assets
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "1000", AccountName = "Cash", AccountType = AccountType.Asset, Balance = 50000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "1100", AccountName = "Accounts Receivable", AccountType = AccountType.Asset, Balance = 25000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "1200", AccountName = "Inventory", AccountType = AccountType.Asset, Balance = 15000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "1500", AccountName = "Equipment", AccountType = AccountType.Asset, Balance = 30000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "1510", AccountName = "Accumulated Depreciation - Equipment", AccountType = AccountType.Asset, Balance = -5000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });

        // Liabilities
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "2000", AccountName = "Accounts Payable", AccountType = AccountType.Liability, Balance = 12000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "2100", AccountName = "Accrued Expenses", AccountType = AccountType.Liability, Balance = 3000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "2500", AccountName = "Long-term Debt", AccountType = AccountType.Liability, Balance = 20000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });

        // Equity
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "3000", AccountName = "Owner's Capital", AccountType = AccountType.Equity, Balance = 75000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "3900", AccountName = "Retained Earnings", AccountType = AccountType.Equity, Balance = 5000, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });

        // Revenue
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "4000", AccountName = "Sales Revenue", AccountType = AccountType.Revenue, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "4100", AccountName = "Service Revenue", AccountType = AccountType.Revenue, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });

        // Expenses
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "5000", AccountName = "Cost of Goods Sold", AccountType = AccountType.Expense, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "6000", AccountName = "Salaries Expense", AccountType = AccountType.Expense, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "6100", AccountName = "Rent Expense", AccountType = AccountType.Expense, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "6200", AccountName = "Utilities Expense", AccountType = AccountType.Expense, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "6300", AccountName = "Office Supplies Expense", AccountType = AccountType.Expense, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
        _accounts.Add(new AccountDto { Id = _nextAccountId++, AccountNumber = "6400", AccountName = "Depreciation Expense", AccountType = AccountType.Expense, Balance = 0, IsActive = true, CreatedDate = DateTime.Now.AddMonths(-6) });
    }

    private void SeedJournalEntries()
    {
        // Sample posted journal entry
        var cashAccount = _accounts.First(a => a.AccountNumber == "1000");
        var revenueAccount = _accounts.First(a => a.AccountNumber == "4000");
        var arAccount = _accounts.First(a => a.AccountNumber == "1100");

        _journalEntries.Add(new JournalEntryDto
        {
            Id = _nextJournalEntryId++,
            EntryNumber = "JE-001",
            EntryDate = DateTime.Now.AddDays(-10),
            Description = "Record sales revenue",
            Reference = "INV-1001",
            Source = JournalEntrySource.Invoice,
            Status = JournalEntryStatus.Posted,
            Lines = new List<JournalLineDto>
            {
                new() { Id = _nextJournalLineId++, LineNumber = 1, AccountId = arAccount.Id, AccountNumber = arAccount.AccountNumber, AccountName = arAccount.AccountName, DebitAmount = 5000, CreditAmount = 0, Description = "Invoice #1001" },
                new() { Id = _nextJournalLineId++, LineNumber = 2, AccountId = revenueAccount.Id, AccountNumber = revenueAccount.AccountNumber, AccountName = revenueAccount.AccountName, DebitAmount = 0, CreditAmount = 5000, Description = "Sales revenue" }
            },
            TotalDebits = 5000,
            TotalCredits = 5000,
            CreatedBy = "System",
            CreatedDate = DateTime.Now.AddDays(-10),
            PostedDate = DateTime.Now.AddDays(-10)
        });

        // Sample draft journal entry
        var rentAccount = _accounts.First(a => a.AccountNumber == "6100");

        _journalEntries.Add(new JournalEntryDto
        {
            Id = _nextJournalEntryId++,
            EntryNumber = "JE-002",
            EntryDate = DateTime.Now,
            Description = "Record monthly rent",
            Reference = "RENT-DEC",
            Source = JournalEntrySource.Manual,
            Status = JournalEntryStatus.Draft,
            Lines = new List<JournalLineDto>
            {
                new() { Id = _nextJournalLineId++, LineNumber = 1, AccountId = rentAccount.Id, AccountNumber = rentAccount.AccountNumber, AccountName = rentAccount.AccountName, DebitAmount = 2000, CreditAmount = 0, Description = "December rent" },
                new() { Id = _nextJournalLineId++, LineNumber = 2, AccountId = cashAccount.Id, AccountNumber = cashAccount.AccountNumber, AccountName = cashAccount.AccountName, DebitAmount = 0, CreditAmount = 2000, Description = "Cash payment" }
            },
            TotalDebits = 2000,
            TotalCredits = 2000,
            CreatedBy = "Admin",
            CreatedDate = DateTime.Now
        });
    }

    // Chart of Accounts
    public Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
    {
        return Task.FromResult(_accounts.Where(a => a.IsActive).AsEnumerable());
    }

    public Task<AccountDto?> GetAccountByIdAsync(int id)
    {
        return Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
    }

    public Task<AccountDto> CreateAccountAsync(CreateAccountRequest request)
    {
        var account = new AccountDto
        {
            Id = _nextAccountId++,
            AccountNumber = request.AccountNumber,
            AccountName = request.AccountName,
            AccountType = request.AccountType,
            Description = request.Description,
            ParentAccountId = request.ParentAccountId,
            Balance = 0,
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        if (request.ParentAccountId.HasValue)
        {
            var parent = _accounts.FirstOrDefault(a => a.Id == request.ParentAccountId.Value);
            account.ParentAccountName = parent?.AccountName;
        }

        _accounts.Add(account);
        return Task.FromResult(account);
    }

    public Task<AccountDto> UpdateAccountAsync(int id, UpdateAccountRequest request)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException("Account not found");

        account.AccountName = request.AccountName;
        account.Description = request.Description;
        account.IsActive = request.IsActive;

        return Task.FromResult(account);
    }

    public Task DeleteAccountAsync(int id)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException("Account not found");

        if (account.Balance != 0)
            throw new InvalidOperationException("Cannot delete account with non-zero balance");

        account.IsActive = false;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<AccountDto>> GetAccountsByTypeAsync(AccountType type)
    {
        return Task.FromResult(_accounts.Where(a => a.AccountType == type && a.IsActive).AsEnumerable());
    }

    // Journal Entries
    public Task<IEnumerable<JournalEntryDto>> GetAllJournalEntriesAsync()
    {
        return Task.FromResult(_journalEntries.AsEnumerable());
    }

    public Task<JournalEntryDto?> GetJournalEntryByIdAsync(int id)
    {
        return Task.FromResult(_journalEntries.FirstOrDefault(e => e.Id == id));
    }

    public Task<JournalEntryDto> CreateJournalEntryAsync(CreateJournalEntryRequest request)
    {
        var entry = new JournalEntryDto
        {
            Id = _nextJournalEntryId++,
            EntryNumber = $"JE-{_nextJournalEntryId:000}",
            EntryDate = request.EntryDate,
            Description = request.Description,
            Reference = request.Reference,
            Source = JournalEntrySource.Manual,
            Status = JournalEntryStatus.Draft,
            Lines = new List<JournalLineDto>(),
            CreatedBy = "User",
            CreatedDate = DateTime.Now
        };

        int lineNumber = 1;
        foreach (var lineRequest in request.Lines)
        {
            var account = _accounts.First(a => a.Id == lineRequest.AccountId);
            entry.Lines.Add(new JournalLineDto
            {
                Id = _nextJournalLineId++,
                LineNumber = lineNumber++,
                AccountId = lineRequest.AccountId,
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                DebitAmount = lineRequest.DebitAmount,
                CreditAmount = lineRequest.CreditAmount,
                Description = lineRequest.Description
            });
        }

        entry.TotalDebits = entry.Lines.Sum(l => l.DebitAmount);
        entry.TotalCredits = entry.Lines.Sum(l => l.CreditAmount);

        if (!entry.IsBalanced)
            throw new InvalidOperationException("Journal entry debits and credits must be equal");

        _journalEntries.Add(entry);
        return Task.FromResult(entry);
    }

    public Task PostJournalEntryAsync(int id)
    {
        var entry = _journalEntries.FirstOrDefault(e => e.Id == id)
            ?? throw new InvalidOperationException("Journal entry not found");

        if (entry.Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Only draft entries can be posted");

        if (!entry.IsBalanced)
            throw new InvalidOperationException("Cannot post unbalanced entry");

        entry.Status = JournalEntryStatus.Posted;
        entry.PostedDate = DateTime.Now;

        // Update account balances
        foreach (var line in entry.Lines)
        {
            var account = _accounts.First(a => a.Id == line.AccountId);

            // Asset, Expense = Debit increases
            if (account.AccountType == AccountType.Asset || account.AccountType == AccountType.Expense)
            {
                account.Balance += line.DebitAmount - line.CreditAmount;
            }
            // Liability, Equity, Revenue = Credit increases
            else
            {
                account.Balance += line.CreditAmount - line.DebitAmount;
            }
        }

        return Task.CompletedTask;
    }

    public Task VoidJournalEntryAsync(int id)
    {
        var entry = _journalEntries.FirstOrDefault(e => e.Id == id)
            ?? throw new InvalidOperationException("Journal entry not found");

        if (entry.Status == JournalEntryStatus.Void)
            throw new InvalidOperationException("Entry is already voided");

        // If posted, reverse the account balances
        if (entry.Status == JournalEntryStatus.Posted)
        {
            foreach (var line in entry.Lines)
            {
                var account = _accounts.First(a => a.Id == line.AccountId);

                if (account.AccountType == AccountType.Asset || account.AccountType == AccountType.Expense)
                {
                    account.Balance -= line.DebitAmount - line.CreditAmount;
                }
                else
                {
                    account.Balance -= line.CreditAmount - line.DebitAmount;
                }
            }
        }

        entry.Status = JournalEntryStatus.Void;
        return Task.CompletedTask;
    }

    // Trial Balance
    public Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime asOfDate)
    {
        var trialBalance = new TrialBalanceDto
        {
            AsOfDate = asOfDate,
            Lines = new List<TrialBalanceLineDto>()
        };

        foreach (var account in _accounts.Where(a => a.IsActive).OrderBy(a => a.AccountNumber))
        {
            var line = new TrialBalanceLineDto
            {
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                AccountType = account.AccountType
            };

            if (account.Balance >= 0)
            {
                if (account.AccountType == AccountType.Asset || account.AccountType == AccountType.Expense)
                    line.DebitBalance = account.Balance;
                else
                    line.CreditBalance = account.Balance;
            }
            else
            {
                if (account.AccountType == AccountType.Asset || account.AccountType == AccountType.Expense)
                    line.CreditBalance = Math.Abs(account.Balance);
                else
                    line.DebitBalance = Math.Abs(account.Balance);
            }

            trialBalance.Lines.Add(line);
        }

        trialBalance.TotalDebits = trialBalance.Lines.Sum(l => l.DebitBalance);
        trialBalance.TotalCredits = trialBalance.Lines.Sum(l => l.CreditBalance);

        return Task.FromResult(trialBalance);
    }

    // Account History
    public Task<AccountHistoryDto> GetAccountHistoryAsync(int accountId, DateTime fromDate, DateTime toDate)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == accountId)
            ?? throw new InvalidOperationException("Account not found");

        var history = new AccountHistoryDto
        {
            AccountId = account.Id,
            AccountNumber = account.AccountNumber,
            AccountName = account.AccountName,
            FromDate = fromDate,
            ToDate = toDate,
            BeginningBalance = 0,
            Transactions = new List<AccountHistoryLineDto>()
        };

        var relevantEntries = _journalEntries
            .Where(e => e.Status == JournalEntryStatus.Posted && 
                       e.EntryDate >= fromDate && 
                       e.EntryDate <= toDate &&
                       e.Lines.Any(l => l.AccountId == accountId))
            .OrderBy(e => e.EntryDate);

        decimal runningBalance = history.BeginningBalance;

        foreach (var entry in relevantEntries)
        {
            var line = entry.Lines.First(l => l.AccountId == accountId);

            decimal change = 0;
            if (account.AccountType == AccountType.Asset || account.AccountType == AccountType.Expense)
            {
                change = line.DebitAmount - line.CreditAmount;
            }
            else
            {
                change = line.CreditAmount - line.DebitAmount;
            }

            runningBalance += change;

            history.Transactions.Add(new AccountHistoryLineDto
            {
                Date = entry.EntryDate,
                EntryNumber = entry.EntryNumber,
                Description = line.Description,
                DebitAmount = line.DebitAmount,
                CreditAmount = line.CreditAmount,
                RunningBalance = runningBalance
            });
        }

        history.EndingBalance = runningBalance;
        return Task.FromResult(history);
    }
}
