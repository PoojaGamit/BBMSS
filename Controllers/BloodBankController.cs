using BBMSDATA1.Context;
using BBMSDATA1.Models;
using BBMS1MVC.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using NuGet.Common;


namespace BBMS1MVC.Controllers
{
    public class BloodBankController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly MYDBContext context;

        public BloodBankController(IHttpClientFactory clientFactory, MYDBContext context)
        {
            this.clientFactory = clientFactory;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWTToken");
           
            if (token != null)
            {
                ViewBag.UserName = JwtHelper.GetUserName(token);
                ViewBag.UserRole = JwtHelper.GetUserRole(token);
                var ab = JwtHelper.GetUserId(token);
                var client = clientFactory.CreateClient("MyApiClient");
                var response = await client.GetAsync($"BloodBank/UnitsAvailable/{ab}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    ViewBag.Units = await context.BloodStock.Where(a => a.BloodBankId == Convert.ToInt32(json)).SumAsync(a => a.Quantity);
                    ViewBag.Bloodtypes = await context.BloodStock.Where(a => a.BloodBankId == Convert.ToInt32(json)).Select(a => a.BloodGroupId).Distinct().CountAsync();
                    ViewBag.Donor = await context.Donations.Where(a => a.BloodBankId == Convert.ToInt32(json)).Select(a => a.DonorId).Distinct().CountAsync();
                    ViewBag.Requests = await context.BloodRequest.Where(a => a.BloodBankId == Convert.ToInt32(json)).Select(a => a.RequestId).Distinct().CountAsync();

                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBloodbank()
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync("BloodBank/GETBloodBanks");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodBanks = JsonConvert.DeserializeObject<IEnumerable<BloodBanks>>(jsonData);

                return View(bloodBanks);
            }
            else
            {
                return View(new List<BloodBanks>());
            }
        }

        [HttpGet]
        public IActionResult BBDetails()
        {
           ViewBag.States = new SelectList(context.States.ToList(), "Id", "StateName");
           return View();
                   
        }

        [HttpPost]
        public async Task<IActionResult> BBDetails([FromForm] BloodBanks model)
        {

            if (ModelState.IsValid)
            {
                var id = HttpContext.Session.GetString("Userid");
                if (id != null)
                {
                    model.AdminId = Convert.ToInt32(id);
                }
                var client = clientFactory.CreateClient("MyApiClient");
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("BloodBank/AddBloodBanks", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetBBbyAdminid");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error while adding Blood Bank.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBBbyAdminid()
        {
            var id = HttpContext.Session.GetString("Userid");
            if (id != null)
            {
                var bid=await context.BloodBanks.Where(a=>a.AdminId==Convert.ToInt32(id)).Select(a=>a.BloodBankId).FirstOrDefaultAsync();
                if(bid==0)
                {
                    return RedirectToAction("BBDetails");
                }
                else {

                    var client = clientFactory.CreateClient("MyApiClient");
                    var response = await client.GetAsync($"BloodBank/GetBloodBankbyadminid/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var bloodBank = JsonConvert.DeserializeObject<BloodBanks>(jsonData);
                        return View(bloodBank);
                    }
                }
              
            }

            return View(new List<BloodBanks>());
        }

        [HttpGet]
        public async Task<IActionResult> Updatebb()
        {
            var id = HttpContext.Session.GetString("Userid");
            if (!string.IsNullOrEmpty(id))
            {
                var client = clientFactory.CreateClient("MyApiClient");
                var response = await client.GetAsync($"BloodBank/GetBloodBankbyadminid/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var bloodBank = JsonConvert.DeserializeObject<BloodBanks>(jsonData);
                    return View(bloodBank);
                }

            }

            return View(new BloodBanks());
        }

        [HttpPost]
        public async Task<IActionResult> Updatebb([FromForm] BloodBanks model)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("BloodBank/Update", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetBBbyAdminid");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error updating Blood Bank.");
            }
            return View(model);
        }

        //[HttpGet]
       

    }
}
