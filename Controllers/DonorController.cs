using Microsoft.AspNetCore.Mvc;

namespace BBMS1MVC.Controllers
{
    public class DonorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
