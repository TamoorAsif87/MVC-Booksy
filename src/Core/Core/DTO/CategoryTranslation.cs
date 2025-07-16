namespace Core.DTO;

public class CategoryTranslationDto
{

    public Guid CategoryId { get; set; }
    public required string Culture { get; set; }
    public required string Name { get; set; }
}
