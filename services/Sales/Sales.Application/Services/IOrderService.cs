using Sales.Application.DTOs;

namespace Sales.Application.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(string id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto> UpdateOrderAsync(UpdateOrderDto dto);
    Task<bool> DeleteOrderAsync(string id);
    Task<OrderDto> ConfirmOrderAsync(string id);
    Task<OrderDto> ShipOrderAsync(string id, string trackingNumber, DateTime? shippedDate = null);
    Task<OrderDto> DeliverOrderAsync(string id, DateTime? deliveredDate = null);
    Task<OrderDto> CancelOrderAsync(string id);
    Task<OrderPaymentDto> AddPaymentAsync(AddOrderPaymentDto dto);
}
