using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LUXURY_DRIVE.Data;
using LUXURY_DRIVE.Models.Entities;

namespace LUXURY_DRIVE.Data
{
    public static class DbSeeder
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.Migrate();
                if (!context.Users.Any(u => u.Role == "Admin"))
                {
                    context.Users.Add(new User
                    {
                        FullName = "Admin",
                        Email = "admin@luxedrive.com",
                        Username = "admin@luxedrive.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        Role = "Admin"
                    });
                    context.SaveChanges();
                }

                if (!context.Vehicles.Any())
                {
                    context.Vehicles.AddRange(
                        new Vehicle
                        {
                            Name = "BMW M5",
                            SubTitle = "Competition · 2024 Model",
                            Category = "Luxury",
                            PriceDay = "10000",
                            ImageUrl = "https://images.unsplash.com/photo-1555215695-3004980ad54e?w=1200",
                            Acc = "3.5s",
                            TopSpeed = "305 km/h",
                            Horsepower = "600 HP",
                            Mileage = "12 km/l",
                            Description = "The BMW M5 Competition is the pinnacle of performance sedans, combining jaw-dropping speed with everyday refinement. Powered by a 4.4-litre V8 twin-turbo engine delivering 600hp, it blasts from 0–100 km/h in just 3.3 seconds while keeping you wrapped in supreme luxury."
                        },
                        new Vehicle
                        {
                            Name = "Audi R8",
                            SubTitle = "V10 Performance · 2024",
                            Category = "Sports",
                            PriceDay = "20000",
                            ImageUrl = "https://images.unsplash.com/photo-1603584173870-7f23fdae1b7a?w=1200",
                            Acc = "3.2s",
                            TopSpeed = "330 km/h",
                            Horsepower = "620 HP",
                            Mileage = "8 km/l",
                            Description = "The Audi R8 V10 Performance is a naturally aspirated supercar like no other. With its mid-mounted V10 engine screaming to 8,700 rpm and quattro all-wheel drive, it delivers a pure, visceral driving experience wrapped in stunning Italian-designed bodywork."
                        },
                        new Vehicle
                        {
                            Name = "Range Rover Velar",
                            SubTitle = "P400 R-Dynamic · 2024",
                            Category = "SUV",
                            PriceDay = "15000",
                            ImageUrl = "https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=1200",
                            Acc = "5.3s",
                            TopSpeed = "250 km/h",
                            Horsepower = "400 HP",
                            Mileage = "11 km/l",
                            Description = "The Range Rover Velar redefines the SUV segment with its breathtaking design, advanced technology and outstanding capability. The perfect blend of sporting performance and supreme luxury, it commands attention wherever it goes."
                        },
                        new Vehicle
                        {
                            Name = "Mercedes S-Class",
                            SubTitle = "S 500 4MATIC · 2024",
                            Category = "Luxury",
                            PriceDay = "12000",
                            ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=1200",
                            Acc = "4.3s",
                            TopSpeed = "250 km/h",
                            Horsepower = "435 HP",
                            Mileage = "13 km/l",
                            Description = "The Mercedes-Benz S-Class represents the very best in automotive luxury. Every journey becomes an indulgent experience with its massaging seats, perfume atomiser, cinema-quality rear screens and the world-first MBUX Hyperscreen stretching the full width of the dashboard."
                        },
                        new Vehicle
                        {
                            Name = "Tesla Model S",
                            SubTitle = "Plaid · 2024",
                            Category = "Electric",
                            PriceDay = "13000",
                            ImageUrl = "https://images.unsplash.com/photo-1561580125-028ee3bd62eb?w=1200",
                            Acc = "1.99s",
                            TopSpeed = "322 km/h",
                            Horsepower = "1020 HP",
                            Mileage = "637 km",
                            Description = "The Tesla Model S Plaid is the world's quickest production car with a 0–100 km/h time of under 2 seconds. Three independent motors deliver 1,020 hp and a range of 637 km on a single charge — redefining what an electric vehicle can be."
                        },
                        new Vehicle
                        {
                            Name = "Lamborghini Huracan",
                            SubTitle = "EVO · 2024",
                            Category = "Sports",
                            PriceDay = "30000",
                            ImageUrl = "https://images.unsplash.com/photo-1511919884226-fd3cad34687c?w=1200",
                            Acc = "2.9s",
                            TopSpeed = "325 km/h",
                            Horsepower = "640 HP",
                            Mileage = "6 km/l",
                            Description = "The Lamborghini Huracán EVO is a masterpiece of Italian engineering — a naturally-aspirated V10 howls to 8,000 rpm, producing 640 hp. The Lamborghini Dinamica Veicolo Integrata system links all major dynamic controls for an exhilarating, yet accessible supercar experience."
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
