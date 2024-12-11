using SimpleLogisticSystem.Models;

namespace SimpleLogisticSystem.ViewModels.Users
{
    public class EditUserDetailViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public string AppUserId { get; set; }
    }
}
