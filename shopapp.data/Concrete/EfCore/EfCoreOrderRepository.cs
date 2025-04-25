using System.Collections.Generic;
using System.Linq;
using shopapp.data.Abstract;
using shopapp.entity;
using Microsoft.EntityFrameworkCore;

namespace shopapp.data.Concrete.EfCore;

public class EfCoreOrderRepository:EfCoreGenericRepository<Order,ShopContext>,IOrderRepository
{
    // No need to override methods since base class implementations are sufficient

   public List<Order> GetOrders(string userId)
    {
        using (var context = new ShopContext())
        {
            var orders = context.Orders
                .Include(i => i.OrderItems)
                .ThenInclude(i => i.Product).AsQueryable();

            if (string.IsNullOrEmpty(userId))
            {
                orders = orders.Where(i => i.UserId == userId);
            }
                
            return orders.ToList();
        }
            
    }
}