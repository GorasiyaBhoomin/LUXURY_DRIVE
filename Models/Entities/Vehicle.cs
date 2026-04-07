using System.ComponentModel.DataAnnotations;

namespace LUXURY_DRIVE.Models.Entities
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(100)]
        public string? SubTitle { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string PriceDay { get; set; }
        
        [Required]
        public string ImageUrl { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Acc { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string TopSpeed { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Horsepower { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Mileage { get; set; }
        
        [Required]
        public string Description { get; set; }

        [MaxLength(50)]
        public string? Seats { get; set; }

        [MaxLength(50)]
        public string? Transmission { get; set; }

        [MaxLength(50)]
        public string? Fuel { get; set; }

        [MaxLength(50)]
        public string? Year { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        public string? KeyFeatures { get; set; }
    }
}
