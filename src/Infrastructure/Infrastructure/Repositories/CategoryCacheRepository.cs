using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace Infrastructure.Repositories;

public class CategoryCacheRepository : ICategoryRepository
{
    private readonly ICategoryRepository _inner;
    private readonly IDistributedCache _cache;

    public CategoryCacheRepository(ICategoryRepository inner, IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(Expression<Func<Category, bool>>? predicate, string? navigationProperties)
    {
        const string cacheKey = "categories:all";
       
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<IEnumerable<Category>>(cached)!;

        var categories = await _inner.GetAllAsync(predicate, navigationProperties);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(categories));
        return categories;
        

    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"categories:{id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<Category>(cached)!;

        var category = await _inner.GetByIdAsync(id);
        if (category != null)
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(category));

        return category;
    }

    public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate, string? navigationProperties)
    {
        return await _inner.GetAllAsync(predicate, navigationProperties);
    }

    public async Task AddAsync(Category entity)
    {
        await _inner.AddAsync(entity);
        await InvalidateCategoryCache(entity.Id);
    }

    public async Task AddRangeAsync(IEnumerable<Category> entities)
    {
        await _inner.AddRangeAsync(entities);
        foreach (var category in entities)
            await InvalidateCategoryCache(category.Id);
    }

    public async Task Update(Category entity)
    {
        await _inner.Update(entity);
        InvalidateCategoryCache(entity.Id).GetAwaiter().GetResult();
    }

    public async Task Remove(Category entity)
    {
        await _inner.Remove(entity);
        InvalidateCategoryCache(entity.Id).GetAwaiter().GetResult();
    }

    public void RemoveRange(IEnumerable<Category> entities)
    {
        _inner.RemoveRange(entities);
        foreach (var category in entities)
            _cache.RemoveAsync($"categories:{category.Id}").GetAwaiter().GetResult();

        _cache.RemoveAsync("categories:all");
    }

    public Task<bool> AnyAsync(Expression<Func<Category, bool>> predicate)
    {
        return _inner.AnyAsync(predicate);
    }

    public Task<int> CountAsync()
    {
        return _inner.CountAsync();
    }

    public Task<int> CountAsync(Expression<Func<Category, bool>> predicate)
    {
        return _inner.CountAsync(predicate);
    }

    public Task SaveChanges()
    {
        return _inner.SaveChanges();
    }

    private async Task InvalidateCategoryCache(Guid id)
    {
        await _cache.RemoveAsync($"categories:{id}");
        await _cache.RemoveAsync("categories:all");
    }

    public async Task<Category?> FindOneAsync(Expression<Func<Category, bool>> predicate, string? navigationProperties)
    {
        var category = await _inner.FindOneAsync(predicate, navigationProperties);
        return category;
    }
}
