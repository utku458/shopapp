namespace shopapp.entity;

public interface IOrderService
{
    void Create(Order entity);

    List<Order> GetOrders(string userId);
}