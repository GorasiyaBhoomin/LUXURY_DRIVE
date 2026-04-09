using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LUXURY_DRIVE.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoDataController : ControllerBase
    {
        // Mock database of automobile specifications
        private static readonly List<Dictionary<string, string>> _carDatabase = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> {
                {"name", "Audi R8"}, {"subTitle", "V10 Performance"}, {"category", "Sports"}, 
                {"priceDay", "20000"}, {"acc", "3.2s"}, {"topSpeed", "330 km/h"}, {"horsepower", "620 HP"}, 
                {"mileage", "8 km/l"}, {"seats", "2"}, {"transmission", "Automatic"}, {"fuel", "Petrol"}, 
                {"year", "2024"}, {"color", "Grey"}, 
                {"description", "The Audi R8 V10 Performance is a naturally aspirated supercar like no other. With its mid-mounted V10 engine screaming to 8,700 rpm and quattro all-wheel drive, it delivers a pure, visceral driving experience wrapped in stunning Italian-designed bodywork."}
            },
            new Dictionary<string, string> {
                {"name", "BMW M5"}, {"subTitle", "Competition"}, {"category", "Luxury"}, 
                {"priceDay", "10000"}, {"acc", "3.5s"}, {"topSpeed", "305 km/h"}, {"horsepower", "600 HP"}, 
                {"mileage", "12 km/l"}, {"seats", "5"}, {"transmission", "Automatic"}, {"fuel", "Petrol"}, 
                {"year", "2024"}, {"color", "Blue"},
                {"description", "The BMW M5 Competition is the pinnacle of performance sedans, combining jaw-dropping speed with everyday refinement."}
            },
            new Dictionary<string, string> {
                {"name", "Lamborghini Huracan"}, {"subTitle", "EVO"}, {"category", "Sports"}, 
                {"priceDay", "30000"}, {"acc", "2.9s"}, {"topSpeed", "325 km/h"}, {"horsepower", "640 HP"}, 
                {"mileage", "6 km/l"}, {"seats", "2"}, {"transmission", "Automatic"}, {"fuel", "Petrol"}, 
                {"year", "2024"}, {"color", "Yellow"},
                {"description", "The Lamborghini Huracán EVO is a masterpiece of Italian engineering — a naturally-aspirated V10 howls to 8,000 rpm."}
            },
            new Dictionary<string, string> {
                {"name", "Mercedes S-Class"}, {"subTitle", "S 500 4MATIC"}, {"category", "Luxury"}, 
                {"priceDay", "12000"}, {"acc", "4.3s"}, {"topSpeed", "250 km/h"}, {"horsepower", "435 HP"}, 
                {"mileage", "13 km/l"}, {"seats", "5"}, {"transmission", "Automatic"}, {"fuel", "Petrol"}, 
                {"year", "2024"}, {"color", "Black"},
                {"description", "The Mercedes-Benz S-Class represents the very best in automotive luxury with massaging seats and cutting edge tech."}
            },
            new Dictionary<string, string> {
                {"name", "Tesla Model S"}, {"subTitle", "Plaid"}, {"category", "Electric"}, 
                {"priceDay", "13000"}, {"acc", "1.99s"}, {"topSpeed", "322 km/h"}, {"horsepower", "1020 HP"}, 
                {"mileage", "637 km (Range)"}, {"seats", "5"}, {"transmission", "Automatic"}, {"fuel", "Electric"}, 
                {"year", "2024"}, {"color", "White"},
                {"description", "The Tesla Model S Plaid is the world's quickest production car with a 0–100 km/h time of under 2 seconds."}
            },
            new Dictionary<string, string> {
                {"name", "Range Rover Velar"}, {"subTitle", "P400 R-Dynamic"}, {"category", "SUV"}, 
                {"priceDay", "15000"}, {"acc", "5.3s"}, {"topSpeed", "250 km/h"}, {"horsepower", "400 HP"}, 
                {"mileage", "11 km/l"}, {"seats", "5"}, {"transmission", "Automatic"}, {"fuel", "Petrol"}, 
                {"year", "2024"}, {"color", "Silver"},
                {"description", "The Range Rover Velar redefines the SUV segment with its breathtaking design, advanced technology and outstanding capability."}
            },
            new Dictionary<string, string> {
                {"name", "Porsche 911"}, {"subTitle", "GT3 RS"}, {"category", "Sports"}, 
                {"priceDay", "25000"}, {"acc", "3.0s"}, {"topSpeed", "296 km/h"}, {"horsepower", "518 HP"}, 
                {"mileage", "7 km/l"}, {"seats", "2"}, {"transmission", "PDK (Auto)"}, {"fuel", "Petrol"}, 
                {"year", "2024"}, {"color", "Red"},
                {"description", "The Porsche 911 GT3 RS is designed for maximum performance, featuring extreme aerodynamics and track-focused suspension."}
            }
        };

        [HttpGet("fetch")]
        public IActionResult FetchCarData([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Query cannot be empty");

            var match = _carDatabase.FirstOrDefault(c => 
                c["name"].ToLower().Contains(query.ToLower()) || 
                query.ToLower().Contains(c["name"].ToLower()));
                
            if (match == null) return NotFound();

            return Ok(match);
        }
    }
}
