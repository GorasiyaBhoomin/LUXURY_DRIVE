using System.ComponentModel.DataAnnotations;

namespace LUXURY_DRIVE.Models
{
    public class CustomerViewModel
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; } // Optional for edit, handled in controller
    }
}
