
using BBMSDATA1.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BBMS1MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly MYDBContext context;

        public HomeController(MYDBContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            var totalBloodUnits = await context.BloodStock.SumAsync(b => b.Quantity); 
            ViewBag.TotalBloodUnits = totalBloodUnits;
            var donor = await context.Donations.Select(b=>b.DonorId).Distinct().CountAsync();
            ViewBag.DonorId = donor;
            return View();
        }

        public IActionResult Aboutus()
        {
          return View();
        }

    }
}
