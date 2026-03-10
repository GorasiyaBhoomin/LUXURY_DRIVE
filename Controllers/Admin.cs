using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LUXURY_DRIVE.Controllers
{
    public class Admin : Controller
    {
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

       
    }
}
