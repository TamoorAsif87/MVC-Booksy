namespace Core.DTO;

public class OrderItemDto
{
    [Required]
    public Guid OrderId { get; set; }
    [Required]
    [MaxLength(150)]
    public string BookName { get; set; }

    [Required]
    public Guid BookId { get; set; }
    public decimal Price { get; set; }

    public int Quantity { get; set; }
    public string? BookCover { get; set; }

    public decimal ItemCost => Price * Quantity;
}
