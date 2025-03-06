using System.Runtime.InteropServices;
using System.Text;
using BBMS1MVC.Helper;
using BBMSDATA1.Context;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BBMS1MVC.Controllers
{
    public class ComponentStockController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly MYDBContext context;

        public ComponentStockController(IHttpClientFactory clientFactory, MYDBContext context)
        {
            this.clientFactory = clientFactory;
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetString("Userid");
            // var token = HttpContext.Session.GetString("JWTToken");
            if (id != null)
            {
                var client = clientFactory.CreateClient("MyApiClient");
                var response = await client.GetAsync($"ComponentStock/GetByCStockId/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var bloodstock = JsonConvert.DeserializeObject<List<ComponentStock>>(jsonData);
                    return View(bloodstock);
                }

            }
            return View(new List<ComponentStock>());
        }

        [HttpGet]
        public IActionResult AddCStocks()
        {
            ViewBag.BloodGroups = new SelectList(context.BloodGroups.ToList(), "BloodGroupId", "BloodGroupName");
            ViewBag.Component = new SelectList(context.ComponentTypes.ToList(), "ComponentTypeId", "ComponentTypeName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCStocks([FromForm] ComponentStock model)
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

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error fetching BloodBank ID.");
                        return View(model);
                    }
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response1 = await client.PostAsync("ComponentStock/AddStock", jsonContent);

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
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCStock(int id)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync($"ComponentStock/GetCStockbyId/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var Component = JsonConvert.DeserializeObject<ComponentStock>(jsonData);
                return View(Component);
            }
            return View(new ComponentStock());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCStock([FromForm] ComponentStock model)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("ComponentStock/UpdateStock", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error updating Blood Stock:{errorContent}");
            }
            return View(model);
        }
    }
}
