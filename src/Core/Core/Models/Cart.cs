namespace Core.Models;

public class Cart
{
    public Guid BookId { get; set; }
    public string? BookCover { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
    public string? Author { get; set; }
    public string? CategoryName { get; set; }
    public decimal Price { get; set; }

    public decimal Total => Quantity * Price;
}
