using Microsoft.AspNetCore.Localization;

namespace UI.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult Set(string culture, string returnUrl)
    {
        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            cookieValue,
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }
}
