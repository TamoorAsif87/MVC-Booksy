using Microsoft.AspNetCore.Http;

namespace Core.ServiceContracts;

public interface IUserActions
{
    /// <summary>
    /// Gets the current user's profile.
    /// </summary>
    /// <returns>User profile details.</returns>
    Task<UserProfile> GetProfileAsync(string userId);


    /// <summary>
    /// Gets the all user's profile.
    /// </summary>
    /// <returns>All Users profile details.</returns>
    Task<IEnumerable<UserProfile>> GetAllProfilesAsync();


    /// <summary>
    /// Updates the current user's profile.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="profileUpdate">Data for profile update.</param>
    /// <returns>True if update succeeded.</returns>
    Task<bool> UpdateProfileAsync(string userId, UserProfile profileUpdate,IFormFile? file);

    // <summary>
    // Gets the list of orders for the current user.
    // </summary>
    // <param name = "userId" > User identifier.</param>
    // <returns>List of user orders.</returns>
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);

    // <summary>
    // Gets the details of a specific order.
    // </summary>
    Task<OrderDto?> GetOrderDetailsAsync(string userId, Guid orderId);

    // <summary>
    // Deletes the user's account.
    // </summary>
    Task<bool> DeleteAccountAsync(string userId);

    
}
