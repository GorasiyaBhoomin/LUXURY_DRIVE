using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LUXURY_DRIVE.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: AuthenticationController
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // In a real app, verify credentials here
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Models.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // In a real app, save user here
                return RedirectToAction("Login", "Authentication");
            }
            return View(model);
        }


    }
}
