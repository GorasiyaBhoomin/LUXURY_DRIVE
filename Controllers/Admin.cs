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

        public IActionResult AdminCustomers()
        {
            return View();
        }
    }
}