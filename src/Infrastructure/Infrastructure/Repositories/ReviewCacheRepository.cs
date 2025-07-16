
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Repositories;

public class ReviewCacheRepository : IReviewRepository
{
    private readonly IReviewRepository _inner;
    private readonly IDistributedCache _cache;

    const string AllReviewsCacheKey = "reviews:all";
    const string AllUserReviewsCacheKey = "user:reviews:all";
    const string AllBookReviewsCacheKey = "books:reviews:all";

    public ReviewCacheRepository(IReviewRepository inner, IDistributedCache cache)
    {
        _inner = inner;
        _cache = cache;
    }



    public async Task AddAsync(Review entity)
    {
        await _inner.AddAsync(entity);
        await InvalidateReviewCache(entity.BookId, entity.ApplicationUserId);
    }

    public async Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>>? predicate, string? navigationProperties)
    {
        if (predicate == null)
        {
            var cachedData = await _cache.GetStringAsync(AllReviewsCacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<IEnumerable<Review>>(cachedData) ?? Enumerable.Empty<Review>();
            }

            var reviews = await _inner.GetAllAsync(null, navigationProperties);
            var serialized = JsonSerializer.Serialize(reviews);
            await _cache.SetStringAsync(AllReviewsCacheKey, serialized);

            return reviews;
        }

        
        return await _inner.GetAllAsync(predicate, navigationProperties);
    }

    public async Task<IEnumerable<Review>> GetAllReviewsRelatedToBook(Guid bookId)
    {

        var cachedData = await _cache.GetStringAsync($"{AllBookReviewsCacheKey}:{bookId.ToString()}");
        if (!string.IsNullOrEmpty(cachedData))
            return JsonSerializer.Deserialize<IEnumerable<Review>>(cachedData) ?? Enumerable.Empty<Review>();

        var reviews =  await _inner.GetAllReviewsRelatedToBook(bookId);
        await _cache.SetStringAsync($"{AllBookReviewsCacheKey}:{bookId.ToString()}", JsonSerializer.Serialize(reviews));

        return reviews;
       
    }

    public async Task<IEnumerable<Review>> GetAllReviewsRelatedToUser(string userId)
    {
        var cachedData = await _cache.GetStringAsync($"{AllBookReviewsCacheKey}:{userId}");
        if (!string.IsNullOrEmpty(cachedData))
            return JsonSerializer.Deserialize<IEnumerable<Review>>(cachedData) ?? Enumerable.Empty<Review>();

        var reviews = await _inner.GetAllReviewsRelatedToUser(userId);
        await _cache.SetStringAsync($"{AllBookReviewsCacheKey}:{userId}", JsonSerializer.Serialize(reviews));

        return reviews;
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _inner.GetByIdAsync(id);
    }

    public async Task Remove(Review entity)
    {
        await _inner.Remove(entity);
        await InvalidateReviewCache(entity.BookId, entity.ApplicationUserId);
    }

    public void RemoveRange(IEnumerable<Review> entities)
    {
        _inner.RemoveRange(entities);
        foreach (var entity in entities)
        {
            Task.Run(() => InvalidateReviewCache(entity.BookId, entity.ApplicationUserId)).ConfigureAwait(false);
        }
    }

    public async Task SaveChanges()
    {
        await _inner.SaveChanges();
    }

    public async Task Update(Review entity)
    {
        await _inner.Update(entity);
        await InvalidateReviewCache(entity.BookId, entity.ApplicationUserId);
    }

    public async Task AddRangeAsync(IEnumerable<Review> entities)
    {
        await _inner.AddRangeAsync(entities);
        foreach (var entity in entities)
        {
            await InvalidateReviewCache(entity.BookId,entity.ApplicationUserId);
        }
      
    }

    public async Task<bool> AnyAsync(Expression<Func<Review, bool>> predicate)
    {
        return await _inner.AnyAsync(predicate);
    }

    public async Task<int> CountAsync()
    {
        return await _inner.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<Review, bool>> predicate)
    {
        return await _inner.CountAsync(predicate);
    }

    public async Task<Review?> FindOneAsync(Expression<Func<Review, bool>> predicate, string? navigationProperties)
    {
        return await _inner.FindOneAsync(predicate, navigationProperties);
    }

    private async Task InvalidateReviewCache(Guid bookId, string userId)
    {
        await _cache.RemoveAsync($"{AllBookReviewsCacheKey}:{bookId}");
        await _cache.RemoveAsync($"{AllUserReviewsCacheKey}:{userId}");
        await _cache.RemoveAsync(AllReviewsCacheKey);
    }
}
