using System.Diagnostics;
using LUXURY_DRIVE.Models;
using LUXURY_DRIVE.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                if (vehicle.Status == "Rented" || vehicle.Status == "Maintenance")
                {
                    TempData["ErrorMessage"] = $"Sorry, this vehicle is currently {vehicle.Status}.";
                    return RedirectToAction("Index");
                }
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
        public async Task<IActionResult> SubmitContact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ShowContactError = true;
                return View("Index", model);
            }

            var contact = new LUXURY_DRIVE.Models.Entities.Contact
            {
                Name = model.Name ?? "",
                Email = model.Email ?? "",
                Message = model.Message ?? "",
                SubmittedAt = DateTime.Now
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your message has been sent successfully!";

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadUserData()
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
            if (user == null) return NotFound();

            var bookings = await _context.CarRents
                .Include(r => r.Vehicle)
                .Where(r => r.Email.ToLower() == email.ToLower())
                .OrderByDescending(r => r.PickupDate)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial).FontColor("#333333"));

                    page.Header().Row(row =>
                    {
                        var shieldSvg = @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24"" fill=""none"" stroke=""#d4af37"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z""/></svg>";

                        row.ConstantItem(60).Height(60).Svg(shieldSvg);
                        
                        row.RelativeItem().PaddingLeft(15).Column(column =>
                        {
                            column.Item().Text("LUXE DRIVE").FontSize(24).SemiBold().FontColor("#212529");
                            column.Item().Text("Premium Rentals").FontSize(11).FontColor("#6c757d");
                        });
                        row.RelativeItem().AlignRight().Column(c => {
                            c.Item().AlignRight().Text("USER DATA REPORT").FontSize(16).SemiBold().FontColor("#212529");
                            c.Item().AlignRight().Text($"Generated: {DateTime.Now:MMMM d, yyyy}").FontSize(10).FontColor("#6c757d");
                        });
                    });

                    page.Content().PaddingVertical(1.5f, Unit.Centimetre).Column(column =>
                    {
                        // User Details Section
                        column.Item().Background("#f8f9fa").Padding(15).Row(row => 
                        {
                            row.RelativeItem().Column(c => {
                                c.Item().PaddingBottom(5).Text("Customer Profile").FontSize(14).SemiBold().FontColor("#212529");
                                c.Item().Text(user.FullName).FontSize(12).SemiBold().FontColor("#495057");
                            });
                            row.RelativeItem().AlignRight().Column(c => {
                                c.Item().AlignRight().Text($"Email: {user.Email}").FontSize(11).FontColor("#495057");
                                c.Item().AlignRight().Text($"Phone: {user.PhoneNumber ?? "Not Provided"}").FontSize(11).FontColor("#495057");
                            });
                        });
                        
                        column.Spacing(20);
                        column.Item().PaddingTop(15).PaddingBottom(5).Text("Booking History").FontSize(18).SemiBold().FontColor("#212529");
                        
                        // Bootstrap Styled Table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Vehicle
                                columns.RelativeColumn(2); // Date
                                columns.RelativeColumn(1); // Days
                                columns.RelativeColumn(2); // Amount
                                columns.RelativeColumn(2); // Status
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#e9ecef").BorderBottom(2).BorderColor("#dee2e6").Padding(8).Text("Vehicle").FontSize(11).SemiBold().FontColor("#495057");
                                header.Cell().Background("#e9ecef").BorderBottom(2).BorderColor("#dee2e6").Padding(8).Text("Pickup Date").FontSize(11).SemiBold().FontColor("#495057");
                                header.Cell().Background("#e9ecef").BorderBottom(2).BorderColor("#dee2e6").Padding(8).AlignRight().Text("Days").FontSize(11).SemiBold().FontColor("#495057");
                                header.Cell().Background("#e9ecef").BorderBottom(2).BorderColor("#dee2e6").Padding(8).AlignRight().Text("Amount").FontSize(11).SemiBold().FontColor("#495057");
                                header.Cell().Background("#e9ecef").BorderBottom(2).BorderColor("#dee2e6").Padding(8).AlignCenter().Text("Status").FontSize(11).SemiBold().FontColor("#495057");
                            });

                            decimal totalSpend = 0;
                            bool isAlternate = false;

                            foreach (var b in bookings)
                            {
                                var vName = b.Vehicle?.Name ?? "Unknown";
                                var amount = 0m;
                                if (b.Vehicle != null && decimal.TryParse(b.Vehicle.PriceDay?.Replace(",", "").Trim(), out var price))
                                {
                                    amount = price * b.NumberOfDays;
                                }
                                
                                var status = b.Status?.ToLower() ?? "";
                                if (status != "cancelled")
                                {
                                    totalSpend += amount;
                                }
                                
                                string statusBgColor = status == "completed" || status == "active" ? "#d1e7dd" : (status == "cancelled" ? "#f8d7da" : "#fff3cd");
                                string statusTextColor = status == "completed" || status == "active" ? "#0f5132" : (status == "cancelled" ? "#842029" : "#664d03");
                                string rowBgColor = isAlternate ? "#f8f9fa" : "#ffffff";

                                table.Cell().Background(rowBgColor).BorderBottom(1).BorderColor("#dee2e6").Padding(8).Text(vName).FontSize(11).FontColor("#212529").SemiBold();
                                table.Cell().Background(rowBgColor).BorderBottom(1).BorderColor("#dee2e6").Padding(8).Text(b.PickupDate.ToString("dd MMM yyyy")).FontSize(11);
                                table.Cell().Background(rowBgColor).BorderBottom(1).BorderColor("#dee2e6").Padding(8).AlignRight().Text(b.NumberOfDays.ToString()).FontSize(11);
                                table.Cell().Background(rowBgColor).BorderBottom(1).BorderColor("#dee2e6").Padding(8).AlignRight().Text($"INR {amount:N0}").FontSize(11).SemiBold();
                                table.Cell().Background(rowBgColor).BorderBottom(1).BorderColor("#dee2e6").Padding(8).AlignCenter().Background(statusBgColor).PaddingHorizontal(8).PaddingVertical(3).Text(b.Status?.ToUpper() ?? "PENDING").FontSize(9).SemiBold().FontColor(statusTextColor);
                                
                                isAlternate = !isAlternate;
                            }
                            
                            // Total Row
                            table.Cell().ColumnSpan(3).BorderTop(2).BorderColor("#212529").Padding(12).AlignRight().Text("Total Lifetime Value:").FontSize(14).SemiBold().FontColor("#212529");
                            table.Cell().ColumnSpan(2).BorderTop(2).BorderColor("#212529").Padding(12).AlignRight().Text($"INR {totalSpend:N0}").FontSize(16).SemiBold().FontColor("#c9a84c");
                        });
                    });

                    page.Footer().Row(row => 
                    {
                        row.RelativeItem().Text("Luxe Drive Premium Rentals | www.luxedrive.com").FontSize(9).FontColor("#adb5bd");
                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Page ").FontColor("#adb5bd").FontSize(9);
                            x.CurrentPageNumber().FontColor("#adb5bd").FontSize(9);
                            x.Span(" of ").FontColor("#adb5bd").FontSize(9);
                            x.TotalPages().FontColor("#adb5bd").FontSize(9);
                        });
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"UserData_LuxeDrive_{user.FullName.Replace(" ", "_")}.pdf");
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