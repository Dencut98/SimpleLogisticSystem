using SimpleLogisticSystem.Data.Enum;

namespace SimpleLogisticSystem.ViewModels.Items
{
    public class EditItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public float Weight { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string CreatedBy { get; set; }
        public string AppUserId { get; set; }
        public List<string> CategoryNames { get; set; }

        public EditItemViewModel()
        {
            CategoryNames = Enum.GetNames(typeof(ItemCategory)).ToList();
        }
    }
}
