using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLogisticSystem.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        // Foreign key for Order
        [Required]
        public string OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        // Foreign key for Item
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        public int Quantity { get; set; }
        public double Weight { get; set; }
    }
}
