using System.Diagnostics;
using LUXURY_DRIVE.Models;
using LUXURY_DRIVE.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LUXURY_DRIVE.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // ── CHANGED: accepts optional ContactViewModel so validation errors
        //             survive the round-trip when SubmitContact returns View("Index", model)
        public IActionResult Index(ContactViewModel model = null)
        {
            ViewBag.Vehicles = _context.Vehicles.ToList();
            return View(model ?? new ContactViewModel());
        }

        public IActionResult Car_view()
        {
            ViewBag.Vehicles = _context.Vehicles.ToList();
            return View();
        }

        public async Task<IActionResult> Car_rent(int id)
        {
            var model = new CarRentViewModel();
            model.VehicleId = id;

            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle != null)
            {
                ViewBag.Vehicle = vehicle;
            }

            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity?.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user != null)
                    {
                        model.UserId = user.UserId;
                        model.Email = user.Email;
                        model.Phone = user.PhoneNumber;
                        
                        // Split FullName into FirstName and LastName
                        if (!string.IsNullOrEmpty(user.FullName))
                        {
                            var names = user.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (names.Length > 0)
                            {
                                model.FirstName = names[0];
                            }
                            if (names.Length > 1)
                            {
                                model.LastName = string.Join(" ", names.Skip(1));
                            }
                        }
                    }
                }
            }

            return View(model);
        }

        public IActionResult Debug()
        {
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.Name = User.Identity?.Name;
            ViewBag.Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            var viewModel = new UserProfilePageViewModel
            {
                Profile = new UserProfileViewModel
                {
                    FullName = user.FullName,
                    Username = user.Username ?? user.Email,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber ?? ""
                }
            };

            ViewBag.UserBookings = await _context.CarRents
                .Include(r => r.Vehicle)
                .Where(r => r.Email.ToLower() == email.ToLower())
                .OrderByDescending(r => r.PickupDate)
                .ToListAsync();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = User.Identity?.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Username = model.Username;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Profile");
            }
            ViewBag.ActiveTab = "personal";
            var viewModel = new UserProfilePageViewModel 
            { 
                Profile = model,
                Password = new UserChangePasswordViewModel()
            };
            var userEmail = User.Identity?.Name ?? "";
            ViewBag.UserBookings = await _context.CarRents
                .Include(r => r.Vehicle)
                .Where(r => r.Email.ToLower() == userEmail.ToLower())
                .OrderByDescending(r => r.PickupDate)
                .ToListAsync();

            return View("Profile", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UserChangePasswordViewModel model)
        {
            var email = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    }
                    else
                    {
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Profile");
                    }
                }
            }
            
            // If we get here, there were validation errors
            ViewBag.ActiveTab = "password";
            var viewModel = new UserProfilePageViewModel 
            { 
                Profile = new UserProfileViewModel
                {
                    FullName = user?.FullName ?? "",
                    Username = user?.Username ?? "",
                    Email = user?.Email ?? "",
                    PhoneNumber = user?.PhoneNumber ?? ""
                },
                Password = model
            };
            var safeEmailForQuery = (user?.Email ?? "").ToLower();
            ViewBag.UserBookings = await _context.CarRents
                .Include(r => r.Vehicle)
                .Where(r => r.Email.ToLower() == safeEmailForQuery)
                .OrderByDescending(r => r.PickupDate)
                .ToListAsync();

            return View("Profile", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRent(CarRentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (model.VehicleId.HasValue)
                {
                    ViewBag.Vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == model.VehicleId.Value);
                }
                return View("Car_rent", model);
            }
            
            var carRent = new LUXURY_DRIVE.Models.Entities.CarRent
            {
                VehicleId = model.VehicleId,
                UserId = model.UserId,
                FirstName = model.FirstName ?? "",
                LastName = model.LastName ?? "",
                Email = model.Email ?? "",
                Phone = model.Phone ?? "",
                LicenseNumber = model.LicenseNumber ?? "",
                LicenseExpiryDate = model.LicenseExpiryDate ?? DateTime.Now,
                PickupDate = model.PickupDate ?? DateTime.Now,
                PickupTime = model.PickupTime ?? "",
                NumberOfDays = model.NumberOfDays ?? 1,
                Status = model.Status ?? "Pending"
            };

            _context.CarRents.Add(carRent);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your booking request has been submitted successfully!";
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitContact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ShowContactError = true;
                return View("Index", model);
            }
            // TODO: Send email / save to db
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}