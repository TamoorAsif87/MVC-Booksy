namespace UI.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    [Route("account/register")]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [Route("account/register")]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto register)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("error", string.Join(Environment.NewLine, ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage)));
            return View(register);
        }

        var result = await accountService.RegisterAsync(register);

        if(!result.Succeeded)
        {
            ModelState.AddModelError("error", string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
            return View(register);
        }

        return RedirectToAction("Login") ;
    }

    [Route("account/login")]
    [HttpGet]
    public IActionResult Login(string returnurl = null)
    {
        ViewBag.ReturnUrl = returnurl;
        return View();
    }

    [Route("account/login")]
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto login,string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("error", string.Join(Environment.NewLine,
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(login);
        }

        try
        {
            var result = await accountService.LoginAsync(login);
            if (result == "success")
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(login);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }


    [Authorize]
    [Route("account/change-password")]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        string? email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        var model = new ChangePasswordDto();
        model.Email = email;

        return View(model);
    }

    [Authorize]
    [Route("account/change-password")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("error", string.Join(Environment.NewLine,
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(dto);
        }

        var result = await accountService.ChangePasswordAsync(dto);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("error", string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
            return View(dto);
        }

        TempData["Success"] = "Password changed successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("account/forgot-password")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [Route("account/forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("error", string.Join(Environment.NewLine,
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(model);
        }

        try
        {
            var result = await accountService.ForgotPasswordAsync(model.Email);

            if (result)
            {
                TempData["Toast.Type"] = "Success";
                TempData["Toast.Message"] = "If an account with that email exists, we've sent password reset instructions.";
                TempData["Toast.Title"] = "Password Reset";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Unable to process the request. Please try again.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(model);
    }

    [Route("account/reset-password")]
    [HttpGet]
    public IActionResult ResetPassword(string email,string token)
    {

        var model = new ResetPasswordDto
        {
            Token = token,
            Email = email
        };

        return View(model);
    }

    [Route("account/reset-password")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("error", string.Join(Environment.NewLine,
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(dto);
        }

        try
        {
            var result = await accountService.ResetPasswordAsync(dto);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("error", string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                return View(dto);
            }

            TempData["Toast.Type"] = "Success";
            TempData["Toast.Message"] = "Your password has been reset successfully. You can now log in."; 
            TempData["Toast.Title"] = "Password Reset";

            return RedirectToAction("Login", "Account");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }
}
