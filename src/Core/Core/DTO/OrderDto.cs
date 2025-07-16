
namespace Core.DTO;

public class OrderDto
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MaxLength(200)]
    public string Address { get; set; }
    [Phone]
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; }
    [Required]
    [MaxLength(50)]
    public  string City { get; set; }
    [Required]
    [MaxLength(50)]
    public  string Country { get; set; }
    public int PostCode { get; set; }
    public ICollection<OrderItemDto> Items { get; set; } = default!;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string? CustomerId { get; set; }
    public ApplicationUser? User { get; set; }
    public bool Paid { get; set; } = false;

    public decimal TotalSum() => Items.Count > 0 ? Items.Sum(item => item.ItemCost): 0;
}
