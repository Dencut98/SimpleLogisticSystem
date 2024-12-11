using SimpleLogisticSystem.Data.Enum;
using SimpleLogisticSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SimpleLogisticSystem.ViewModels.Items
{
    public class CreateItemViewModel
    {
        public int ItemId { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        public ItemCategory ItemCategory { get; set; }
        public List<string> CategoryNames { get; set; }
        [Required]
        public float Weight { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        public int Quantity { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0,")]
        public float Price { get; set; }
        public string CreatedBy { get; set; }
        public string AppUserId { get; set; }

        public CreateItemViewModel()
        {
            CategoryNames = Enum.GetNames(typeof(ItemCategory)).ToList();
        }
    }
}
