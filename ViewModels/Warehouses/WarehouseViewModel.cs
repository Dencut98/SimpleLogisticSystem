using SimpleLogisticSystem.Data.Enum;

namespace SimpleLogisticSystem.ViewModels.Warehouses
{
    public class WarehouseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public WarehouseCompany Company { get; set; }
    }
}
