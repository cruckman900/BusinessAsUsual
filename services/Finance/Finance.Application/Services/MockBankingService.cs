using Finance.Application.DTOs;

namespace Finance.Application.Services;

public class MockBankingService : IBankingService
{
    private readonly List<BankAccountDto> _accounts = new();
    private readonly List<BankTransactionDto> _transactions = new();

    public MockBankingService()
    {
        // Seed sample accounts
        var checkingAccount = new BankAccountDto
        {
            Id = Guid.NewGuid().ToString(),
            AccountName = "Operating Account",
            AccountNumber = "****1234",
            BankName = "First National Bank",
            AccountType = "Checking",
            Currency = "USD",
            CurrentBalance = 125000m,
            AvailableBalance = 120000m,
            IsActive = true,
            CreatedDate = DateTime.Now.AddMonths(-6),
            LastReconciledDate = DateTime.Now.AddDays(-7)
        };

        var savingsAccount = new BankAccountDto
        {
            Id = Guid.NewGuid().ToString(),
            AccountName = "Reserve Account",
            AccountNumber = "****5678",
            BankName = "First National Bank",
            AccountType = "Savings",
            Currency = "USD",
            CurrentBalance = 50000m,
            AvailableBalance = 50000m,
            IsActive = true,
            CreatedDate = DateTime.Now.AddMonths(-6)
        };

        _accounts.Add(checkingAccount);
        _accounts.Add(savingsAccount);

        // Seed sample transactions for checking account
        decimal runningBalance = 100000m;

        _transactions.Add(new BankTransactionDto
        {
            Id = Guid.NewGuid().ToString(),
            BankAccountId = checkingAccount.Id,
            BankAccountName = checkingAccount.AccountName,
            TransactionDate = DateTime.Now.AddDays(-10),
            Description = "Customer Payment - INV-001",
            Reference = "TXN-12345",
            Amount = 25000m,
            Type = "Credit",
            RunningBalance = runningBalance + 25000m,
            Category = "Customer Payment",
            IsReconciled = true,
            CreatedDate = DateTime.Now.AddDays(-10)
        });
        runningBalance += 25000m;

        _transactions.Add(new BankTransactionDto
        {
            Id = Guid.NewGuid().ToString(),
            BankAccountId = checkingAccount.Id,
            BankAccountName = checkingAccount.AccountName,
            TransactionDate = DateTime.Now.AddDays(-5),
            Description = "Vendor Payment - Office Supplies",
            Reference = "CHK-98765",
            Amount = -3000m,
            Type = "Debit",
            RunningBalance = runningBalance - 3000m,
            Category = "Vendor Payment",
            IsReconciled = false,
            CreatedDate = DateTime.Now.AddDays(-5)
        });
        runningBalance -= 3000m;

        _transactions.Add(new BankTransactionDto
        {
            Id = Guid.NewGuid().ToString(),
            BankAccountId = checkingAccount.Id,
            BankAccountName = checkingAccount.AccountName,
            TransactionDate = DateTime.Now.AddDays(-2),
            Description = "Payroll Processing",
            Reference = "PAYROLL-2024-01",
            Amount = -15000m,
            Type = "Debit",
            RunningBalance = runningBalance - 15000m,
            Category = "Payroll",
            IsReconciled = false,
            CreatedDate = DateTime.Now.AddDays(-2)
        });
    }

    public Task<IEnumerable<BankAccountDto>> GetAllBankAccountsAsync()
        => Task.FromResult<IEnumerable<BankAccountDto>>(_accounts);

    public Task<BankAccountDto?> GetBankAccountByIdAsync(string id)
        => Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));

    public Task<BankAccountDto> CreateBankAccountAsync(CreateBankAccountRequest request)
    {
        var account = new BankAccountDto
        {
            Id = Guid.NewGuid().ToString(),
            AccountName = request.AccountName,
            AccountNumber = request.AccountNumber,
            BankName = request.BankName,
            AccountType = request.AccountType,
            Currency = request.Currency,
            CurrentBalance = request.OpeningBalance,
            AvailableBalance = request.OpeningBalance,
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        _accounts.Add(account);
        return Task.FromResult(account);
    }

    public Task<BankAccountDto> UpdateBankAccountAsync(string id, CreateBankAccountRequest request)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        if (account == null)
            throw new KeyNotFoundException($"Bank account {id} not found");

        account.AccountName = request.AccountName;
        account.AccountNumber = request.AccountNumber;
        account.BankName = request.BankName;
        account.AccountType = request.AccountType;
        account.Currency = request.Currency;

        return Task.FromResult(account);
    }

    public Task DeleteBankAccountAsync(string id)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        if (account != null)
        {
            account.IsActive = false;
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<BankTransactionDto>> GetTransactionsByAccountAsync(string accountId)
        => Task.FromResult<IEnumerable<BankTransactionDto>>(
            _transactions.Where(t => t.BankAccountId == accountId).OrderByDescending(t => t.TransactionDate));

    public Task<BankTransactionDto?> GetTransactionByIdAsync(string id)
        => Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));

    public Task<BankTransactionDto> CreateTransactionAsync(CreateBankTransactionRequest request)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == request.BankAccountId);
        if (account == null)
            throw new KeyNotFoundException($"Bank account {request.BankAccountId} not found");

        var lastTransaction = _transactions
            .Where(t => t.BankAccountId == request.BankAccountId)
            .OrderByDescending(t => t.TransactionDate)
            .FirstOrDefault();

        var runningBalance = lastTransaction?.RunningBalance ?? account.CurrentBalance;
        runningBalance += request.Amount;

        var transaction = new BankTransactionDto
        {
            Id = Guid.NewGuid().ToString(),
            BankAccountId = request.BankAccountId,
            BankAccountName = account.AccountName,
            TransactionDate = request.TransactionDate,
            Description = request.Description,
            Reference = request.Reference,
            Amount = request.Amount,
            Type = request.Type,
            RunningBalance = runningBalance,
            Category = request.Category,
            IsReconciled = false,
            CreatedDate = DateTime.Now
        };

        _transactions.Add(transaction);
        account.CurrentBalance = runningBalance;
        account.AvailableBalance = runningBalance;

        return Task.FromResult(transaction);
    }

    public Task DeleteTransactionAsync(string id)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        if (transaction != null)
            _transactions.Remove(transaction);

        return Task.CompletedTask;
    }

    public Task<BankTransactionDto> ReconcileTransactionAsync(ReconcileTransactionRequest request)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == request.TransactionId);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction {request.TransactionId} not found");

        transaction.IsReconciled = true;
        transaction.MatchedToInvoiceId = request.MatchedToInvoiceId;
        transaction.MatchedToBillId = request.MatchedToBillId;
        transaction.MatchedToPaymentId = request.MatchedToPaymentId;

        var account = _accounts.FirstOrDefault(a => a.Id == transaction.BankAccountId);
        if (account != null)
            account.LastReconciledDate = DateTime.Now;

        return Task.FromResult(transaction);
    }

    public Task<IEnumerable<BankTransactionDto>> GetUnreconciledTransactionsAsync(string accountId)
        => Task.FromResult<IEnumerable<BankTransactionDto>>(
            _transactions.Where(t => t.BankAccountId == accountId && !t.IsReconciled)
                        .OrderByDescending(t => t.TransactionDate));
}
