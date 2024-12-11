using SimpleLogisticSystem.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLogisticSystem.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public string? ItemName { get; set; }
        public float Weight { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }

        // Foreign key for AppUser
        public string? AppUserId { get; set; }
        [ForeignKey("CreatedBy")]
        public string? CreatedBy { get; set; }
        public AppUser? AppUser { get; set; }

        // Navigation properties
        public ICollection<WarehouseItem> WarehouseItems { get; set; } = new List<WarehouseItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
