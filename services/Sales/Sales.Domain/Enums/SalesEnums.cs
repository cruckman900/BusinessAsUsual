namespace Sales.Domain.Enums;

public enum OrderStatus
{
    Draft = 0,
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Refunded = 7,
    OnHold = 8
}

public enum QuoteStatus
{
    Draft = 0,
    Sent = 1,
    Viewed = 2,
    Accepted = 3,
    Rejected = 4,
    Expired = 5,
    Converted = 6
}

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    BankTransfer = 2,
    Cash = 3,
    Check = 4,
    PayPal = 5,
    Stripe = 6,
    Other = 7
}

public enum ShippingMethod
{
    Standard = 0,
    Express = 1,
    Overnight = 2,
    International = 3,
    PickUp = 4,
    Freight = 5
}

public enum DiscountType
{
    Percentage = 0,
    FixedAmount = 1,
    BuyXGetY = 2
}

public enum Currency
{
    USD = 0,
    EUR = 1,
    GBP = 2,
    CAD = 3,
    AUD = 4,
    JPY = 5
}
