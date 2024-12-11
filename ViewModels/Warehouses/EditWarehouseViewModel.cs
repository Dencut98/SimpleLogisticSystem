using SimpleLogisticSystem.Data.Enum;
using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.ViewModels.Warehouses
{
    public class EditWarehouseViewModel
    {
        public int Id { get; set; }
        public string WarehouseName { get; set; }
        public WarehouseCompany WarehouseCompany { get; set; }
        public Address Address { get; set; }
        public int QuantityCapacity { get; set; }
        public double WeightCapacity { get; set; }
        public string CreatedBy { get; set; }
        public string AppUserId { get; set; }
        public List<string> CompanyNames { get; set; }

        public EditWarehouseViewModel()
        {
            CompanyNames = Enum.GetNames(typeof(WarehouseCompany)).ToList();
        }
    }
}
