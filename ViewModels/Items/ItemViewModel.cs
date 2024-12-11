using SimpleLogisticSystem.Data.Enum;

namespace SimpleLogisticSystem.ViewModels.Items
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public double Price { get; set; }
        public float Weight { get; set; }
        public int Quantity { get; set; }
        public string CreatedBy { get; set; }
        public string AppUserId { get; set; }
    }
}
