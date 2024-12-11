using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleLogisticSystem.Models
{
    public class AppUser : IdentityUser
    {
        public string? AccountType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Foreign key for Address
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        // Navigation properties
        public ICollection<Warehouse> Warehouses { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
