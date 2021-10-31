using Microsoft.AspNetCore.Mvc;

namespace MonamourWeb.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult AccessForbidden()
        {
            return View();
        }
    }
}
