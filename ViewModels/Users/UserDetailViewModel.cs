using Microsoft.AspNetCore.Identity;
using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.ViewModels.Users
{
    public class UserDetailViewModel
    {
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public string AppUserId { get; set; }
        public ICollection<Warehouse> Warehouses { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}

