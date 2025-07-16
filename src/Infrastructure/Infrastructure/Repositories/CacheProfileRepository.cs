using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Repositories;

public class CacheProfileRepository(IProfileRepository _profileRepository, IDistributedCache _cache) : IProfileRepository
{
   

    public async Task<UserProfile?> GetByIdAsync(string userId)
    {
        var redisValue = await _cache.GetStringAsync($"{userId.ToLowerInvariant()}-Profile");
        if (string.IsNullOrEmpty(redisValue))
        {
            var profile = await _profileRepository.GetByIdAsync(userId);
            await _cache.SetStringAsync($"{userId}-Profile", JsonSerializer.Serialize(profile));
            return profile;
        }

        return JsonSerializer.Deserialize<UserProfile>(redisValue!);
    }


    public async Task AddAsync(UserProfile profile)
    {
        await _profileRepository.AddAsync(profile);
        await _cache.SetStringAsync($"{profile.ApplicationUserId.ToLowerInvariant()}-Profile", JsonSerializer.Serialize(profile));
        await _cache.RemoveAsync("all:Profiles");
    }

    public async Task Update(UserProfile profile)
    {
        await _profileRepository.Update(profile);
        await _cache.SetStringAsync($"{profile.ApplicationUserId}-Profile", JsonSerializer.Serialize(profile));
        await _cache.RemoveAsync("all:Profiles");
    }

    public async Task Delete(UserProfile profile)
    {
        await _profileRepository.Delete(profile);
        await _cache.RemoveAsync($"{profile.ApplicationUserId}-Profile");
        await _cache.RemoveAsync("all:Profiles");
    }

    public async Task SaveChangesAsync(string? userId = null)
    {
        if(userId != null)
        {
            await _cache.RemoveAsync($"{userId}-Profile");
            await _cache.RemoveAsync("all:Profiles");
        }

        await _profileRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserProfile>> GetAllProfiles()
    {
        var cacheData = await _cache.GetStringAsync("all:Profiles");
        if (!string.IsNullOrEmpty(cacheData))
            return JsonSerializer.Deserialize<IEnumerable<UserProfile>>(cacheData)!;

        var profiles = await _profileRepository.GetAllProfiles();
        await _cache.SetStringAsync("all:Profiles", JsonSerializer.Serialize(profiles));

        return profiles;
    }
}
