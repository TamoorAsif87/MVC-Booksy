namespace Core.Mappers;

class CategoryProfile:Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CategoryTranslation,CategoryTranslationDto>().ReverseMap();
    }
}
