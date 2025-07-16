using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Core.Services;
public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IPublishEndpoint publishEndpoint, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _publishEndpoint = publishEndpoint;
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            Name = registerDto.Name
        };
        var result =  await _userManager.CreateAsync(user, registerDto.Password);

        // create profile as task using masstransit

        if(result.Succeeded)
        {
            await _publishEndpoint.Publish<UserCreated>(new UserCreated
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name
            });
        }


        return result;

    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) throw new Exception($"No user found with email {loginDto.Email}");

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);
        if (!result.Succeeded) throw new Exception("invalid credientials.password or email is invalid.");

        return "success";
    }

    public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = $"User not found with email {dto.Email}" });

        return await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new Exception($"User not found with email {email}");

        var token =  await _userManager.GeneratePasswordResetTokenAsync(user);

        var scheme = _httpContextAccessor.HttpContext!.Request.Scheme;
        var host = _httpContextAccessor.HttpContext.Request.Host.Value;
        var encodedToken = Uri.EscapeDataString(token);

        // Construct the reset link
        var resetLink = $"{scheme}://{host}/account/reset-password?email={email}&token={encodedToken}";

        // Read HTML template
        var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "templates", "forgot-password-template.html");
        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Email template not found.", templatePath);

        var htmlTemplate = await File.ReadAllTextAsync(templatePath);

        // Replace the {link} placeholder
        var htmlBody = htmlTemplate.Replace("{link}", resetLink);

        if (!string.IsNullOrEmpty(encodedToken))
        {
            await _publishEndpoint.Publish<EmailForgotPassword>(new EmailForgotPassword
            {
                EmailTo = user.Email!,
                Subject = "Password Reset Request",
                Body = "Please click the link below to reset your password.",
                HtmlTemplate = htmlBody,
                UserName = user.UserName!

            });
        }


        return true;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        var decodeToken = Uri.UnescapeDataString(dto.Token);
        return await _userManager.ResetPasswordAsync(user, decodeToken, dto.NewPassword);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

   
}
