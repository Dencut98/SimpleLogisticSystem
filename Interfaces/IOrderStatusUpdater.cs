using SimpleLogisticSystem.Data.Enum;

namespace SimpleLogisticSystem.Interfaces
{
    public interface IOrderStatusUpdater
    {
        Task UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, TimeSpan delay);
    }
}
