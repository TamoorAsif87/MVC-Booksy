namespace Core.ServiceContracts;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterDto registerDto);
    Task<string?> LoginAsync(LoginDto loginDto); 
    Task<IdentityResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<bool> ForgotPasswordAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetDto);
    Task LogoutAsync();
}
