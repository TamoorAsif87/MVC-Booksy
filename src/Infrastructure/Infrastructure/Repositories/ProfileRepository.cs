
namespace Infrastructure.Repositories;

public class ProfileRepository(StoreContext context) : IProfileRepository
{
    private readonly StoreContext _context = context;
    private readonly DbSet<UserProfile> _dbSet = context.Set<UserProfile>();

    public async Task<UserProfile?> GetByIdAsync(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
    }


    public async Task AddAsync(UserProfile profile)
    {
        await _dbSet.AddAsync(profile);
    }

    public Task Update(UserProfile profile)
    {
        _dbSet.Update(profile);
        return Task.CompletedTask;
    }

    public Task Delete(UserProfile profile)
    {
        _dbSet.Remove(profile);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(string? userId = null)
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserProfile>> GetAllProfiles()
    {
        var profiles = await _context.UserProfiles.ToListAsync();
        return profiles;
    }
}
