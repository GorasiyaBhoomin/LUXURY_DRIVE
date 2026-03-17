using System.Diagnostics;
using LUXURY_DRIVE.Models;
using Microsoft.AspNetCore.Mvc;

namespace LUXURY_DRIVE.Controllers
{
    public class HomeController : Controller
    {
        // ── CHANGED: accepts optional ContactViewModel so validation errors
        //             survive the round-trip when SubmitContact returns View("Index", model)
        public IActionResult Index(ContactViewModel model = null)
        {
            return View(model ?? new ContactViewModel());
        }

        public IActionResult Car_view()
        {
            return View();
        }

        public IActionResult Car_rent()
        {
            return View(new CarRentViewModel());
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Profile");
            }
            ViewBag.ActiveTab = "personal";
            return View("Profile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(UserChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Profile");
            }
            ViewBag.ActiveTab = "password";
            return View("Profile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRent(CarRentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Car_rent", model);
            }
            // TODO: Save booking to database here
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