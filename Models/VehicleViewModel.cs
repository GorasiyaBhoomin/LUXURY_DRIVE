using System.ComponentModel.DataAnnotations;

namespace LUXURY_DRIVE.Models
{
    public class VehicleViewModel
    {
        public int? VehicleId { get; set; }

        [Required(ErrorMessage = "Vehicle Name is required.")]
        public string Name { get; set; }

        public string SubTitle { get; set; } // Optional

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Price / Day is required.")]
        public string PriceDay { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "0-100 km/h acceleration is required.")]
        public string Acc { get; set; }

        [Required(ErrorMessage = "Top Speed is required.")]
        public string TopSpeed { get; set; }

        [Required(ErrorMessage = "Horsepower is required.")]
        public string Horsepower { get; set; }

        [Required(ErrorMessage = "Mileage is required.")]
        public string Mileage { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string? Seats { get; set; }
        public string? Transmission { get; set; }
        public string? Fuel { get; set; }
        public string? Year { get; set; }
        public string? Color { get; set; }
        public string? KeyFeatures { get; set; }
    }
}
