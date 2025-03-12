using System.Text;
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
            var bloodStockQuery = await context.BloodStock.Where(s => s.IsActive == true).GroupBy(s => s.BloodGroupId).Select(g => new
                {
                    BloodGroupId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                }).ToListAsync();

            var bloodStockData = bloodGroups.Select(bg => bloodStockQuery.FirstOrDefault(bs => bs.BloodGroupId == bg.BloodGroupId)?.TotalQuantity ?? 0).ToList();

            ViewBag.BloodStockData = bloodStockData;

            // Blood Group Distribution for pie chart (based on stock)
            ViewBag.BloodGroupDistribution = bloodStockData;

            // Blood Group Labels
            ViewBag.BloodGroupLabels = bloodGroups.Select(bg => bg.BloodGroupName).ToList();

            ViewBag.UpcomingCamps = await context.BloodCamps
                .Where(bc => bc.Date >= DateTime.Today).OrderBy(bc => bc.Date).Take(5).Select(bc => new
                {
                    Name = bc.Name,
                    Location = bc.Location,
                    Date = bc.Date,
                    Time = $"{bc.StartTime.ToString(@"hh\:mm tt")} - {bc.EndTime.ToString(@"hh\:mm tt")}",
                    Organizer = bc.Organizer
                }).ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Bloodstock()
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var response = await client.GetAsync("BloodStock/AllBloodStock");

            // Get blood groups from database
            var bloodGroups = await context.BloodGroups.ToListAsync();
            ViewBag.BloodGroups = bloodGroups;

            // Get component types from database
            var componentTypes = await context.ComponentTypes.ToListAsync();
            ViewBag.ComponentTypes = componentTypes;

            // Get blood banks from database
            var bloodBanks = await context.BloodBanks.Where(b => b.IsActive == true).ToListAsync();
            ViewBag.BloodBanks = bloodBanks;

            // Define expiry status options
            ViewBag.ExpiryStatuses = new List<object>
    {
        new { Id = "expired", Name = "Expired" },
        new { Id = "expiring", Name = "Expiring Soon (7 days)" },
        new { Id = "valid", Name = "Valid" }
    };

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodStocks = JsonConvert.DeserializeObject<IEnumerable<BloodStock>>(jsonData);

                return View(bloodStocks);
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
        public async Task<IActionResult> Componentstock(int? bloodBankId = null, int? bloodGroupId = null, int? componentTypeId = null, string dateRange = null)
        {
            // Get blood stock data
            var bloodStockItems = await context.BloodStock.ToListAsync();

            // Map BloodStock to ComponentStock
            var components = bloodStockItems.Select(bs => new ComponentStock
            {
                CId = bs.StockId,               // Assuming CId maps to StockId
                BloodBankId = bs.BloodBankId,
                BloodGroupId = bs.BloodGroupId,
                ComponentTypeId = bs.ComponentTypeId,
                Quantity = bs.Quantity,
                ExpiryDate = bs.ExpiryDate,
                LastUpdated = bs.LastUpdated,
                UpdatedBy = bs.UpdatedBy
            }).ToList();

            // Get blood banks for filter
            var bloodBanks = await context.BloodBanks .Where(b => b.IsActive == true).Select(b => new { Id = b.BloodBankId, Name = b.BloodBankName }).ToListAsync();
            ViewBag.BloodBanks = bloodBanks;

            // Get blood groups for filter
            var bloodGroups = await context.BloodGroups.Select(a=>new {Id=a.BloodGroupId,Name=a.BloodGroupName}).ToListAsync();
            ViewBag.BloodGroups = bloodGroups;

            // Get component types for filter
            var componentTypes = await context.ComponentTypes.Select(b=>new {Id=b.ComponentTypeId,Name=b.ComponentTypeName}).ToListAsync();
            ViewBag.ComponentTypes = componentTypes;

            // Store selected filter values for the view
            ViewBag.SelectedBloodBankId = bloodBankId;
            ViewBag.SelectedBloodGroupId = bloodGroupId;
            ViewBag.SelectedComponentTypeId = componentTypeId;
            ViewBag.SelectedDateRange = dateRange;

            // Apply filters if provided
            IEnumerable<ComponentStock> filteredComponents = components;

            if (bloodBankId.HasValue)
            {
                filteredComponents = filteredComponents.Where(c => c.BloodBankId == bloodBankId.Value);
            }

            if (bloodGroupId.HasValue)
            {
                filteredComponents = filteredComponents.Where(c => c.BloodGroupId == bloodGroupId.Value);
            }

            if (componentTypeId.HasValue)
            {
                filteredComponents = filteredComponents.Where(c => c.ComponentTypeId == componentTypeId.Value);
            }

            if (!string.IsNullOrEmpty(dateRange))
            {
                var dates = dateRange.Split('-');
                if (dates.Length == 2)
                {
                    var startDate = DateTime.Parse(dates[0].Trim());
                    var endDate = DateTime.Parse(dates[1].Trim()).AddDays(1).AddSeconds(-1); // End of the selected day

                    filteredComponents = filteredComponents.Where(c =>
                        c.ExpiryDate >= startDate && c.ExpiryDate <= endDate);
                }
            }

            return View(filteredComponents);
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

        public async Task<IActionResult> ExportBloodStock()
        {
            try
            {
                // Get all blood stock data
                var client = clientFactory.CreateClient("MyApiClient");
                var response = await client.GetAsync("BloodStock/AllBloodStock");

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Bloodstock", new { error = "Failed to retrieve blood stock data for export" });
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                var bloodStocks = JsonConvert.DeserializeObject<IEnumerable<BloodStock>>(jsonData);

                // Get blood groups and component types for mapping IDs to names
                var bloodGroups = await context.BloodGroups.ToDictionaryAsync(bg => bg.BloodGroupId, bg => bg.BloodGroupName);
                var componentTypes = await context.ComponentTypes.ToDictionaryAsync(ct => ct.ComponentTypeId, ct => ct.ComponentTypeName);

                // Build CSV content
                var csvBuilder = new StringBuilder();

                // Add CSV header
                csvBuilder.AppendLine("Blood Bank ID,Blood Group,Component Type,Quantity,Last Updated,Updated By,Expiry Date,Status");

                // Add data rows
                foreach (var item in bloodStocks)
                {
                    var bloodGroupName = bloodGroups.ContainsKey(item.BloodGroupId) ? bloodGroups[item.BloodGroupId] : item.BloodGroupId.ToString();
                    var componentName = componentTypes.ContainsKey(item.ComponentTypeId) ? componentTypes[item.ComponentTypeId] : item.ComponentTypeId.ToString();
                    var status = item.IsActive ? "Active" : "Inactive";

                    csvBuilder.AppendLine($"{item.BloodBankId}," +
                                          $"\"{bloodGroupName}\"," +
                                          $"\"{componentName}\"," +
                                          $"{item.Quantity}," +
                                          $"\"{item.LastUpdated:yyyy-MM-dd HH:mm}\"," +
                                          $"\"{item.UpdatedBy}\"," +
                                          $"\"{item.ExpiryDate:yyyy-MM-dd}\"," +
                                          $"\"{status}\"");
                }

                // Generate file name with timestamp
                var fileName = $"Blood_Stock_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                // Return as a downloadable file
                byte[] fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                // Log the exception
                // logger.LogError(ex, "Error exporting blood stock data");

                // Redirect with error message
                return RedirectToAction("Bloodstock", new { error = "An error occurred during export: " + ex.Message });
            }
        }
    }

}

