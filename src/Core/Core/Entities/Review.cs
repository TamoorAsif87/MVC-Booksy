namespace Core.Entities;

public class Review:BaseEntity
{
    public required string ApplicationUserId { get; set; }
    public ApplicationUser? User { get; set; }
    public required string Email { get; set; }
    public int Rating { get; set; }
    public required string Comment { get; set; }

    public string? UserImage { get; set; }
    public DateTime ReviewTime { get; set; }
    public Guid BookId { get; set; }
    public Book? Book { get; set; }
}
