namespace Core.ServiceContracts;

public interface ICart
{
    
    void AddToCart(Guid bookId, string? bookCover, int quantity, string name, string? author, string? categoryName, decimal price);
    void RemoveFromCart(Guid bookId);
    void ClearCart();
    void UpdateCart(Guid bookId, int quantity);
    IEnumerable<Cart> GetCartItems();
    decimal GetTotalCost();

    int GetTotalItems();
}
