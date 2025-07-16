using Microsoft.AspNetCore.Authentication;

namespace UI.Controllers;

[Authorize]
public class UserController(IUserActions _userActions,IOrderService _orderService) : Controller
{
    [Route("user/profile")]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var profile = await _userActions.GetProfileAsync(userId);
        return View("EditProfile", profile);
    }

    // POST: /User/Profile
    [Route("user/profile")]
    [HttpPost]
    public async Task<IActionResult> Profile(UserProfile model, IFormFile? file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            return View("EditProfile", model);
        }

       

        var result = await _userActions.UpdateProfileAsync(userId, model,file);
        if (!result)
        {
            ModelState.AddModelError("", "Failed to update profile.");
            return View("EditProfile", model);
        }



        TempData["Toast.Type"] = "Success";
        TempData["Toast.Message"] = "Your Profile has been updated";
        TempData["Toast.Title"] = "Profile Updated";
        return RedirectToAction(nameof(Profile));
    }


    [Route("user/orders")]
    [HttpGet]
    public async Task<IActionResult> UserOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var orders = await _userActions.GetUserOrdersAsync(userId);
        return View("UserOrders", orders);
    }

    [Route("user/orders/{orderId}")]
    [HttpGet]
    public async Task<IActionResult> OrderDetails(Guid orderId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        try
        {
            var order = await _userActions.GetOrderDetailsAsync(userId, orderId);
            return View("OrderDetails", order);
        }
        catch (Exception ex)
        {
            TempData["Toast.Type"] = "Error";
            TempData["Toast.Message"] = ex.Message;
            TempData["Toast.Title"] = "Order View";
            return RedirectToAction(nameof(UserOrders));
        }
    }

    [Route("user/delete-account")]
    [HttpPost]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        try
        {
            var result = await _userActions.DeleteAccountAsync(userId);
            if (result)
            {
                await HttpContext.SignOutAsync(); // logout user
                TempData["Success"] = "Your account was deleted.";
                return RedirectToAction("Index", "Home");
            }

            TempData["Toast.Type"] = "Error";
            TempData["Toast.Message"] = "Could not delete your account.";
            TempData["Toast.Title"] = "Account Delete";

      
            return RedirectToAction(nameof(Profile));
        }
        catch (Exception ex)
        {
            TempData["Toast.Type"] = "Error";
            TempData["Toast.Message"] = ex.Message;
            TempData["Toast.Title"] = "Account Delete";
            return RedirectToAction(nameof(Profile));
        }
    }

    [HttpGet]
    [Route("user/order/checkout/{orderId}")]
    public async Task<IActionResult> UserOrderCheckout(Guid orderId)
    {
        HttpContext.Session.SetString("order:Id", orderId.ToString());
        try
        {
            var sessionUrl = await _orderService.CheckOut();
            return Redirect(sessionUrl);
        }
        catch (Exception ex)
        {

            return BadRequest("Could not initiate Stripe checkout.");
        }
    }
}
