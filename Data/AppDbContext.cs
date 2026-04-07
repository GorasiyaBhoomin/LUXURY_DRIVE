using Microsoft.EntityFrameworkCore;
using LUXURY_DRIVE.Models.Entities;

namespace LUXURY_DRIVE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<CarRent> CarRents { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
