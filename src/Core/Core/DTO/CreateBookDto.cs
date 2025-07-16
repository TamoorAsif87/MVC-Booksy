namespace Core.DTOs;
using System.ComponentModel.DataAnnotations;

public class CreateBookDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = null!;

    [Required]
    [Range(0, 999999.99)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal Discount { get; set; }

    public List<string> ImageUrls { get; set; } = new();

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Available { get; set; }

    [Required]
    public DateOnly PublishedDate { get; set; }

    [MaxLength(20)]
    public string? ISBN { get; set; }
    public string? BookCover { get; set; }

}