namespace Core.ServiceContracts;

public interface IOrderService
{
    Task CreateOrderAsync(OrderDto orderDto);
    Task<OrderDto> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task UpdateOrderAsync(Guid orderId, OrderDto orderDto);
    Task DeleteOrderAsync(Guid orderId);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(string customerId);
    Task<OrderDto> GetOrderDetailsOfCustomer(string customerId, Guid orderId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);

    Task MarkAsShipped(Guid orderId);
    Task MarkAsDelivered(Guid orderId);
    Task<string> CheckOut();
    Task CancelOrder (Guid orderId);

    Task<Guid> PaymentSuccessful();
    
}
