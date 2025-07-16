namespace Core.Mappers;

public class OrderProfile: Profile
{
    public OrderProfile()
    {
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Order, OrderDto>().ReverseMap();
    }
}

