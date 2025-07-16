using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Contracts;
using StackExchange.Redis;

namespace Shared.Services.Redis;

public class Recommender : IRecommender
{
    private readonly IDatabase _db;

    public Recommender(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public string GetBookKey(Guid bookId)
    {
        return $"book:{bookId} bought-with";
    }

    public async Task BoughtTogether(List<Guid> bookIds)
    {
        var ids = bookIds?.Distinct().ToArray();
        if (ids == null || ids.Length < 2) return;

        var pending = new List<Task>();

        foreach (var id in ids!)
        {
            var key = GetBookKey(id);

            foreach (var with in ids)
            {
                if (with == id) continue;

                pending.Add(_db.SortedSetIncrementAsync(
                    key, with.ToString(), 1, CommandFlags.FireAndForget));
            }
        }

        await _db.PingAsync();
    }

    public List<Guid> SuggestedBooksFor(List<Guid> bookIds, int take = 5, int minScore = 1)
    {
        if(bookIds == null || bookIds.Count == 0)
            return new List<Guid>();

        RedisKey[] keys = [.. bookIds.Select(x => (RedisKey)GetBookKey(x))];

        if(keys.Length == 1)
        {
            var results = _db.SortedSetRangeByScoreWithScores(keys[0],order:Order.Descending, take:take);
            
            return [.. results.Where(x => x.Score >= minScore)
                          .Select(x => Guid.TryParseExact(x.Element,"D", out var g) ? g:Guid.Empty)
                          .Where(g => g!= Guid.Empty && !bookIds.Contains(g))];
        }

        var tempKey = $"tem:suggested:{Guid.NewGuid()}:N";

        _db.SortedSetCombineAndStore(SetOperation.Union, tempKey, keys, aggregate: Aggregate.Sum);

        try
        {
               foreach (var id in bookIds)
               {
                _db.SortedSetRemove(tempKey, id.ToString("D"));
               } 

               var results = _db.SortedSetRangeByScoreWithScores(tempKey,order:Order.Descending, take:take);

            return [.. results.Where(x => x.Score > minScore)
                          .Select(x => Guid.TryParseExact(x.Element,"D", out var g) ? g:Guid.Empty)
                          .Where(g => g!= Guid.Empty && !bookIds.Contains(g))];
        }
        
        finally
        {

            _db.KeyDelete(tempKey,CommandFlags.FireAndForget);
        }
    }
}
