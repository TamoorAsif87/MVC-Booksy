namespace UI.Areas.admin.Controllers;

[Authorize(Roles = "admin")]
[Area("admin")]
public class DashboardController : Controller
{
    [Route("admin/home")]
    public IActionResult Home()
    {
        return View();
    }



}
