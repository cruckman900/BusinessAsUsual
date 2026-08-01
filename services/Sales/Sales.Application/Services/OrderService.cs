using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Enums;

namespace Sales.Application.Services;

public class OrderService : IOrderService
{
    private readonly List<Order> _orders = new(); // TODO: Replace with repository

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        await Task.CompletedTask;
        return _orders.Select(MapToDto);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(string id)
    {
        await Task.CompletedTask;
        var order = _orders.FirstOrDefault(o => o.Id == id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        await Task.CompletedTask;

        var order = new Order
        {
            OrderNumber = $"O-{DateTime.UtcNow:yyyyMMdd}-{_orders.Count + 1:D4}",
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            CustomerPhone = dto.CustomerPhone,
            Currency = dto.Currency,
            ShippingMethod = dto.ShippingMethod,
            ShippingAddressLine1 = dto.ShippingAddressLine1,
            ShippingAddressLine2 = dto.ShippingAddressLine2,
            ShippingCity = dto.ShippingCity,
            ShippingState = dto.ShippingState,
            ShippingPostalCode = dto.ShippingPostalCode,
            ShippingCountry = dto.ShippingCountry,
            BillingAddressLine1 = dto.BillingAddressLine1,
            BillingAddressLine2 = dto.BillingAddressLine2,
            BillingCity = dto.BillingCity,
            BillingState = dto.BillingState,
            BillingPostalCode = dto.BillingPostalCode,
            BillingCountry = dto.BillingCountry,
            Notes = dto.Notes,
            InternalNotes = dto.InternalNotes,
            AssignedToEmployeeId = dto.AssignedToEmployeeId,
            ShippingCost = dto.ShippingCost,
            LineItems = dto.LineItems.Select((li, index) => new OrderLineItem
            {
                OrderId = string.Empty, // Will be set after order creation
                ProductId = li.ProductId,
                ProductName = li.ProductName,
                SKU = li.SKU,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                DiscountPercentage = li.DiscountPercentage,
                TaxPercentage = li.TaxPercentage,
                SortOrder = index
            }).ToList()
        };

        // Set OrderId for all line items
        foreach (var item in order.LineItems)
        {
            item.OrderId = order.Id;
        }

        _orders.Add(order);
        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateOrderAsync(UpdateOrderDto dto)
    {
        await Task.CompletedTask;

        var order = _orders.FirstOrDefault(o => o.Id == dto.Id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {dto.Id} not found");

        order.CustomerId = dto.CustomerId;
        order.CustomerName = dto.CustomerName;
        order.CustomerEmail = dto.CustomerEmail;
        order.CustomerPhone = dto.CustomerPhone;
        order.Status = dto.Status;
        order.Currency = dto.Currency;
        order.ShippingMethod = dto.ShippingMethod;
        order.ShippingAddressLine1 = dto.ShippingAddressLine1;
        order.ShippingAddressLine2 = dto.ShippingAddressLine2;
        order.ShippingCity = dto.ShippingCity;
        order.ShippingState = dto.ShippingState;
        order.ShippingPostalCode = dto.ShippingPostalCode;
        order.ShippingCountry = dto.ShippingCountry;
        order.BillingAddressLine1 = dto.BillingAddressLine1;
        order.BillingAddressLine2 = dto.BillingAddressLine2;
        order.BillingCity = dto.BillingCity;
        order.BillingState = dto.BillingState;
        order.BillingPostalCode = dto.BillingPostalCode;
        order.BillingCountry = dto.BillingCountry;
        order.Notes = dto.Notes;
        order.InternalNotes = dto.InternalNotes;
        order.AssignedToEmployeeId = dto.AssignedToEmployeeId;
        order.ShippingCost = dto.ShippingCost;
        order.LastModifiedDate = DateTime.UtcNow;

        // Update line items
        order.LineItems = dto.LineItems.Select((li, index) => new OrderLineItem
        {
            OrderId = order.Id,
            ProductId = li.ProductId,
            ProductName = li.ProductName,
            SKU = li.SKU,
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            DiscountPercentage = li.DiscountPercentage,
            TaxPercentage = li.TaxPercentage,
            SortOrder = index
        }).ToList();

        return MapToDto(order);
    }

    public async Task<bool> DeleteOrderAsync(string id)
    {
        await Task.CompletedTask;
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null) return false;

        _orders.Remove(order);
        return true;
    }

    public async Task<OrderDto> ConfirmOrderAsync(string id)
    {
        await Task.CompletedTask;

        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found");

        order.Status = OrderStatus.Confirmed;
        order.ConfirmedDate = DateTime.UtcNow;
        order.LastModifiedDate = DateTime.UtcNow;

        return MapToDto(order);
    }

    public async Task<OrderDto> ShipOrderAsync(string id, string trackingNumber, DateTime? shippedDate = null)
    {
        await Task.CompletedTask;

        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found");

        order.Status = OrderStatus.Shipped;
        order.ShippedDate = shippedDate ?? DateTime.UtcNow;
        order.TrackingNumber = trackingNumber;
        order.LastModifiedDate = DateTime.UtcNow;

        return MapToDto(order);
    }

    public async Task<OrderDto> DeliverOrderAsync(string id, DateTime? deliveredDate = null)
    {
        await Task.CompletedTask;

        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found");

        order.Status = OrderStatus.Delivered;
        order.DeliveredDate = deliveredDate ?? DateTime.UtcNow;
        order.LastModifiedDate = DateTime.UtcNow;

        return MapToDto(order);
    }

    public async Task<OrderDto> CancelOrderAsync(string id)
    {
        await Task.CompletedTask;

        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found");

        order.Status = OrderStatus.Cancelled;
        order.CancelledDate = DateTime.UtcNow;
        order.LastModifiedDate = DateTime.UtcNow;

        return MapToDto(order);
    }

    public async Task<OrderPaymentDto> AddPaymentAsync(AddOrderPaymentDto dto)
    {
        await Task.CompletedTask;

        var order = _orders.FirstOrDefault(o => o.Id == dto.OrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {dto.OrderId} not found");

        var payment = new OrderPayment
        {
            OrderId = dto.OrderId,
            PaymentMethod = dto.PaymentMethod,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            TransactionId = dto.TransactionId,
            ReferenceNumber = dto.ReferenceNumber,
            Notes = dto.Notes,
            IsCompleted = true
        };

        order.Payments.Add(payment);
        order.LastModifiedDate = DateTime.UtcNow;

        return new OrderPaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            PaymentMethod = payment.PaymentMethod,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            TransactionId = payment.TransactionId,
            ReferenceNumber = payment.ReferenceNumber,
            Notes = payment.Notes,
            IsCompleted = payment.IsCompleted
        };
    }

    private static OrderDto MapToDto(Order order)
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
