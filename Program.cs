using System.Text;
using BBMSDATA1.Context;
using BBMSDATA1.Repository.Generic.Implement;
using BBMSDATA1.Repository.Generic.Interface;
using BBMSDATA1.Repository.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BBMS1MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient("MyApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7193/");
            });
            builder.Services.AddDbContext<MYDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("db")));
            builder.Services.AddScoped(typeof(IRepo<>), typeof(Repo<>));
            builder.Services.AddScoped<HashPassword>();
            //for role based routing fun
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(options =>
          {
              var jwtkey = builder.Configuration["Jwt:Key"];
              if (string.IsNullOrEmpty(jwtkey))
              {
                  throw new InvalidOperationException("JWT Key is not configured.");
              }
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = builder.Configuration["Jwt:Issuer"],
                  ValidAudience = builder.Configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey))
              };
          });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

          
            app.UseSession();
            app.UseRouting();
           
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
