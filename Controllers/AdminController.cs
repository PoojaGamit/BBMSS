using System.Collections.Generic;
using BBMSDATA1.Context;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BBMS1MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly MYDBContext context;

        public AdminController(IHttpClientFactory clientFactory,MYDBContext context)
        {
            this.clientFactory = clientFactory;
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Basic statistics for cards
            ViewBag.TotalBloodUnits = await context.BloodStock.SumAsync(x => x.Quantity);
            ViewBag.TotalBloodBanks = await context.BloodBanks.CountAsync();
            ViewBag.TotalDonors = await context.Users.Where(u => u.R_ID == 2).CountAsync();
            ViewBag.PendingRequests = await context.BloodRequest.CountAsync(x => x.Status == "Pending");

            // Get blood groups
            var bloodGroups = await context.BloodGroups.ToListAsync();

            // Blood Stock Data for bar chart
            var bloodStockQuery = await context.BloodStock
                .Where(s => s.IsActive == true)
                .GroupBy(s => s.BloodGroupId)
                .Select(g => new
                {
                    BloodGroupId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .ToListAsync();

            // Join with blood groups to get names
            var bloodStockData = bloodGroups
                .Select(bg => bloodStockQuery
                    .FirstOrDefault(bs => bs.BloodGroupId == bg.BloodGroupId)?
                    .TotalQuantity ?? 0)
                .ToList();

            ViewBag.BloodStockData = bloodStockData;

            // Blood Group Distribution for pie chart (based on stock)
            ViewBag.BloodGroupDistribution = bloodStockData;

            // Blood Group Labels
            ViewBag.BloodGroupLabels = bloodGroups.Select(bg => bg.BloodGroupName).ToList();

            ViewBag.UpcomingCamps = await context.BloodCamps
                .Where(bc => bc.Date >= DateTime.Today)
                .OrderBy(bc => bc.Date)
                .Take(5)
                .Select(bc => new
                {
                    Name = bc.Name,
                    Location = bc.Location,
                    Date = bc.Date,
                    Time = $"{bc.StartTime.ToString(@"hh\:mm tt")} - {bc.EndTime.ToString(@"hh\:mm tt")}",
                    Organizer = bc.Organizer
                })
                .ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Bloodstock()
        { 
           var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync("BloodStock/AllBloodStock");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodBanks = JsonConvert.DeserializeObject<IEnumerable<BloodStock>>(jsonData);

                return View(bloodBanks);
            }
            else
            {
                return View(new List<BloodStock>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> BloodCamps()
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync("BloodCamps/GetAllCamps");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodBanks = JsonConvert.DeserializeObject<IEnumerable<BloodCamps>>(jsonData);

                return View(bloodBanks);
            }
            else
            {
                return View(new List<BloodCamps>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> BloodBanks()
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
        public async Task<IActionResult> Componentstock()
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync("ComponentStock/GetCStock");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodBanks = JsonConvert.DeserializeObject<IEnumerable<ComponentStock>>(jsonData);

                return View(bloodBanks);
            }
            else
            {
                return View(new List<ComponentStock>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync("Users/AllUsers");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodBanks = JsonConvert.DeserializeObject<IEnumerable<Users>>(jsonData);
                return View(bloodBanks);
            }
            else
            {
                return View(new List<Users>());
            }
        }
    }

}

