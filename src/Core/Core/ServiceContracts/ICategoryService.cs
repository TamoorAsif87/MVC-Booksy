namespace Core.ServicesContracts;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(CategorySpecs? categorySpecs);
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task AddAsync(CategoryDto dto);
    Task UpdateAsync(Guid id, CategoryDto dto);
    Task DeleteAsync(Guid id);
}
