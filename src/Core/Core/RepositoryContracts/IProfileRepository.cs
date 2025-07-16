namespace Core.RepositoryContracts;

public interface IProfileRepository
{
    // Get profile by ID
    Task<UserProfile?> GetByIdAsync(string userId);

    // Get All Profiles

    Task<IEnumerable<UserProfile>> GetAllProfiles();

    // Add a new profile
    Task AddAsync(UserProfile profile);

    // Update a profile
    Task Update(UserProfile profile);

    // Delete a profile
    Task Delete(UserProfile profile);

    
    Task SaveChangesAsync(string? userId = null);
}
