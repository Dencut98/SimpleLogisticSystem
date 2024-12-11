using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<Warehouse>> GetAll();
        Task<Warehouse> GetbyIdAsync(int id);
        Task<Warehouse> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<WarehouseItem>> GetWarehouseItemsByWarehouseId(int warehouseId);
        Task<IEnumerable<Warehouse>> GetAllIncludingAddressAndItems();
        Task<IEnumerable<WarehouseItem>> GetAllWarehouseItems();
        Task<IEnumerable<Warehouse>> GetAllIncludingItems();
        bool Add(Warehouse warehouse);
        bool Update(Warehouse warehouse);   
        bool Delete(Warehouse warehouse);
        bool Save();
    }
}
