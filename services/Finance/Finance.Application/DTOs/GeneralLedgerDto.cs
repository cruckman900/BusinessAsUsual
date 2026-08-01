namespace Finance.Application.DTOs;

public class AccountDto
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? ParentAccountId { get; set; }
    public string? ParentAccountName { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
}

public class CreateAccountRequest
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? ParentAccountId { get; set; }
}

public class UpdateAccountRequest
{
    public string AccountName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class JournalEntryDto
{
    public int Id { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public JournalEntrySource Source { get; set; }
    public JournalEntryStatus Status { get; set; }
    public List<JournalLineDto> Lines { get; set; } = new();
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public bool IsBalanced => TotalDebits == TotalCredits;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? PostedDate { get; set; }
}

public class CreateJournalEntryRequest
{
    public DateTime EntryDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public List<CreateJournalLineRequest> Lines { get; set; } = new();
}

public class JournalLineDto
{
    public int Id { get; set; }
    public int LineNumber { get; set; }
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateJournalLineRequest
{
    public int AccountId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class TrialBalanceDto
{
    public DateTime AsOfDate { get; set; }
    public List<TrialBalanceLineDto> Lines { get; set; } = new();
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public bool IsBalanced => TotalDebits == TotalCredits;
}

public class TrialBalanceLineDto
{
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal DebitBalance { get; set; }
    public decimal CreditBalance { get; set; }
}

public class AccountHistoryDto
{
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal BeginningBalance { get; set; }
    public List<AccountHistoryLineDto> Transactions { get; set; } = new();
    public decimal EndingBalance { get; set; }
}

public class AccountHistoryLineDto
{
    public DateTime Date { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal RunningBalance { get; set; }
}

public enum AccountType
{
    Asset,
    Liability,
    Equity,
    Revenue,
    Expense
}

public enum JournalEntrySource
{
    Manual,
    Invoice,
    Bill,
    Payment,
    VendorPayment,
    Payroll,
    BankReconciliation,
    Adjustment
}

public enum JournalEntryStatus
{
    Draft,
    Posted,
    Void
}
