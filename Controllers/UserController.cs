using System.Net.Http.Headers;
using System.Text;
using BBMS1MVC.Helper;
using BBMSDATA1.Context;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace BBMS1MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly MYDBContext context;
       
        public UserController(IHttpClientFactory clientFactory, MYDBContext context)
        {
            this.clientFactory = clientFactory;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = clientFactory.CreateClient("MyApiClient");
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Users/Login", jsonContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("API Response: " + responseBody); 

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                return View(model);
            }

           
            var tokenObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
            var token = "";
            if (tokenObject != null)
            {
                token = (string?)tokenObject.token ?? (string?)tokenObject.Token;
            }


            if (string.IsNullOrWhiteSpace(token) || !token.Contains("."))
            {
                ModelState.AddModelError(string.Empty, "Invalid token received from server.");
                return View(model);
            }

            HttpContext.Session.SetString("JWTToken", token);

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var role = JwtHelper.GetUserRole(token);
                var userId = JwtHelper.GetUserId(token);
               var userName = JwtHelper.GetUserName(token);

                if(!string.IsNullOrEmpty(userName))
                {
                    HttpContext.Session.SetString("UserName", userName);

                }
                if (!string.IsNullOrEmpty(userId))
                {
                    HttpContext.Session.SetString("Userid", userId);

                }
                if (!string.IsNullOrEmpty(role))
                { 
                  HttpContext.Session.SetString("Role", role);
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return role switch
                {
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "BAdmin" => RedirectToAction("Index", "BloodBank"),
                    "Donor" => RedirectToAction("UserProfile", "User"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error parsing JWT token.");
                Console.WriteLine($"JWT Parsing Error: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = new SelectList(context.Roles.ToList(), "R_Id", "R_Name");
            ViewBag.BloodGroups = new SelectList(context.BloodGroups.ToList(), "BloodGroupId", "BloodGroupName");
            ViewBag.States = new SelectList(context.States.ToList(), "Id", "StateName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users model)
        {

            var client = clientFactory.CreateClient("MyApiClient");

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Users/Register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                context.Users.Add(model);
                // await context.SaveChangesAsync();
                return RedirectToAction("Login", "User");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to register user.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUserprofile()
        {
            var userid = HttpContext.Session.GetString("Userid");
            if (userid != null)
            {
                var client = clientFactory.CreateClient("MyApiClient");
                var response = await client.GetAsync($"Users/UsersById/{userid}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var User = JsonConvert.DeserializeObject<Users>(jsonData);
                    return View(User);
                }

            }
            return View(new Users());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserprofile([FromForm] Users model)
        {
            var role=HttpContext.Session.GetString("Role");
            var client = clientFactory.CreateClient("MyApiClient");
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("Users/UpdateUser", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (role == "BAdmin")
                {
                    return RedirectToAction("Index", "BloodBank");
                }
                else if (role == "Donor")
                {
                    return RedirectToAction("UserProfile", "User");
                }
                //return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error updating Blood Stock:{errorContent}");
            }
            return View(model);

        }

        [HttpGet]
        [HttpPost]
        public IActionResult LogoutUser()
        {
            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "You have successfully logged out.";
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var userId = HttpContext.Session.GetString("Userid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("User", "Login");
            }

            int uid = Convert.ToInt32(userId);
            var client = clientFactory.CreateClient("MyApiClient");

            try
            {
                // Get user data
                var userResponse = await client.GetAsync($"Users/UsersById/{uid}");
                userResponse.EnsureSuccessStatusCode();
                var jsonData = await userResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Users>(jsonData);

                // Get blood group name
               var bloodGroup=context.Users.Where(a=>a.UserId == uid).Select(Users => Users.BloodGroupId).FirstOrDefault();
                var bloodGroupname=context.BloodGroups.Where(a => a.BloodGroupId == bloodGroup).Select(BloodGroups => BloodGroups.BloodGroupName).FirstOrDefault();
                ViewBag.BloodGroupName = bloodGroupname;
                

                // Get state name
                var stateid=context.Users.Where(a => a.UserId == uid).Select(Users => Users.StateId).FirstOrDefault();
             var state=context.States.Where(a => a.Id == stateid).Select(States => States.StateName).FirstOrDefault();
                ViewBag.StateName = state;
                 

                // Get donation history
                var donationsResponse = await client.GetAsync($"Donations/ByDonor/{uid}");
                if (donationsResponse.IsSuccessStatusCode)
                {
                    var donationsData = await donationsResponse.Content.ReadAsStringAsync();
                    var donationHistory = JsonConvert.DeserializeObject<List<dynamic>>(donationsData);
                    ViewBag.DonationHistory = donationHistory;
                    ViewBag.TotalDonations = donationHistory?.Count ?? 0;

                    // Calculate days since last donation
                    if (donationHistory != null && donationHistory.Count > 0)
                    {
                        // Manual approach instead of using LINQ on dynamic objects
                        DateTime lastDonationDate = DateTime.MinValue;
                        foreach (var donation in donationHistory)
                        {
                            var donationDate = DateTime.Parse(donation.DonationDate.ToString());
                            if (donationDate > lastDonationDate)
                            {
                                lastDonationDate = donationDate;
                            }
                        }

                        ViewBag.DaysSinceLastDonation = (DateTime.Now - lastDonationDate).Days;
                    }
                    else
                    {
                        ViewBag.DaysSinceLastDonation = 0;
                    }
                }
                else
                {
                    ViewBag.DonationHistory = new List<dynamic>();
                    ViewBag.TotalDonations = 0;
                    ViewBag.DaysSinceLastDonation = 0;
                }

                // Get blood requests
                var requestsResponse = await client.GetAsync($"BloodRequest/ByPatient/{uid}");
                if (requestsResponse.IsSuccessStatusCode)
                {
                    var requestsData = await requestsResponse.Content.ReadAsStringAsync();
                    var bloodRequests = JsonConvert.DeserializeObject<List<dynamic>>(requestsData);
                    ViewBag.BloodRequests = bloodRequests;

                    // Count pending requests manually
                    int pendingCount = 0;
                    if (bloodRequests != null)
                    {
                        foreach (var request in bloodRequests)
                        {
                            if (request.Status.ToString() == "Pending")
                            {
                                pendingCount++;
                            }
                        }
                    }
                    ViewBag.PendingRequests = pendingCount;
                }
                else
                {
                    ViewBag.BloodRequests = new List<dynamic>();
                    ViewBag.PendingRequests = 0;
                }

                var campcount=context.BloodCamps.Where(a=>a.IsActive == true).Count();
                ViewBag.UpcomingCamps = campcount;

                var camps = context.BloodCamps.Where(a => a.IsActive == true).ToList();
                ViewBag.UpcomingCampsList = camps;
                // Get upcoming blood camps
                //var campsResponse = await client.GetAsync("BloodCamps/Upcoming");
                //if (campsResponse.IsSuccessStatusCode)
                //{
                //    var campsData = await campsResponse.Content.ReadAsStringAsync();
                //    var camps = JsonConvert.DeserializeObject<List<dynamic>>(campsData);

                //    // Check registration status for each camp
                //    if (camps != null)
                //    {
                //        foreach (var camp in camps)
                //        {
                //            var regResponse = await client.GetAsync($"CampRegistrations/CheckRegistration?campId={camp.CampId}&userId={uid}");
                //            if (regResponse.IsSuccessStatusCode)
                //            {
                //                var regData = await regResponse.Content.ReadAsStringAsync();
                //                camp.IsRegistered = bool.Parse(regData);
                //            }
                //            else
                //            {
                //                camp.IsRegistered = false;
                //            }
                //        }
                //    }

                //    ViewBag.UpcomingCampsList = camps;
                //    ViewBag.UpcomingCamps = camps?.Count ?? 0;
                //}
                //else
                //{
                //    ViewBag.UpcomingCampsList = new List<dynamic>();
                //    ViewBag.UpcomingCamps = 0;
                //}

                // Get notifications
               
                    ViewBag.Notifications = context.Notification.Where(a=>a.IsActive == true).ToList();

                return View(user);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to load profile data. Please try again later.";
                return View(new Users());
            }
        }
    }
}
