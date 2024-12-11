using SimpleLogisticSystem.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLogisticSystem.Models
{
    public class Order
    {
        private static readonly Random random = new Random();

        public Order()
        {
            OrderId = GenerateRandomOrderId();
            OrderItems = new List<OrderItem>();
            OrderType = OrderType.Purchase;
            CreatedAt = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond);
        }

        [Key]
        public string OrderId { get; set; }

        [Required]
        public OrderType OrderType { get; set; }

        // Foreign key for AppUser
        public string AppUserId { get; set; }

        [ForeignKey("CreatedBy")]
        public string CreatedBy { get; set; }

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign key for Address
        public Address Address { get; set; }

        [ForeignKey("Address")]
        public int AddressId { get; set; }

        // Calculated properties
        public float TotalPrice {
            get
            {
                return OrderItems.Sum(item => item.Item.Price * item.Quantity);
            }
        }
        public float TotalWeight {
            get
            {
                return OrderItems.Sum(item => item.Item.Weight * item.Quantity);
            }
        }

        private string GenerateRandomOrderId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 9).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}