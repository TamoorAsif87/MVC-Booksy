namespace Core.Entities;

public class Book : BaseEntity
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required decimal Discount { get; set; }

    public List<string> ImageUrls { get; set; } = new();

    public required Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    public int Available { get; set; }

    public float AverageRating { get; set; }
    public int TotalReviews { get; set; }

    public DateOnly PublishedDate { get; set; }
    public string? ISBN { get; set; }
    public string? BookCover { get; set; }

    public ICollection<Review>? Reviews { get; set; }
}
