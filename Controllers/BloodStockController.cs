using System.Text;
using BBMSDATA1.Context;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BBMS1MVC.Controllers
{
    public class BloodStockController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly MYDBContext context;

        public BloodStockController(IHttpClientFactory clientFactory, MYDBContext context)
        {
            this.clientFactory = clientFactory;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetByBloodBankId()
        {
            var id = HttpContext.Session.GetString("Userid");
            if (id != null)
            {
                var client = clientFactory.CreateClient("MyApiClient");
                var response = await client.GetAsync($"BloodStock/GetByBloodBankId/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var bloodstock = JsonConvert.DeserializeObject<List<BloodStock>>(jsonData);
                    return View(bloodstock);
                }
            }
            return View(new List<BloodStock>());

        }

        [HttpGet]
        public IActionResult AddBloodstock()
        {
            ViewBag.BloodGroups = new SelectList(context.BloodGroups.ToList(), "BloodGroupId", "BloodGroupName");
            ViewBag.Component = new SelectList(context.ComponentTypes.ToList(), "ComponentTypeId", "ComponentTypeName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBloodstock([FromForm] BloodStock model)
        {
            if (ModelState.IsValid)
            {
                var id = HttpContext.Session.GetString("Userid");
                if (id != null)
                {
                    model.UpdatedBy = Convert.ToInt32(id);
                    var client = clientFactory.CreateClient("MyApiClient");
                    var response = await client.GetAsync($"BloodStock/FindBloodBankId/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var bloodBankId = await response.Content.ReadAsStringAsync();
                        model.BloodBankId = Convert.ToInt32(bloodBankId);
                        model.IsActive = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error fetching BloodBank ID.");
                        return View(model);
                    }
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response1 = await client.PostAsync("BloodStock/AddBloodStock", jsonContent);

                    if (response1.IsSuccessStatusCode)
                    {
                        return RedirectToAction("GetByBloodBankId");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error while adding Blood Bank.");
                    }

                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBloodstock(int id)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync($"BloodStock/BStockByid/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodStock = JsonConvert.DeserializeObject<BloodStock>(jsonData);
                return View(bloodStock);
            }

            return View(new BloodStock());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBloodstock([FromForm] BloodStock model)
        {
            var client = clientFactory.CreateClient("MyApiClient");

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("BloodStock/UpdateBloodStock", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetByBloodBankId");

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error updating Blood Stock: {errorContent}");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult>InActiveBloodstock(int id)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync($"BloodStock/DeleteBloodStock/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsondata = await response.Content.ReadAsStringAsync();
                var bloodstock=JsonConvert.DeserializeObject<BloodStock>(jsondata);
                return RedirectToAction("GetByBloodBankId");
            }
            return BadRequest();
        }

    }
}
