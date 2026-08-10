using Sales.Application.DTOs;

namespace Sales.Application.Services;

/// <summary>
/// Mock implementation of IOrderService for shell environments where the Sales API is unavailable.
/// Returns empty/null data to prevent hard failures in the UI.
/// </summary>
public class MockOrderService : IOrderService
{
    public Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        return Task.FromResult(Enumerable.Empty<OrderDto>());
    }

    public Task<OrderDto?> GetOrderByIdAsync(string id)
    {
        return Task.FromResult<OrderDto?>(null);
    }

    public Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        // Return a minimal order DTO to prevent null reference exceptions
        return Task.FromResult(new OrderDto
        {
            Id = Guid.NewGuid().ToString(),
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            CustomerId = dto.CustomerId,
            CustomerName = "Mock Customer",
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

    public Task<OrderDto> UpdateOrderAsync(UpdateOrderDto dto)
    {
        // Return a minimal order DTO to prevent null reference exceptions
        return Task.FromResult(new OrderDto
        {
            Id = dto.Id,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            CustomerId = dto.CustomerId ?? string.Empty,
            CustomerName = "Mock Customer",
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

    public Task<bool> DeleteOrderAsync(string id)
    {
        return Task.FromResult(true);
    }

    public Task<OrderDto> ConfirmOrderAsync(string id)
    {
        return Task.FromResult(new OrderDto
        {
            Id = id,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.OrderStatus.Confirmed,
            OrderDate = DateTime.UtcNow,
            ConfirmedDate = DateTime.UtcNow
        });
    }

    public Task<OrderDto> ShipOrderAsync(string id, string trackingNumber, DateTime? shippedDate = null)
    {
        return Task.FromResult(new OrderDto
        {
            Id = id,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.OrderStatus.Shipped,
            OrderDate = DateTime.UtcNow,
            ShippedDate = shippedDate ?? DateTime.UtcNow,
            TrackingNumber = trackingNumber
        });
    }

    public Task<OrderDto> DeliverOrderAsync(string id, DateTime? deliveredDate = null)
    {
        return Task.FromResult(new OrderDto
        {
            Id = id,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.OrderStatus.Delivered,
            OrderDate = DateTime.UtcNow,
            DeliveredDate = deliveredDate ?? DateTime.UtcNow
        });
    }

    public Task<OrderDto> CancelOrderAsync(string id)
    {
        return Task.FromResult(new OrderDto
        {
            Id = id,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001",
            Status = Domain.Enums.OrderStatus.Cancelled,
            OrderDate = DateTime.UtcNow,
            CancelledDate = DateTime.UtcNow
        });
    }

    public Task<OrderPaymentDto> AddPaymentAsync(AddOrderPaymentDto dto)
    {
        return Task.FromResult(new OrderPaymentDto
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = dto.OrderId,
            PaymentMethod = dto.PaymentMethod,
            Amount = dto.Amount,
            PaymentDate = DateTime.UtcNow,
            TransactionId = dto.TransactionId,
            ReferenceNumber = dto.ReferenceNumber,
            Notes = dto.Notes
        });
    }
}
