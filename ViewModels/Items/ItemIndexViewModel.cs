using SimpleLogisticSystem.Data.Enum;
using SimpleLogisticSystem.ViewModels.Warehouses;

namespace SimpleLogisticSystem.ViewModels.Items
{
    public class ItemIndexViewModel
    {
        public IEnumerable<ItemViewModel> Items { get; set; }
        public IEnumerable<WarehouseViewModel> Warehouses { get; set; }
        public List<string> CategoryNames { get; set; }

        public ItemIndexViewModel()
        {
            CategoryNames = Enum.GetNames(typeof(ItemCategory)).ToList();
        }
    }
}
