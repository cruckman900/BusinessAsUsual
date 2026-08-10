using Sales.Application.DTOs;

namespace Sales.Application.Services;

/// <summary>
/// Mock implementation of IQuoteService for shell environments where the Sales API is unavailable.
/// Returns empty/null data to prevent hard failures in the UI.
/// </summary>
public class MockQuoteService : IQuoteService
{
    public Task<IEnumerable<QuoteDto>> GetAllQuotesAsync()
    {
        return Task.FromResult(Enumerable.Empty<QuoteDto>());
    }

    public Task<QuoteDto?> GetQuoteByIdAsync(string id)
    {
        return Task.FromResult<QuoteDto?>(null);
    }

    public Task<QuoteDto> CreateQuoteAsync(CreateQuoteDto dto)
    {
        // Return a minimal quote DTO to prevent null reference exceptions
        return Task.FromResult(new QuoteDto
        {
            Id = Guid.NewGuid().ToString(),
            QuoteNumber = $"QTE-{DateTime.UtcNow:yyyyMMdd}-0001",
            CustomerId = dto.CustomerId,
            CustomerName = "Mock Customer",
            Status = Domain.Enums.QuoteStatus.Draft,
            Currency = Domain.Enums.Currency.USD,
            CreatedDate = DateTime.UtcNow,
            LineItems = new List<QuoteLineItemDto>(),
            Subtotal = 0,
            TotalDiscount = 0,
            TotalTax = 0,
            Total = 0,
            IsExpired = false
        });
    }

    public Task<QuoteDto> UpdateQuoteAsync(UpdateQuoteDto dto)
    {
        // Return a minimal quote DTO to prevent null reference exceptions
        return Task.FromResult(new QuoteDto
        {
            Id = dto.Id,
            QuoteNumber = $"QTE-{DateTime.UtcNow:yyyyMMdd}-0001",
            CustomerId = dto.CustomerId ?? string.Empty,
            CustomerName = "Mock Customer",
            Status = Domain.Enums.QuoteStatus.Draft,
            Currency = Domain.Enums.Currency.USD,
            CreatedDate = DateTime.UtcNow,
            LineItems = new List<QuoteLineItemDto>(),
            Subtotal = 0,
            TotalDiscount = 0,
            TotalTax = 0,
            Total = 0,
            IsExpired = false
        });
    }

    public Task<bool> DeleteQuoteAsync(string id)
    {
        return Task.FromResult(true);
    }

    public Task<QuoteDto> SendQuoteAsync(string id)
    {
        return Task.FromResult(new QuoteDto
        {
            Id = id,
            QuoteNumber = $"QTE-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.QuoteStatus.Sent,
            CreatedDate = DateTime.UtcNow,
            SentDate = DateTime.UtcNow
        });
    }

    public Task<QuoteDto> AcceptQuoteAsync(string id)
    {
        return Task.FromResult(new QuoteDto
        {
            Id = id,
            QuoteNumber = $"QTE-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.QuoteStatus.Accepted,
            CreatedDate = DateTime.UtcNow,
            AcceptedDate = DateTime.UtcNow
        });
    }

    public Task<QuoteDto> RejectQuoteAsync(string id)
    {
        return Task.FromResult(new QuoteDto
        {
            Id = id,
            QuoteNumber = $"QTE-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.QuoteStatus.Rejected,
            CreatedDate = DateTime.UtcNow
        });
    }

    public Task<OrderDto> ConvertQuoteToOrderAsync(string quoteId)
    {
        return Task.FromResult(new OrderDto
        {
            Id = Guid.NewGuid().ToString(),
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.OrderStatus.Draft,
            Currency = Domain.Enums.Currency.USD,
            OrderDate = DateTime.UtcNow,
            LineItems = new List<OrderLineItemDto>(),
            Payments = new List<OrderPaymentDto>(),
            Subtotal = 0,
            TotalDiscount = 0,
            TotalTax = 0,
            ShippingCost = 0,
            Total = 0,
            AmountPaid = 0,
            BalanceDue = 0,
            IsPaid = false
        });
    }
}
