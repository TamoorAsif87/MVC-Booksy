using Shared.Contracts;
using System.Threading.Tasks;

namespace UI.Controllers;

public class CartController : Controller
{
    private readonly ICart _cartService;
    private readonly IRecommender _recommender;
    private readonly IBookService _bookService;
    public CartController(ICart cartService, IRecommender recommender, IBookService bookService)
    {
        _cartService = cartService;
        _recommender = recommender;
        _bookService = bookService;
    }

    [HttpPost]
    [Route("cart/add")]
    public IActionResult AddInCart([FromBody] Cart cart)
    {
        try
        {
            _cartService.AddToCart(cart.BookId, cart.BookCover, cart.Quantity, cart.Name, cart.Author, cart.CategoryName, cart.Price);
            var totalItems = _cartService.GetTotalItems();
            return Json(new { success = true, message = "Successfully added to cart", itemsCount = totalItems });
        }
        catch (Exception ex)
        {

            return Json(new { success = false, message = ex.Message });
        }


    }



    [HttpPost]
    public IActionResult AddToCart([FromForm] Cart cart)
    {
       
        _cartService.AddToCart(cart.BookId, cart.BookCover, cart.Quantity, cart.Name, cart.Author, cart.CategoryName, cart.Price);
        return RedirectToAction(nameof(GetCart));
    }



    [HttpGet("cart/mini-view")]
    public IActionResult MiniView()
    {
        return PartialView("_MiniCartView");
    }

    [Route("cart")]
    public async Task<IActionResult> GetCart()
    {
        var cartItems = _cartService.GetCartItems();
        var bookIds = _recommender.SuggestedBooksFor(cartItems.Select(x => x.BookId).ToList());

        if (bookIds.Any())
        {
            ViewBag.boughtTogether = await _bookService.GetSuggestedBooksAsync(bookIds);
        }

        return View("cart", cartItems);
    }

    [Route("cart/update")]
    [HttpPost]
    public IActionResult UpdateCart([FromBody] CartUpdateModel cartUpdateModel)
    {
        try
        {
            _cartService.UpdateCart(cartUpdateModel.BookId, cartUpdateModel.Quantity);
            return Json(new { success = true, message = "cart updated" });
        }
        catch (Exception ex)
        {

            return Json(new { success = false, message = ex.Message });
        }
    }

    [Route("cart/remove")]
    [HttpPost]
    public IActionResult RemoveFromCart([FromBody] CartRemoveModel cartRemoveModel)
    {
        try
        {
            _cartService.RemoveFromCart(cartRemoveModel.BookId);
            return Json(new { success = true, message = "cart updated",itemsCount = _cartService.GetTotalItems() });
        }
        catch (Exception ex)
        {

            return Json(new { success = false, message = ex.Message });
        }
    }

    [Route("cart/clear")]
    [HttpPost]
    public IActionResult CartClear()
    {

        _cartService.ClearCart();
        return RedirectToAction(nameof(GetCart));


    }

    [Route("cart/update/ajax")]
    public IActionResult UpdateCartView()
    {
        var cartItems = _cartService.GetCartItems();
        return PartialView("_CartPartialView", cartItems);
    }

}
