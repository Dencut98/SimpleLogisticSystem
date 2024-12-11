using SimpleLogisticSystem.Data.Enum;
using SimpleLogisticSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SimpleLogisticSystem.ViewModels.Warehouses
{
    public class CreateWarehouseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Warehouse Name is required")]
        public string WarehouseName { get; set; }

        public WarehouseCompany WarehouseCompany { get; set; }
        public Address Address { get; set; } = new Address();

        [Required(ErrorMessage = "Quantity Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity Capacity must be greater than 0")]
        public int QuantityCapacity { get; set; }

        [Required(ErrorMessage = "Weight Capacity is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight Capacity must be greater than 0")]
        public double WeightCapacity { get; set; }

        public string CreatedBy { get; set; }
        public string AppUserId { get; set; }
        public List<string> CompanyNames { get; set; }

        public CreateWarehouseViewModel()
        {
            CompanyNames = Enum.GetNames(typeof(WarehouseCompany)).ToList();
        }
    }
}
