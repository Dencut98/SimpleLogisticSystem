using SimpleLogisticSystem.ViewModels.Warehouses;
using System.ComponentModel.DataAnnotations;

namespace SimpleLogisticSystem.ViewModels.Orders
{
    public class CreateOrderViewModel
    {
        public string CreatedBy { get; set; }
        public string AppUserId { get; set; }

        [Required]
        public List<OrderItemViewModel> OrderItems { get; set; }
        public List<WarehouseItemViewModel> WarehouseItems { get; set; }
    }
}
