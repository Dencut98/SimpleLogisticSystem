using SimpleLogisticSystem.Data.Enum;
using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.ViewModels.Orders
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }
        public OrderType OrderType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public OrderStatus OrderStatus { get; set; }
        public Address Address { get; set; }
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
    }
}
