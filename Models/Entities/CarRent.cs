using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LUXURY_DRIVE.Models.Entities
{
    public class CarRent
    {
        [Key]
        public int RentId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; }
        
        public DateTime LicenseExpiryDate { get; set; }
        
        public DateTime PickupDate { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string PickupTime { get; set; }
        
        public int NumberOfDays { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        public int? VehicleId { get; set; }
        
        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }

        public int? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
