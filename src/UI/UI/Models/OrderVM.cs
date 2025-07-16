namespace UI.Models;

public class OrderVM
{
    public IEnumerable<Cart>? Items { get; set; }
    public OrderDto? Order { get; set; }
}
