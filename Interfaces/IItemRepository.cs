using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAll();
        Task<Item> GetbyIdAsync(int id);
        bool Add(Item item);
        bool Update(Item item);
        bool Delete(Item item);
        bool Save();
    }
}
