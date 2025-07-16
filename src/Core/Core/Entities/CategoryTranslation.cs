using System.Text.Json.Serialization;

namespace Core.Entities;

public class CategoryTranslation
{

    public Guid CategoryId { get; set; }
    [JsonIgnore]
    public Category? Category { get; set; }
    public required string Culture { get; set; }
    public required string Name { get; set; }
}
