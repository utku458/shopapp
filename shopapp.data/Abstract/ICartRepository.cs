using DefaultNamespace;

namespace shopapp.data.Abstract;

public interface ICartRepository:IRepository<Cart>
{
    Cart GetByUserId(string userId);

    void DeleteFromCart(int cartId, int productId);
    
    void UpdateQuantity(int cartId, int productId, int quantity);
    
    void ClearCart(int cartId);

}