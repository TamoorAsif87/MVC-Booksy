using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Core.Services;
public class CartService : ICart
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "cart";

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private List<Cart> CartItems
    {
        get
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var cartData = session?.GetString(CartSessionKey);

            return cartData != null
                ? JsonSerializer.Deserialize<List<Cart>>(cartData)!
                : new List<Cart>();
        }
        set
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var json = JsonSerializer.Serialize(value);
            session?.SetString(CartSessionKey, json);
        }
    }

    public void AddToCart(Guid bookId, string? bookCover, int quantity, string name, string? author, string? categoryName, decimal price)
    {
        var items = CartItems;

        var existing = items.FirstOrDefault(i => i.BookId == bookId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            items.Add(new Cart
            {
                BookId = bookId,
                BookCover = bookCover,
                Quantity = quantity,
                Name = name,
                Author = author,
                CategoryName = categoryName,
                Price = price
            });
        }

        CartItems = items;
    }

    public void RemoveFromCart(Guid bookId)
    {
        var items = CartItems;
        var item = items.FirstOrDefault(i => i.BookId == bookId);
        if (item != null)
        {
            items.Remove(item);
            CartItems = items;
        }
    }

    public void ClearCart()
    {
        CartItems = new List<Cart>();
    }

    public void UpdateCart(Guid bookId, int quantity)
    {
        var items = CartItems;
        var item = items.FirstOrDefault(i => i.BookId == bookId);
        if (item != null)
        {
            item.Quantity = quantity;
            CartItems = items;
        }
    }

    public IEnumerable<Cart> GetCartItems()
    {
        return CartItems;
    }

    public decimal GetTotalCost()
    {
        return CartItems.Sum(i => i.Total);
    }

    public int GetTotalItems()
    {
        return CartItems.Count;
    }
}
