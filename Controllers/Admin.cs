using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LUXURY_DRIVE.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Adminbookings()
        {
            return View();
        }

        public IActionResult AdminVehicles()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddVehicle(Models.VehicleViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("AdminVehicles");
            }
            ViewBag.OpenAddForm = true;
            return View("AdminVehicles", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVehicle(Models.VehicleViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("AdminVehicles");
            }
            ViewBag.OpenEditModal = true;
            return View("AdminVehicles", model);
        }

        public IActionResult AdminCustomers()
        {
            return View();
        }

        public IActionResult AdminProfile()
        {
            return View(new Models.AdminProfilePageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(Models.AdminProfileViewModel profile)
        {
            if (ModelState.IsValid)
                return RedirectToAction("AdminProfile");
            return View("AdminProfile", new Models.AdminProfilePageViewModel { Profile = profile });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(Models.AdminChangePasswordViewModel password)
        {
            if (ModelState.IsValid)
                return RedirectToAction("AdminProfile");
            return View("AdminProfile", new Models.AdminProfilePageViewModel { Password = password });
        }
    }
}
