using shopapp.data.Abstract;

namespace shopapp.entity;

public class OrderManager : IOrderService
{
    
    private IOrderRepository _orderRepository;

    public OrderManager(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        
    }
    public void Create(Order entity)
    {
        _orderRepository.Create(entity);
    }

    public List<Order> GetOrders(string userId)
    {
        return _orderRepository.GetOrders(userId);
    }
}