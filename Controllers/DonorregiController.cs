using BBMSDATA1.Context;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BBMS1MVC.Controllers
{
    public class DonorregiController : Controller
    {
        private readonly MYDBContext context;

        public DonorregiController(MYDBContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            //if (HttpContext.Session.GetString("Userid") == null)
            //{
            //    return RedirectToAction("Login", "User");
            //}

            // Populate blood groups from database
            ViewBag.BloodGroups = context.BloodGroups.Select(bg => new SelectListItem
            {
                Value = bg.BloodGroupId.ToString(),
                Text = bg.BloodGroupName
            }).ToList();

            // Populate states from database
            ViewBag.States = context.States.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.StateName
            }).ToList();

            // Districts, cities, and blood banks will be populated via AJAX based on selected state/district

            return View();
        }
        [HttpPost]
        public IActionResult Index([FromForm]Donorregi donor)
        {
            if (ModelState.IsValid)
            {
                context.Donorregi.Add(donor);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(donor);
        }
    }
}
