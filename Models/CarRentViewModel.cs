using System.ComponentModel.DataAnnotations;
using System;

namespace LUXURY_DRIVE.Models
{
    public class CarRentViewModel
    {
        public int? RentId { get; set; }
        public int? VehicleId { get; set; }
        public int? UserId { get; set; }
        public string? Status { get; set; } // Optional: To support e.g. "Confirmed", "Pending", "Active", "Cancelled"

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Driving license number is required.")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "License expiry date is required.")]
        public DateTime? LicenseExpiryDate { get; set; }

        [Required(ErrorMessage = "Pickup date is required.")]
        public DateTime? PickupDate { get; set; }

        [Required(ErrorMessage = "Pickup time is required.")]
        public string PickupTime { get; set; }

        [Required(ErrorMessage = "Number of days is required.")]
        [Range(1, 30, ErrorMessage = "Number of days must be between 1 and 30.")]
        public int? NumberOfDays { get; set; }
    }
}