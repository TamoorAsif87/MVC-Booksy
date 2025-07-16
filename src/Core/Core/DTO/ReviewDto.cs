namespace Core.DTO;

public class ReviewDto
{
    public Guid Id { get; set; }
    public  string ApplicationUserId { get; set; }
    [EmailAddress]
    [Required]
    public  string Email { get; set; }
    [Required]
    [MaxLength(500)]
    public string Comment { get; set; }
    [Required]
    [Range(1,5,ErrorMessage = "Rating value must be between 1 to 5")]
    public int Rating { get; set; }
    public string? UserImage { get; set; }
    public DateTime ReviewTime { get; set; }
    [Required]
    public Guid BookId { get; set; }
    public BookDto? Book { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
