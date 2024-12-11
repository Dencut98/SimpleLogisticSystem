using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Data.Enum;
using SimpleLogisticSystem.Interfaces;

namespace SimpleLogisticSystem.Repository
{
    public class OrderStatusUpdaterRepository : IOrderStatusUpdater
    {
        private readonly IServiceScopeFactory _scopefactory;

        public OrderStatusUpdaterRepository(IServiceScopeFactory scopeFactory)
        {
            _scopefactory = scopeFactory;
        }

        // Updates the status of an order after a specified delay
        public async Task UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, TimeSpan delay)
        {
            // Wait for the delay
            await Task.Delay(delay);

            // Create a new scope to get a new instance of the ApplicationDbContext
            using (var scope = _scopefactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var order = await context.Orders.FindAsync(orderId);
                if (order != null)
                {
                    // Update the order status and save changes
                    order.OrderStatus = newStatus;
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
