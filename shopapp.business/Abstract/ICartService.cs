namespace DefaultNamespace;

public interface ICartService
{
    void InitalizeCart(string userId);
    
    Cart GetCart(string userId);
    
    void AddToCart(string userId, int productId, int quantity);
    
    void DeleteFromCart(string userId, int productId);

    void UpdateQuantity(string userId, int productId, int quantity);
    
    
    void ClearCart(int cartId);

}