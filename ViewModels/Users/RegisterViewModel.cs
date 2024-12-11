using System.ComponentModel.DataAnnotations;

namespace SimpleLogisticSystem.ViewModels.Users
{
    public class RegisterViewModel
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required!")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required!")]
        [Compare("Password", ErrorMessage = "Password do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "First name is required!")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last name is required!")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Street is required!")]
        public string Street { get; set; }


        [Required(ErrorMessage = "City is required!")]
        public string City { get; set; }


        [Required(ErrorMessage = "Postal code is required!")]
        public string PostalCode { get; set; }


        [Required(ErrorMessage = "Country is required!")]
        public string Country { get; set; }
    }
}
