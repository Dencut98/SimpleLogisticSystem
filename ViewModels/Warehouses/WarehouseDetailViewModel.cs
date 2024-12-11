using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.ViewModels.Warehouses
{
    public class WarehouseDetailViewModel
    {
        public Warehouse Warehouse { get; set; }
        public int TotalQuantity { get; set; }
        public double TotalWeight { get; set; }
    }
}
