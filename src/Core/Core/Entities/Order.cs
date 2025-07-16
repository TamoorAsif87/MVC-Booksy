using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Order:BaseEntity
{
    
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public int PostCode { get; set; }
    public ICollection<OrderItem> Items { get; set; } = default!;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public ApplicationUser? User { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public bool Paid { get; set; } = false;
    public decimal TotalPrice { get; set; }

}


