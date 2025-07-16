namespace Core.DTOs;
public class BookDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Description { get; set; } = null!;

    public decimal Price { get; set; }
    public decimal Discount { get; set; }

    public List<string> ImageUrls { get; set; } = new();

    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public int Available { get; set; }

    public float AverageRating { get; set; }
    public int TotalReviews { get; set; }

    public DateOnly PublishedDate { get; set; }
    public string? ISBN { get; set; }
    public string? BookCover { get; set; }
    public ICollection<ReviewDto>? Reviews { get; set; }
}
