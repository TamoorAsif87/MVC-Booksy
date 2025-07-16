namespace UI.Areas.admin.Controllers;

[Area("admin")]
[Authorize]
public class OrdersController(IOrderService _orderService) : Controller
{
    [Route("admin/orders")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return View(orders);
    }

    [HttpPost]
    [Route("admin/order/shipped/{orderId}")]
    public async Task<IActionResult> MarkAsShipped(Guid orderId)
    {
        await _orderService.MarkAsShipped(orderId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Route("admin/order/delivered/{orderId}")]
    public async Task<IActionResult> MarkAsDelivered(Guid orderId)
    {
        await _orderService.MarkAsDelivered(orderId);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("admin/orders/details/{orderId}")]
    public async Task<IActionResult> GetOrderDetails(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        return View("OrderDetails",order);
    }
}
