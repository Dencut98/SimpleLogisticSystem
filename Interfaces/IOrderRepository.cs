using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAll();
        Task<Order> GetbyIdAsync(string orderId);
        Task<Order> GetByIdAsyncNoTracking(string orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetAllIncludingItems();
        bool Add(Order order);
        bool Update(Order order);
        bool Delete(Order order);
        bool Save();
    }
}
