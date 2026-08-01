using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Enums;

namespace Sales.Application.Services;

public class QuoteService : IQuoteService
{
    private readonly List<Quote> _quotes = new(); // TODO: Replace with repository

    public async Task<IEnumerable<QuoteDto>> GetAllQuotesAsync()
    {
        await Task.CompletedTask;
        return _quotes.Select(MapToDto);
    }

    public async Task<QuoteDto?> GetQuoteByIdAsync(string id)
    {
        await Task.CompletedTask;
        var quote = _quotes.FirstOrDefault(q => q.Id == id);
        return quote == null ? null : MapToDto(quote);
    }

    public async Task<QuoteDto> CreateQuoteAsync(CreateQuoteDto dto)
    {
        await Task.CompletedTask;

        var quote = new Quote
        {
            QuoteNumber = $"Q-{DateTime.UtcNow:yyyyMMdd}-{_quotes.Count + 1:D4}",
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            CustomerPhone = dto.CustomerPhone,
            Currency = dto.Currency,
            ExpiryDate = dto.ExpiryDate,
            Notes = dto.Notes,
            Terms = dto.Terms,
            AssignedToEmployeeId = dto.AssignedToEmployeeId,
            LineItems = dto.LineItems.Select((li, index) => new QuoteLineItem
            {
                QuoteId = string.Empty, // Will be set after quote creation
                ProductId = li.ProductId,
                ProductName = li.ProductName,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                DiscountPercentage = li.DiscountPercentage,
                TaxPercentage = li.TaxPercentage,
                SortOrder = index
            }).ToList()
        };

        // Set QuoteId for all line items
        foreach (var item in quote.LineItems)
        {
            item.QuoteId = quote.Id;
        }

        _quotes.Add(quote);
        return MapToDto(quote);
    }

    public async Task<QuoteDto> UpdateQuoteAsync(UpdateQuoteDto dto)
    {
        await Task.CompletedTask;

        var quote = _quotes.FirstOrDefault(q => q.Id == dto.Id);
        if (quote == null)
            throw new KeyNotFoundException($"Quote with ID {dto.Id} not found");

        quote.CustomerId = dto.CustomerId;
        quote.CustomerName = dto.CustomerName;
        quote.CustomerEmail = dto.CustomerEmail;
        quote.CustomerPhone = dto.CustomerPhone;
        quote.Status = dto.Status;
        quote.Currency = dto.Currency;
        quote.ExpiryDate = dto.ExpiryDate;
        quote.Notes = dto.Notes;
        quote.Terms = dto.Terms;
        quote.AssignedToEmployeeId = dto.AssignedToEmployeeId;
        quote.LastModifiedDate = DateTime.UtcNow;

        // Update line items
        quote.LineItems = dto.LineItems.Select((li, index) => new QuoteLineItem
        {
            QuoteId = quote.Id,
            ProductId = li.ProductId,
            ProductName = li.ProductName,
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            DiscountPercentage = li.DiscountPercentage,
            TaxPercentage = li.TaxPercentage,
            SortOrder = index
        }).ToList();

        return MapToDto(quote);
    }

    public async Task<bool> DeleteQuoteAsync(string id)
    {
        await Task.CompletedTask;
        var quote = _quotes.FirstOrDefault(q => q.Id == id);
        if (quote == null) return false;

        _quotes.Remove(quote);
        return true;
    }

    public async Task<QuoteDto> SendQuoteAsync(string id)
    {
        await Task.CompletedTask;

        var quote = _quotes.FirstOrDefault(q => q.Id == id);
        if (quote == null)
            throw new KeyNotFoundException($"Quote with ID {id} not found");

        quote.Status = QuoteStatus.Sent;
        quote.SentDate = DateTime.UtcNow;
        quote.LastModifiedDate = DateTime.UtcNow;

        return MapToDto(quote);
    }

    public async Task<QuoteDto> AcceptQuoteAsync(string id)
    {
        await Task.CompletedTask;

        var quote = _quotes.FirstOrDefault(q => q.Id == id);
        if (quote == null)
            throw new KeyNotFoundException($"Quote with ID {id} not found");

        quote.Status = QuoteStatus.Accepted;
        quote.AcceptedDate = DateTime.UtcNow;
        quote.LastModifiedDate = DateTime.UtcNow;

        return MapToDto(quote);
    }

    public async Task<QuoteDto> RejectQuoteAsync(string id)
    {
        await Task.CompletedTask;

        var quote = _quotes.FirstOrDefault(q => q.Id == id);
        if (quote == null)
            throw new KeyNotFoundException($"Quote with ID {id} not found");

        quote.Status = QuoteStatus.Rejected;
        quote.LastModifiedDate = DateTime.UtcNow;

        return MapToDto(quote);
    }

    public async Task<OrderDto> ConvertQuoteToOrderAsync(string quoteId)
    {
        await Task.CompletedTask;

        var quote = _quotes.FirstOrDefault(q => q.Id == quoteId);
        if (quote == null)
            throw new KeyNotFoundException($"Quote with ID {quoteId} not found");

        if (quote.Status != QuoteStatus.Accepted)
            throw new InvalidOperationException("Only accepted quotes can be converted to orders");

        var order = new Order
        {
            OrderNumber = $"O-{DateTime.UtcNow:yyyyMMdd}-{1:D4}", // TODO: Proper numbering
            CustomerId = quote.CustomerId,
            CustomerName = quote.CustomerName,
            CustomerEmail = quote.CustomerEmail,
            CustomerPhone = quote.CustomerPhone,
            Currency = quote.Currency,
            SourceModule = "Sales",
            SourceReferenceId = quote.Id,
            Status = OrderStatus.Pending,
            LineItems = quote.LineItems.Select((li, index) => new OrderLineItem
            {
                OrderId = string.Empty,
                ProductId = li.ProductId,
                ProductName = li.ProductName,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                DiscountPercentage = li.DiscountPercentage,
                TaxPercentage = li.TaxPercentage,
                SortOrder = index
            }).ToList()
        };

        foreach (var item in order.LineItems)
        {
            item.OrderId = order.Id;
        }

        quote.Status = QuoteStatus.Converted;
        quote.ConvertedDate = DateTime.UtcNow;
        quote.ConvertedToOrderId = order.Id;
        quote.LastModifiedDate = DateTime.UtcNow;

        // TODO: Add order to order service/repository
        return MapOrderToDto(order);
    }

    private static QuoteDto MapToDto(Quote quote)
    {
        return new QuoteDto
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            CustomerId = quote.CustomerId,
            CustomerName = quote.CustomerName,
            CustomerEmail = quote.CustomerEmail,
            CustomerPhone = quote.CustomerPhone,
            Status = quote.Status,
            Currency = quote.Currency,
            CreatedDate = quote.CreatedDate,
            SentDate = quote.SentDate,
            ExpiryDate = quote.ExpiryDate,
            AcceptedDate = quote.AcceptedDate,
            ConvertedDate = quote.ConvertedDate,
            ConvertedToOrderId = quote.ConvertedToOrderId,
            Notes = quote.Notes,
            Terms = quote.Terms,
            AssignedToEmployeeId = quote.AssignedToEmployeeId,
            LineItems = quote.LineItems.Select(li => new QuoteLineItemDto
            {
                Id = li.Id,
                QuoteId = li.QuoteId,
                ProductId = li.ProductId,
                ProductName = li.ProductName,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                DiscountPercentage = li.DiscountPercentage,
                TaxPercentage = li.TaxPercentage,
                Subtotal = li.Subtotal,
                DiscountAmount = li.DiscountAmount,
                TaxAmount = li.TaxAmount,
                LineTotal = li.LineTotal,
                SortOrder = li.SortOrder
            }).ToList(),
            Subtotal = quote.Subtotal,
            TotalDiscount = quote.TotalDiscount,
            TotalTax = quote.TotalTax,
            Total = quote.Total,
            IsExpired = quote.IsExpired
        };
    }

    private static OrderDto MapOrderToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            CustomerPhone = order.CustomerPhone,
            Status = order.Status,
            Currency = order.Currency,
            OrderDate = order.OrderDate,
            ConfirmedDate = order.ConfirmedDate,
            ShippedDate = order.ShippedDate,
            DeliveredDate = order.DeliveredDate,
            CancelledDate = order.CancelledDate,
            ShippingMethod = order.ShippingMethod,
            ShippingAddressLine1 = order.ShippingAddressLine1,
            ShippingAddressLine2 = order.ShippingAddressLine2,
            ShippingCity = order.ShippingCity,
            ShippingState = order.ShippingState,
            ShippingPostalCode = order.ShippingPostalCode,
            ShippingCountry = order.ShippingCountry,
            TrackingNumber = order.TrackingNumber,
            BillingAddressLine1 = order.BillingAddressLine1,
            BillingAddressLine2 = order.BillingAddressLine2,
            BillingCity = order.BillingCity,
            BillingState = order.BillingState,
            BillingPostalCode = order.BillingPostalCode,
            BillingCountry = order.BillingCountry,
            Notes = order.Notes,
            InternalNotes = order.InternalNotes,
            AssignedToEmployeeId = order.AssignedToEmployeeId,
            LineItems = order.LineItems.Select(li => new OrderLineItemDto
            {
                Id = li.Id,
                OrderId = li.OrderId,
                ProductId = li.ProductId,
                ProductName = li.ProductName,
                SKU = li.SKU,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                DiscountPercentage = li.DiscountPercentage,
                TaxPercentage = li.TaxPercentage,
                Subtotal = li.Subtotal,
                DiscountAmount = li.DiscountAmount,
                TaxAmount = li.TaxAmount,
                LineTotal = li.LineTotal,
                SortOrder = li.SortOrder,
                QuantityShipped = li.QuantityShipped,
                QuantityDelivered = li.QuantityDelivered,
                IsFulfilled = li.IsFulfilled
            }).ToList(),
            Payments = order.Payments.Select(p => new OrderPaymentDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                TransactionId = p.TransactionId,
                ReferenceNumber = p.ReferenceNumber,
                Notes = p.Notes,
                IsCompleted = p.IsCompleted
            }).ToList(),
            Subtotal = order.Subtotal,
            TotalDiscount = order.TotalDiscount,
            TotalTax = order.TotalTax,
            ShippingCost = order.ShippingCost,
            Total = order.Total,
            AmountPaid = order.AmountPaid,
            BalanceDue = order.BalanceDue,
            IsPaid = order.IsPaid
        };
    }
}
