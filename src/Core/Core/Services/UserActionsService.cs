using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Core.Services;

public class UserActionsService(IProfileRepository profileRepository,IWebHostEnvironment _env,IFileUpload fileUpload,UserManager<ApplicationUser> userManager,IOrderService orderService) : IUserActions
{
    public async Task<bool> DeleteAccountAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
       
        if (user == null) throw new Exception("User not Found");
        var tempUserId = user.Id;

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            await profileRepository.SaveChangesAsync(tempUserId);
            return true;
        }
        throw new Exception(string.Join(", ", result.Errors));
    }

    public async Task<IEnumerable<UserProfile>> GetAllProfilesAsync()
    {
        var profiles = await profileRepository.GetAllProfiles();
        return profiles;
    }

    public async Task<OrderDto?> GetOrderDetailsAsync(string userId, Guid orderId)
    {
        var order = await orderService.GetOrderDetailsOfCustomer(userId, orderId);
        if (order == null) throw new Exception($"Order Not Found With Id {orderId}");
        return order;
    }

    public async Task<UserProfile> GetProfileAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.");

        var profile = await profileRepository.GetByIdAsync(userId);

        return profile ?? throw new KeyNotFoundException("User profile not found.");
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await orderService.GetOrdersByCustomerIdAsync(userId);
        return orders;
    }

    public async Task<bool> UpdateProfileAsync(string userId, UserProfile profileUpdate,IFormFile? file)
    {
       
        if (profileUpdate == null)
            throw new ArgumentNullException(nameof(profileUpdate));

        var existingProfile = await profileRepository.GetByIdAsync(userId);
        if (existingProfile == null)
            return false;

        // Update fields (example: only Name and Email, expand as needed)

        if (file != null)
        {
            existingProfile.ProfilePicture = await fileUpload.UploadImage("uploads/images", _env.WebRootPath, $"{Guid.NewGuid()}_{file.FileName}", file);
        }

        existingProfile.Name = profileUpdate.Name;
        existingProfile.Email = profileUpdate.Email;
        await profileRepository.Update(existingProfile);
        await profileRepository.SaveChangesAsync();

        return true;
    }
}
