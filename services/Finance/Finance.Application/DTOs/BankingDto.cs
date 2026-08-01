namespace Finance.Application.DTOs;

public class BankAccountDto
{
    public string Id { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastReconciledDate { get; set; }
}

public class BankTransactionDto
{
    public string Id { get; set; } = string.Empty;
    public string BankAccountId { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal RunningBalance { get; set; }
    public string? Category { get; set; }
    public bool IsReconciled { get; set; }
    public string? MatchedToInvoiceId { get; set; }
    public string? MatchedToBillId { get; set; }
    public string? MatchedToPaymentId { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateBankAccountRequest
{
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Checking";
    public string Currency { get; set; } = "USD";
    public decimal OpeningBalance { get; set; }
}

public class CreateBankTransactionRequest
{
    public string BankAccountId { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class ReconcileTransactionRequest
{
    public string TransactionId { get; set; } = string.Empty;
    public string? MatchedToInvoiceId { get; set; }
    public string? MatchedToBillId { get; set; }
    public string? MatchedToPaymentId { get; set; }
}
