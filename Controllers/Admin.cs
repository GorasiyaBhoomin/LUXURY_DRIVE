using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LUXURY_DRIVE.Data;
using LUXURY_DRIVE.Models.Entities;
using LUXURY_DRIVE.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System;

namespace LUXURY_DRIVE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalUsers = _context.Users.Count(u => u.Role == "User");
            ViewBag.TotalVehicles = _context.Vehicles.Count();
            ViewBag.TotalRents = _context.CarRents.Count();

            // Calculate total revenue from bookings
            var rents = _context.CarRents.Include(r => r.Vehicle).ToList();
            decimal totalRevenue = 0;
            foreach (var r in rents)
            {
                if (r.Vehicle != null && decimal.TryParse(r.Vehicle.PriceDay.Replace(",", "").Trim(), out var price))
                    totalRevenue += price * r.NumberOfDays;
            }
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.RecentBookings = rents.OrderByDescending(r => r.PickupDate).Take(5).ToList();

            return View();
        }

        public async Task<IActionResult> Adminbookings()
        {
            var bookings = await _context.CarRents.Include(c => c.Vehicle).Include(c => c.User).ToListAsync();
            ViewBag.Bookings = bookings;
            ViewBag.Vehicles = await _context.Vehicles.ToListAsync();
            ViewBag.Users = await _context.Users.Where(u => u.Role == "User").ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBooking(CarRentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var rent = new CarRent
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    LicenseNumber = model.LicenseNumber,
                    LicenseExpiryDate = model.LicenseExpiryDate ?? DateTime.Now,
                    PickupDate = model.PickupDate ?? DateTime.Now,
                    PickupTime = model.PickupTime,
                    NumberOfDays = model.NumberOfDays ?? 1,
                    VehicleId = model.VehicleId,
                    UserId = model.UserId,
                    Status = model.Status ?? "Pending"
                };
                
                _context.CarRents.Add(rent);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Adminbookings");
            }
            ViewBag.OpenAddForm = true;
            ViewBag.Bookings = await _context.CarRents.Include(c => c.Vehicle).Include(c => c.User).ToListAsync();
            ViewBag.Vehicles = await _context.Vehicles.ToListAsync();
            ViewBag.Users = await _context.Users.Where(u => u.Role == "User").ToListAsync();
            return View("Adminbookings", model);
        }

        [HttpPost]
        public async Task<IActionResult> SendBookingEmail(int id)
        {
            var rent = await _context.CarRents
                .Include(r => r.Vehicle)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RentId == id);
                
            if (rent == null)
            {
                TempData["EmailStatus"] = "Error: Booking not found.";
                return RedirectToAction("Adminbookings");
            }

            try
            {
                var customerName = rent.User?.FullName ?? rent.FirstName + " " + rent.LastName;
                var customerEmail = rent.User?.Email ?? rent.Email;
                var vehicleName = rent.Vehicle?.Name ?? "Unknown Vehicle";
                var vehicleImage = rent.Vehicle?.ImageUrl ?? "https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=600&q=70&fit=crop";
                
                var priceStr = rent.Vehicle?.PriceDay?.Replace(",", "").Trim();
                string amountText = "TBD";
                if (decimal.TryParse(priceStr, out var priceValue))
                {
                    amountText = "₹" + (priceValue * (rent.NumberOfDays > 0 ? rent.NumberOfDays : 1)).ToString("N0");
                }
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("gorasiyabhoomin@gmail.com", "Luxe Drive"),
                    Subject = $"Booking Confirmation - Luxe Drive ({vehicleName})",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(customerEmail);

                string body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; background-color: #f7f9fa; color: #1a1a1a; text-align: center; padding: 40px;'>
                        <div style='background-color: #ffffff; max-width: 600px; margin: 0 auto; border-radius: 12px; padding: 40px 30px; box-shadow: 0 4px 20px rgba(0,0,0,0.05);'>
                            <h1 style='color: #c9a84c; border-bottom: 2px solid #f0f0f0; padding-bottom: 20px; font-weight: 800; letter-spacing: 2px;'>LUXE DRIVE</h1>
                            <p style='font-size: 18px; margin-top: 30px;'>Hello <strong>{customerName}</strong>,</p>
                            <p style='font-size: 15px; color: #555; line-height: 1.6;'>We are delighted to confirm your booking for the <strong>{vehicleName}</strong>. Below are your reservation details.</p>
                            
                            <img src='{vehicleImage}' alt='{vehicleName}' style='max-width: 100%; height: auto; border-radius: 8px; margin: 25px 0; border: 1px solid #eaeaea;' />
                            
                            <div style='background-color: #f8fafc; padding: 25px; border-radius: 12px; display: inline-block; text-align: left; width: 100%; box-sizing: border-box; border: 1px solid #e2e8f0;'>
                                <ul style='list-style: none; padding: 0; margin: 0; color: #334155; font-size: 15px;'>
                                    <li style='margin-bottom: 12px; display: flex; justify-content: space-between;'><strong>Booking ID:</strong> <span style='font-weight: 600; color: #0f172a;'>#{rent.RentId}</span></li>
                                    <li style='margin-bottom: 12px; display: flex; justify-content: space-between;'><strong>License Plate:</strong> <span style='font-weight: 600; color: #0f172a;'>{rent.LicenseNumber}</span></li>
                                    <li style='margin-bottom: 12px; display: flex; justify-content: space-between;'><strong>Pickup:</strong> <span style='font-weight: 600; color: #0f172a;'>{rent.PickupDate:dd MMM yyyy} at {rent.PickupTime}</span></li>
                                    <li style='margin-bottom: 12px; display: flex; justify-content: space-between;'><strong>Duration:</strong> <span style='font-weight: 600; color: #0f172a;'>{rent.NumberOfDays} days</span></li>
                                    <li style='margin-bottom: 12px; display: flex; justify-content: space-between;'><strong>Total Amount:</strong> <span style='font-weight: 700; color: #0f172a; font-size: 16px;'>{amountText}</span></li>
                                    <li style='display: flex; justify-content: space-between; border-top: 1px solid #cbd5e1; padding-top: 12px; margin-top: 12px;'><strong>Status:</strong> <span style='font-weight: 700; color: #c9a84c;'>{(rent.Status ?? "Pending").ToUpper()}</span></li>
                                </ul>
                            </div>
                            
                            <p style='margin-top: 40px; font-size: 14px; color: #888;'>Thank you for choosing Luxe Drive. Safe travels!</p>
                        </div>
                    </body>
                    </html>";

                mailMessage.Body = body;

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential("gorasiyabhoomin@gmail.com", "vnfw gqut zypr izhs");
                    await smtpClient.SendMailAsync(mailMessage);
                }

                TempData["EmailStatus"] = "Success: Booking details sent successfully to " + customerEmail;
            }
            catch (Exception ex)
            {
                TempData["EmailStatus"] = "Error: Failed to send email. " + ex.Message;
            }

            return RedirectToAction("Adminbookings");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var rent = await _context.CarRents.FindAsync(id);
            if (rent != null)
            {
                _context.CarRents.Remove(rent);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Adminbookings");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, string status)
        {
            var rent = await _context.CarRents.FindAsync(id);
            if (rent != null && !string.IsNullOrEmpty(status))
            {
                rent.Status = status;
                _context.CarRents.Update(rent);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Booking not found or status invalid." });
        }

        public async Task<IActionResult> AdminVehicles()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            // Passing entity list to view via ViewBag for demonstration
            ViewBag.VehiclesList = vehicles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(VehicleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vehicle = new Vehicle
                {
                    Name = model.Name,
                    SubTitle = model.SubTitle,
                    Category = model.Category,
                    PriceDay = model.PriceDay,
                    ImageUrl = model.ImageUrl,
                    Acc = model.Acc,
                    TopSpeed = model.TopSpeed,
                    Horsepower = model.Horsepower,
                    Mileage = model.Mileage,
                    Description = model.Description,
                    Seats = model.Seats,
                    Transmission = model.Transmission,
                    Fuel = model.Fuel,
                    Year = model.Year,
                    Color = model.Color,
                    KeyFeatures = model.KeyFeatures
                };
                
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("AdminVehicles");
            }
            ViewBag.OpenAddForm = true;
            return View("AdminVehicles", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(VehicleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Name = model.Name;
                    vehicle.SubTitle = model.SubTitle;
                    vehicle.Category = model.Category;
                    vehicle.PriceDay = model.PriceDay;
                    vehicle.ImageUrl = model.ImageUrl;
                    vehicle.Acc = model.Acc;
                    vehicle.TopSpeed = model.TopSpeed;
                    vehicle.Horsepower = model.Horsepower;
                    vehicle.Mileage = model.Mileage;
                    vehicle.Description = model.Description;
                    vehicle.Seats = model.Seats;
                    vehicle.Transmission = model.Transmission;
                    vehicle.Fuel = model.Fuel;
                    vehicle.Year = model.Year;
                    vehicle.Color = model.Color;
                    vehicle.KeyFeatures = model.KeyFeatures;
                    
                    _context.Vehicles.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("AdminVehicles");
            }
            ViewBag.OpenEditModal = true;
            ViewBag.VehiclesList = await _context.Vehicles.ToListAsync();
            return View("AdminVehicles", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminVehicles");
        }

        public async Task<IActionResult> AdminCustomers()
        {
            var customers = await _context.Users.Where(u => u.Role == "User").ToListAsync();
            ViewBag.Customers = customers;
            ViewBag.TotalRents = await _context.CarRents.CountAsync();
            var rents = await _context.CarRents.Include(r => r.Vehicle).ToListAsync();
            
            decimal totalRevenue = 0;
            var bookingsCount = new System.Collections.Generic.Dictionary<string, int>();
            var spentAmount = new System.Collections.Generic.Dictionary<string, decimal>();
            var uniqueVehicles = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<int>>();
            
            foreach (var r in rents)
            {
                if (r.Vehicle != null && decimal.TryParse(r.Vehicle.PriceDay.Replace(",", "").Trim(), out var price))
                {
                    decimal bookingRevenue = price * r.NumberOfDays;
                    totalRevenue += bookingRevenue;
                    
                    if (!string.IsNullOrEmpty(r.Email))
                    {
                        var email = r.Email.ToLower().Trim();
                        if (!bookingsCount.ContainsKey(email)) bookingsCount[email] = 0;
                        if (!spentAmount.ContainsKey(email)) spentAmount[email] = 0m;
                        if (!uniqueVehicles.ContainsKey(email)) uniqueVehicles[email] = new System.Collections.Generic.HashSet<int>();
                        
                        bookingsCount[email]++;
                        spentAmount[email] += bookingRevenue;
                        if (r.VehicleId.HasValue) uniqueVehicles[email].Add(r.VehicleId.Value);
                    }
                }
            }
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.BookingsCount = bookingsCount;
            ViewBag.SpentAmount = spentAmount;
            
            var vehiclesCount = new System.Collections.Generic.Dictionary<string, int>();
            foreach (var kvp in uniqueVehicles) vehiclesCount[kvp.Key] = kvp.Value.Count;
            ViewBag.VehiclesCount = vehiclesCount;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Username = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = !string.IsNullOrEmpty(model.Password) 
                        ? BCrypt.Net.BCrypt.HashPassword(model.Password) 
                        : BCrypt.Net.BCrypt.HashPassword("Default@123"), // default password
                    Role = "User"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminCustomers");
            }
            ViewBag.OpenAddForm = true;
            ViewBag.Customers = await _context.Users.Where(u => u.Role == "User").ToListAsync();
            return View("AdminCustomers", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(model.UserId);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    }
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("AdminCustomers");
            }
            ViewBag.OpenEditModal = true;
            ViewBag.Customers = await _context.Users.Where(u => u.Role == "User").ToListAsync();
            return View("AdminCustomers", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null && user.Role == "User")
            {
                // Optionally remove related bookings if needed
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminCustomers");
        }

        public async Task<IActionResult> AdminContacts()
        {
            var contacts = await _context.Contacts.OrderByDescending(c => c.SubmittedAt).ToListAsync();
            ViewBag.Contacts = contacts;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminContacts");
        }

        public async Task<IActionResult> AdminProfile()
        {
            var email = User.Identity?.Name;
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role == "Admin");
            var vm = new AdminProfilePageViewModel();
            if (admin != null)
            {
                vm.Profile = new AdminProfileViewModel
                {
                    FullName = admin.FullName,
                    Email = admin.Email,
                    PhoneNumber = admin.PhoneNumber
                };
                ViewBag.TotalBookings = _context.CarRents.Count();
                ViewBag.TotalCustomers = _context.Users.Count(u => u.Role == "User");
                ViewBag.TotalVehicles = _context.Vehicles.Count();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AdminProfileViewModel profile)
        {
            if (ModelState.IsValid)
            {
                var email = User.Identity?.Name;
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role == "Admin");
                if (admin != null)
                {
                    admin.FullName = profile.FullName;
                    admin.Email = profile.Email;
                    admin.PhoneNumber = profile.PhoneNumber ?? admin.PhoneNumber;
                    _context.Users.Update(admin);
                    await _context.SaveChangesAsync();
                }
                TempData["ProfileSuccess"] = "Profile updated successfully.";
                return RedirectToAction("AdminProfile");
            }
            return View("AdminProfile", new AdminProfilePageViewModel { Profile = profile });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(AdminChangePasswordViewModel password)
        {
            if (ModelState.IsValid)
            {
                var email = User.Identity?.Name;
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role == "Admin");
                if (admin != null)
                {
                    if (!BCrypt.Net.BCrypt.Verify(password.CurrentPassword, admin.PasswordHash))
                    {
                        ModelState.AddModelError("Password.CurrentPassword", "Current password is incorrect.");
                        return View("AdminProfile", new AdminProfilePageViewModel { Password = password });
                    }
                    if (password.NewPassword != password.ConfirmPassword)
                    {
                        ModelState.AddModelError("Password.ConfirmPassword", "Passwords do not match.");
                        return View("AdminProfile", new AdminProfilePageViewModel { Password = password });
                    }
                    admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password.NewPassword);
                    _context.Users.Update(admin);
                    await _context.SaveChangesAsync();
                }
                TempData["PasswordSuccess"] = "Password updated successfully.";
                return RedirectToAction("AdminProfile");
            }
            return View("AdminProfile", new AdminProfilePageViewModel { Password = password });
        }
    }
}
