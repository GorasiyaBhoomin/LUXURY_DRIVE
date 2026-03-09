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
        public ActionResult Register()
        {
            return View();
        }


    }
}
