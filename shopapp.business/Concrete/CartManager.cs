using shopapp.data.Abstract;

namespace DefaultNamespace;

public class CartManager : ICartService
{

    private ICartRepository _cartRepository;

    public CartManager(ICartRepository cartRepository)
    {

        _cartRepository = cartRepository;
    }
    
    

    public void InitalizeCart(string userId)
    {
        _cartRepository.Create(new Cart()
        {
            UserId = userId,

        });
    }

    public Cart GetCart(string userId)
    {
        return _cartRepository.GetByUserId(userId);
    }

    public void AddToCart(string userId, int productId, int quantity)
    {
        var cart = GetCart(userId);
        if (cart != null)
        {
            var index = cart.CartItems.FindIndex(x => x.ProductId == productId);

            if (index < 0)
            {
                cart.CartItems.Add(new CartItem()
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.Id
                });
            }
            else
            {
                cart.CartItems[index].Quantity += quantity;
            }
            
            _cartRepository.Update(cart);
        }
    }

    public void DeleteFromCart(string userId, int productId)
    {
        var cart = GetCart(userId);

        if (cart!=null)
        {
          _cartRepository.DeleteFromCart(cart.Id, productId);
        }
    }

    public void UpdateQuantity(string userId, int productId, int quantity)
    {
        var cart = GetCart(userId);

        if (cart != null)
        {
            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;

                // Business mantık burada tamam, şimdi veri katmanına gönderelim
                _cartRepository.UpdateQuantity(cart.Id, productId, quantity);

            }
        }
    }

    public void ClearCart(int cartId)
    {
        _cartRepository.ClearCart(cartId);
    }
}