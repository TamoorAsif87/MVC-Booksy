using Microsoft.AspNetCore.Mvc;

namespace Core.DTO;

public class CategoryDto
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    [Remote(
        action: "IsNameUnique",
        controller: "Category",
        areaName: "admin",
        AdditionalFields = nameof(Id),
        ErrorMessage = "Category name already exists."
    )]
    public string Name { get; set; }

    public ICollection<CategoryTranslationDto>? CategoryTranslations { get; set; }
}
