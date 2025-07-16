namespace Core.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string BookName { get; set; }
    public Guid BookId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? BookCover { get; set; }

}
