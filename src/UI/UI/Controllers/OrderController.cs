namespace UI.Controllers;

public class OrderController(IOrderService _orderService,ICart _cartService,IHttpContextAccessor _httpContextAccessor, IRazorViewToStringRenderer _razorViewToStringRenderer,IConverter _converter) : Controller
{
    [Route("order/create")]
    [HttpGet]   
    public IActionResult OrderCreation()
    {
        var items = _cartService.GetCartItems();
        if(items == null || !items.Any())
        {
            ViewBag.ErrorMessage = "Your cart is empty. Please add items to your cart before placing an order.";
            return View("Error");
        }

        var model = new OrderVM
        {
            Items = items,
            Order = new OrderDto
            {
                CustomerId = User?.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                
            }
        };

        return View(model);
    }

    [Route("order/create")]
    [HttpPost]
    public async Task<IActionResult> OrderCreation(OrderVM orderVM)
    {
        try
        {
            await _orderService.CreateOrderAsync(orderVM.Order!);
            return RedirectToAction(actionName: "Checkout");
        }
        catch (Exception ex)
        {

            ModelState.AddModelError("error", ex.Message);
        }

        orderVM.Items = _cartService.GetCartItems();
        return View(orderVM);
    }

    [Route("order/checkout")]
    public async Task<IActionResult> Checkout()
    {
        var orderId = Guid.Parse(_httpContextAccessor.HttpContext?.Session.GetString("order:Id")!);

        if(orderId == Guid.Empty)
        {
            throw new Exception("Something went wrong while creating order");
        }

        var order = await _orderService.GetOrderByIdAsync(orderId);

        return View(order);
    }

    [Route("order/cancel/{orderId}/{userId}")]
    public async Task<IActionResult> Cancel(Guid orderId,string userId)
    {
        if(User.FindFirstValue(ClaimTypes.NameIdentifier) != userId)
        {
            return Unauthorized();
        }

        try
        {
            await _orderService.CancelOrder(orderId);
            TempData["success"] = "Order Canceled";
        }
        catch (Exception ex)
        {

            TempData["error"] = ex.Message;
        }



        return RedirectToAction(controllerName: nameof(UserController).Replace("Controller", ""), actionName: nameof(UserController.UserOrders));
    }

    [Route("order/payment")]
    [HttpPost]
    public async Task<IActionResult> PaymentStripe()
    {
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

    [Route("checkout/success")]

    public async Task<IActionResult> Success()
    {
        try
        {
            var orderSuccessId = await _orderService.PaymentSuccessful();
            return View("PaymentSuccess",orderSuccessId);
            
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }

    }

    [Route("checkout/failure")]

    public IActionResult Failure()
    {
        ViewBag.OrderId = HttpContext.Session.GetString("order:Id");
        return View("PaymentFailure");
    }

    [Route("invoices/{orderId}.pdf")]

    public async Task<IActionResult> GeneratePdf(Guid orderId)
    {
        // 1. Fetching order data
        var order = await _orderService.GetOrderByIdAsync(orderId);

        // 2. Render Razor view to HTML string
        var html = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Invoices/PdfTemplate.cshtml", order);

        // 3. Create the PDF document settings
        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
                Margins = new MarginSettings { Top = 30, Bottom = 30, Left = 20, Right = 20 },
                DocumentTitle = $"Invoice_{orderId}"
            },
            Objects =
            {
                new ObjectSettings
                {
                    HtmlContent = html,
                    WebSettings = new WebSettings
                    {
                        DefaultEncoding = "utf-8",
                        LoadImages = true
                    }
                }
            }
        };

     
        // 4. Convert HTML to PDF
        var pdfBytes = _converter.Convert(doc);

        // 5. Return the PDF file
        return File(pdfBytes, "application/pdf", $"Invoice_{orderId}.pdf");
    }
}
