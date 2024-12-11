using SimpleLogisticSystem.ViewModels.Warehouses;

namespace SimpleLogisticSystem.ViewModels.Report
{
    public class InventoryReportViewModel
    {
        public string ItemName { get; set; }
        public int QuantityNotStored { get; set; }
        public int QuantityInWarehouse { get; set; }
        public List<WarehouseDetailViewModel> WarehouseDetails { get; set; }
    }
}
