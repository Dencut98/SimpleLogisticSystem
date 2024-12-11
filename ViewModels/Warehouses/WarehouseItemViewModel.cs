using SimpleLogisticSystem.Data.Enum;

namespace SimpleLogisticSystem.ViewModels.Warehouses
{
    public class WarehouseItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public float Weight { get; set; }
        public ItemCategory ItemCategory { get; set; }
    }
}
