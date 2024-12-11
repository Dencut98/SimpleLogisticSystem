using Microsoft.EntityFrameworkCore;
using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adds a new item to the database
        public bool Add(Item item)
        {
            _context.Add(item);
            return Save();
        }

        // Deletes and existing item from the database
        public bool Delete(Item item)
        {
            _context.Remove(item);
            return Save();
        }

        // Retrieves all items from the database asynchronously
        public async Task<IEnumerable<Item>> GetAll()
        {
            return await _context.Items.ToListAsync();
        }

        // Retrieves an item by its id from the database asynchronously
        public async Task<Item> GetbyIdAsync(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id);
        }

        // Saves changes to the database
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        // Updates an existing item in the database
        public bool Update(Item item)
        {
            _context.Update(item);
            return Save();
        }
    }
}
