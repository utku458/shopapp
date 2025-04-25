using System.Linq;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using shopapp.data.Abstract;

namespace shopapp.data.Concrete.EfCore;

public class EfCoreCartRepository:EfCoreGenericRepository<Cart,ShopContext>,ICartRepository
{
    public Cart GetByUserId(string userId)
    {
        using (var context = new ShopContext())
        {
            return context.Carts.Include(i=>i.CartItems)
                .ThenInclude(i=>i.Product).FirstOrDefault(i=>i.UserId == userId);
        }
    }
    
    
    public override void Update(Cart entity)
    {
        using (var context = new ShopContext())
        {
           context.Carts.Update(entity);
           context.SaveChanges();
        }
        
       
    }

    public void DeleteFromCart(int cartId, int productId)
    {
        using (var context = new ShopContext())
        {
            var cmd = @"DELETE FROM CartItems WHERE CartId = @p0 AND ProductId = @p1";
            context.Database.ExecuteSqlRaw(cmd,cartId,productId);
        }
    }
    
    public void UpdateQuantity(int cartId, int productId, int quantity)
    {
        using (var context = new ShopContext())
        {
            var item = context.CartItems
                .FirstOrDefault(i => i.CartId == cartId && i.ProductId == productId);

            if (item != null)
            {
                item.Quantity = quantity;
                context.SaveChanges();
            }
        }
    }

    public void ClearCart(int cartId)
    {
        using (var context = new ShopContext())
        {
            var cmd = @"DELETE FROM CartItems WHERE CartId = @p0";
            context.Database.ExecuteSqlRaw(cmd,cartId);
        }
    }
}