using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LUXURY_DRIVE.Data;

namespace LUXURY_DRIVE.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AutoDataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> FetchCarData([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Query cannot be empty");

            // Fetch dynamically from your actual database
            var match = await _context.Vehicles.FirstOrDefaultAsync(c => 
                c.Name.ToLower().Contains(query.ToLower()));
                
            if (match == null) return NotFound();

            return Ok(new {
                name = match.Name,
                subTitle = match.SubTitle,
                category = match.Category,
                priceDay = match.PriceDay,
                acc = match.Acc,
                topSpeed = match.TopSpeed,
                horsepower = match.Horsepower,
                mileage = match.Mileage,
                seats = match.Seats,
                transmission = match.Transmission,
                fuel = match.Fuel,
                year = match.Year,
                color = match.Color,
                description = match.Description,
                imageUrl = match.ImageUrl
            });
        }
    }
}
