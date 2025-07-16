namespace UI.Areas.admin.Controllers;

[Area("admin")]
[Authorize(Roles = "admin")]
public class UsersController(IUserActions userActions) : Controller
{
    [Route("admin/users")]
    public async Task<IActionResult> Index()
    {
        var users = await userActions.GetAllProfilesAsync();
        return View(users);
    }
}
