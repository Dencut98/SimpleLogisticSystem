using SimpleLogisticSystem.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLogisticSystem.Models
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }
        public string? WarehouseName { get; set; }
        public WarehouseCompany WarehouseCompany { get; set; }

        // Foreign key to Address
        [ForeignKey("Address")]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        public int QuantityCapacity { get; set; }
        public int CurrentQuantity { get; set; }
        public double CurrentWeight { get; set; }
        public double WeightCapacity { get; set; }

        // Foreign key to AppUser
        [ForeignKey("CreatedBy")]
        public string? CreatedBy { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        // Navigation property
        public ICollection<WarehouseItem> WarehouseItems { get; set; } = new List<WarehouseItem>();
    }
}
