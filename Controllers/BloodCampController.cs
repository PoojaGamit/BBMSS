
using System.Text;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BBMS1MVC.Controllers
{
    public class BloodCampController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public BloodCampController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetString("Userid");
            if (id != null)
            {
                var client = clientFactory.CreateClient("MyApiClient");
                var response = await client.GetAsync($"BloodCamps/GetcampByBloodbankId/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var bloodstock = JsonConvert.DeserializeObject<List<BloodCamps>>(jsonData);
                    return View(bloodstock);
                }

            }
            return View(new List<BloodCamps>());

        }

        [HttpGet]
        public IActionResult AddBCamp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBCamp([FromForm] BloodCamps model)
        {

            var id = HttpContext.Session.GetString("Userid");
            if (id != null)
            {
                model.CreatedBy = Convert.ToInt32(id);
                var client = clientFactory.CreateClient("MyApiClient");
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("BloodCamps/AddCamp", jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error while adding Blood Bank.");
                }
            }




            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> EditBCamp(int id)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync($"BloodCamps/GetCampById/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodstock = JsonConvert.DeserializeObject<BloodCamps>(jsonData);
                return View(bloodstock);
            }
            return View(new Notification());
        }

        [HttpPost]
        public async Task<IActionResult> EditBCamp([FromForm] BloodCamps model)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response=await client.PutAsync("BloodCamps/UpdateCamp", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error while adding Blood Bank.");
            }
            return View(model);
        }
    }
}
