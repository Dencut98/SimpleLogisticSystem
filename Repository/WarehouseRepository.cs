using Microsoft.EntityFrameworkCore;
using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext _context;

        public WarehouseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adds a new warehouse to the database
        public bool Add(Warehouse warehouse)
        {
            _context.Add(warehouse);
            return Save();
        }

        // Deletes a warehouse from the database
        public bool Delete(Warehouse warehouse)
        {
            _context.Remove(warehouse);
            return Save();  
        }

        // Retrieves all warehouses from the database asynchronously
        public async Task<IEnumerable<Warehouse>> GetAll()
        {
            return await _context.Warehouses.ToListAsync();
        }

        // Retrieves all warehouses from the database including their items asynchronously
        public async Task<IEnumerable<Warehouse>> GetAllIncludingItems()
        {
            return await _context.Warehouses.Include(w => w.WarehouseItems).ThenInclude(wi => wi.Item).ToListAsync();
        }

        public async Task<IEnumerable<WarehouseItem>> GetAllWarehouseItems()
        {
            return await _context.WarehouseItems.Include(wi => wi.Item).ToListAsync();
        }

        // Retrieves a warehouse by its ID including its address asynchronously
        public async Task<Warehouse> GetbyIdAsync(int id)
        {
            return await _context.Warehouses.Include(i => i.Address).FirstOrDefaultAsync(i => i.Id == id);
        }

        // Retrieves all warehouses including their addresses, warehouse items, and items asynchronously
        public async Task<IEnumerable<Warehouse>> GetAllIncludingAddressAndItems()
        {
            return await _context.Warehouses
                .Include(w => w.Address)
                .Include(w => w.WarehouseItems)
                .ThenInclude(wi => wi.Item)
                .ToListAsync();
        }

        public Task<Warehouse> GetByIdAsyncNoTracking(int id)
        {
            throw new NotImplementedException();
        }

        // Retrieves warehouse items by warehouse ID including their items from the database asynchronously
        public async Task<IEnumerable<WarehouseItem>> GetWarehouseItemsByWarehouseId(int warehouseId)
        {
            return await _context.WarehouseItems.Where(wi => wi.WarehouseId == warehouseId).Include(wi => wi.Item).ToListAsync();
        }

        // Saves changes to the database
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        // Updates a warehouse in the database
        public bool Update(Warehouse warehouse)
        {
            _context.Update(warehouse);
            return Save();
        }
    }
}
