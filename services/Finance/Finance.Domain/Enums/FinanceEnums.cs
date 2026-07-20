namespace Finance.Domain.Enums;

public enum InvoiceStatus
{
    Draft,
    Sent,
    PartiallyPaid,
    Paid,
    Overdue,
    Cancelled,
    Refunded
}

public enum PaymentMethod
{
    Unknown,
    CreditCard,
    BankTransfer,
    Cash,
    Check,
    PayPal,
    Other
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public enum Currency
{
    USD,
    EUR,
    GBP,
    CAD,
    AUD
}
