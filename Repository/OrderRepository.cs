using Microsoft.EntityFrameworkCore;
using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adds a new order to the database
        public bool Add(Order order)
        {
            _context.Add(order);
            return Save();
        }

        // Deletes an order from the database
        public bool Delete(Order order)
        {
            _context.Remove(order);
            return Save();
        }

        // Returns all orders from the database asynchronously
        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _context.Orders.ToListAsync();
        }

        // Retrieves all orders from the database including their items from the database asynchronously
        public async Task<IEnumerable<Order>> GetAllIncludingItems()
        {
            return await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Item).ToListAsync();
        }

        // Retrieves all orders including their addresses and items from the database asynchronously
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ToListAsync();
        }

        // Retrieves and order by its ID including its address and items from the database asynchronously
        public async Task<Order> GetbyIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public Task<Order> GetByIdAsyncNoTracking(string orderId)
        {
            throw new NotImplementedException();
        }

        // Retrieves orders by user ID including their addresses and items from the database asynchronously
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.AppUserId == userId)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ToListAsync();
        }

        // Saves changes to the database
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        // Updates an existing order in the database
        public bool Update(Order order)
        {
            _context.Update(order);
            return Save();
        }
    }
}
