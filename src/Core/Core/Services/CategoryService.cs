using Core.Specifications;

namespace Infrastructure.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CategorySpecs? categorySpecs)
    {
        var categories = await _categoryRepository.GetAllAsync(predicate:null,navigationProperties: "CategoryTranslations");
        var specs = categorySpecs ?? new CategorySpecs();

        if(specs == null) return _mapper.Map<IEnumerable<CategoryDto>>(categories);

        var filteredCategories = Filters<Category>.GetValues(categories, new CategorySpecification(specs));

        return _mapper.Map<IEnumerable<CategoryDto>>(filteredCategories);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category is null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task AddAsync(CategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChanges();
    }

    public async Task UpdateAsync(Guid id, CategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null || dto.Id != id)
            throw new KeyNotFoundException("Category not found");

        category.Name = dto.Name;

        await _categoryRepository.Update(category);
        await _categoryRepository.SaveChanges();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException("Category not found");

        await  _categoryRepository.Remove(category);
        await _categoryRepository.SaveChanges();
    }
}
