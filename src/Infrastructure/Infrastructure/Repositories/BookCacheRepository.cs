using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Repositories;

public class BookCacheRepository:IBookRepository
{
    private readonly IBookRepository _inner;
    private readonly IDistributedCache _cache;

    public BookCacheRepository(IBookRepository inner, IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(Expression<Func<Book, bool>>? predicate, string? navigationProperties)
    {
        
            const string cacheKey = "books:all";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<IEnumerable<Book>>(cached)!;

            var books = await _inner.GetAllAsync(predicate, navigationProperties);
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(books));
            return books;
        

      
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {

        var book =  await _inner.GetByIdAsync(id);
        await _cache.RemoveAsync("books:all");
        return book;

    }

    

    public async Task<IEnumerable<Book>> FindAsync(Expression<Func<Book, bool>> predicate, string? navigationProperties)
    {
        return await _inner.GetAllAsync(predicate, navigationProperties);
    }

    public async Task AddAsync(Book entity)
    {
        await _inner.AddAsync(entity);
        await InvalidateBookCache(entity.Id);
    }

    public async Task AddRangeAsync(IEnumerable<Book> entities)
    {
        await _inner.AddRangeAsync(entities);
        foreach (var book in entities)
            await InvalidateBookCache(book.Id);
    }

    public async Task Update(Book entity)
    {
        //await _inner.Update(entity);
        await InvalidateBookCache(entity.Id);
    }

    public async Task Remove(Book entity)
    {
        await _inner.Remove(entity);
        InvalidateBookCache(entity.Id).GetAwaiter().GetResult();
    }

    public void RemoveRange(IEnumerable<Book> entities)
    {
        _inner.RemoveRange(entities);
        foreach (var book in entities)
        {
            _cache.RemoveAsync($"books:{book.Id}").GetAwaiter().GetResult();
        }
        _cache.RemoveAsync("books:all");
    }

    public Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate)
    {
        return _inner.AnyAsync(predicate); 
    }

    public Task<int> CountAsync()
    {
        return _inner.CountAsync();
    }

    public Task<int> CountAsync(Expression<Func<Book, bool>> predicate)
    {
        return _inner.CountAsync(predicate); 
    }

    public Task SaveChanges()
    {
        return _inner.SaveChanges();
    }

    private async Task InvalidateBookCache(Guid id)
    {
        await _cache.RemoveAsync($"books:{id}");
        await _cache.RemoveAsync("books:all");
    }

    public async Task<Book?> FindOneAsync(Expression<Func<Book, bool>> predicate, string? navigationProperties)
    {
        var book = await _inner.FindOneAsync(predicate, navigationProperties);
        return book;
    }
}
