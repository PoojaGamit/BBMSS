using System.Net.Http.Headers;
using System.Text;
using BBMS1MVC.Helper;
using BBMSDATA1.Context;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        public async Task<IActionResult> Login([FromForm]LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                var client = clientFactory.CreateClient("MyApiClient");

                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                Console.WriteLine(JsonConvert.SerializeObject(model));

                var response = await client.PostAsync("Users/Login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("JWTToken", token);
                    Console.WriteLine(token);
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var Role = JwtHelper.GetUserRole(token);
                   
                    var userid=JwtHelper.GetUserId(token);
                    if(userid!=null)
                        HttpContext.Session.SetString("Userid", userid);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    Console.WriteLine(token);
            
                    if (Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (Role == "BAdmin")
                    {
                        return RedirectToAction("Index", "BloodBank");
                    }
                    else if (Role == "Donor")
                    {
                        return RedirectToAction("Index", "Donor");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Donor");
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }
            return View(model);
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
           // var token = HttpContext.Session.GetString("JWTToken");
            var userid=HttpContext.Session.GetString("Userid");
            if (userid!= null)
            {
                var client = clientFactory.CreateClient("MyApiClient");
                    var response = await client.GetAsync($"Users/UsersById/{userid}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var User= JsonConvert.DeserializeObject<Users>(jsonData);
                        return View(User);
                    }
                
            }
            return View(new Users());
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateUserprofile([FromForm]Users model)
        {
            var client = clientFactory.CreateClient("MyApiClient");
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("Users/UpdateUser", jsonContent);
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

        [HttpGet]
        [HttpPost]
        public IActionResult LogoutUser()
        {
            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "You have successfully logged out.";
            return RedirectToAction("Login", "User");
        }
    }
}
