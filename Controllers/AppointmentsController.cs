using System.Text;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;

namespace BBMS1MVC.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public AppointmentsController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
           var id = HttpContext.Session.GetString("Userid");
            if (id == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                return View();
            }
        
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] Appointments model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var id = HttpContext.Session.GetInt32("Userid");
            if (id == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                model.DonorId = (int)id;
                var client = clientFactory.CreateClient("MyApiClient");
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response1 = await client.PostAsync("Appointments/AddAppointments", jsonContent);

                if (response1.IsSuccessStatusCode)
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
    }
}
