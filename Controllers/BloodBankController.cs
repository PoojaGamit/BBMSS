using BBMSDATA1.Context;
using BBMSDATA1.Models;
using BBMS1MVC.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                ViewBag.UserName = JwtHelper.GetUserName(token);
                ViewBag.UserRole = JwtHelper.GetUserRole(token);
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
                var token = HttpContext.Session.GetString("JWTToken");
                if(token!=null)
                {
                    var adminId = JwtHelper.GetUserId(token);

                    if (!string.IsNullOrEmpty(adminId))
                    {
                        model.AdminId = Convert.ToInt32(adminId);
                    }
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
            var token = HttpContext.Session.GetString("JWTToken");
            if (token != null) 
            {
                var adminId = JwtHelper.GetUserId(token);

                if (!string.IsNullOrEmpty(adminId))
                {
                    var client = clientFactory.CreateClient("MyApiClient");
                    var response = await client.GetAsync($"BloodBank/GetBloodBankbyadminid/{adminId}");

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
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token)) 
            {

                var adminId = JwtHelper.GetUserId(token);

                if (!string.IsNullOrEmpty(adminId))
                {
                    var client = clientFactory.CreateClient("MyApiClient");
                    var response = await client.GetAsync($"BloodBank/GetBloodBankbyadminid/{adminId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var bloodBank = JsonConvert.DeserializeObject<BloodBanks>(jsonData);
                        return View(bloodBank);
                    }
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

    }
}
