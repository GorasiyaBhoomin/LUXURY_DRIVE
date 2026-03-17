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
                // In a real database, add the vehicle here
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
                // In a real database, update the vehicle here
                return RedirectToAction("AdminVehicles");
            }
            ViewBag.OpenEditModal = true;
            return View("AdminVehicles", model);
        }
        //public IActionResult ViewAdminVehicles()
        //{
        //    return View();
        //}
        //public IActionResult AddAdminVehicles()
        //{
        //    return View();
        //}
        //public IActionResult EditAdminVehicles()
        //{
        //    return View();
        //}

        public IActionResult AdminCustomers()
        {
            return View();
        }


        public IActionResult AdminProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(Models.AdminProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to update profile in database
                return RedirectToAction("AdminProfile");
            }
            // If invalid, re-render the profile page
            return View("AdminProfile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(Models.AdminChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to update password in database
                return RedirectToAction("AdminProfile");
            }
            // If invalid, re-render the profile page
            return View("AdminProfile", model);
        }
    }
}
