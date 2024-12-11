using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLogisticSystem.Models
{
    public class WarehouseItem
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to Warehouse
        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        // Foreign key to Item
        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int Quantity { get; set; }
        public double Weight { get; set; }
    }
}
