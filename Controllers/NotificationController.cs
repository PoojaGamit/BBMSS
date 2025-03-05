using System.Text;
using BBMS1MVC.Helper;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BBMS1MVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public NotificationController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var token = HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                var adminId = JwtHelper.GetUserId(token);

                if (!string.IsNullOrEmpty(adminId))
                {
                    var client = clientFactory.CreateClient("MyApiClient");
                    var response = await client.GetAsync($"Notification/GetNotibyAdmin/{adminId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var bloodstock = JsonConvert.DeserializeObject<List<Notification>>(jsonData);
                        return View(bloodstock);
                    }
                }
            }
            return View(new List<Notification>());

        }

        [HttpGet]
        public IActionResult AddNoti()
        { 
          return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNoti([FromForm]Notification model)
        {

            if (ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (token != null)
                {
                    var adminId = JwtHelper.GetUserId(token);

                    if (!string.IsNullOrEmpty(adminId))
                    {
                        var client = clientFactory.CreateClient("MyApiClient");
                        var response = await client.GetAsync($"Notification/BloodBankidfromadmin/{adminId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var bloodBankId = await response.Content.ReadAsStringAsync();
                            model.BloodBankId = Convert.ToInt32(bloodBankId);
                          
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Error fetching BloodBank ID.");
                            return View(model);
                        }
                        var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        var response1 = await client.PostAsync("Notification/AddNoti", jsonContent);

                        if (response1.IsSuccessStatusCode)
                        {
                             return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Error while adding Blood Bank.");
                        }
                    }
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateNotifi(int id)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync($"Notification/FindByNotificationid/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var notification = JsonConvert.DeserializeObject<Notification>(jsonData);
                return View(notification);
            }

            return View(new Notification());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNotifi([FromForm] Notification model)
        {
            var client = clientFactory.CreateClient("MyApiClient");

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("Notification/Updatenotification", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error updating Notification: {errorContent}");
            }

            return View(model);
        }
       
    }
}
