using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BBMSDATA1.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BBMSDATA1.Repository.Services
{
    public class Auth
    {
        private readonly MYDBContext context;
        private readonly IConfiguration configuration;
        private readonly HashPassword hashPassword;

        public Auth(MYDBContext context, IConfiguration configuration, HashPassword hashPassword)
        {
            this.context = context;
            this.configuration = configuration;
            this.hashPassword = hashPassword;
        }

        public async Task<string?> Login(Models.LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return "Username or Password cannot be empty";
            }

            var user = context.Users.FirstOrDefault(s => s.Email == loginRequest.Username);

            if (user == null)
            {
                return "Invalid Username";
            }

            var isPasswordValid = await hashPassword.VerifyPasswordAsync(user, loginRequest.Password);
            if (!isPasswordValid)
            {
                return "Invalid Username or Password";
            }
            

            var sub = configuration["Jwt:Subject"] ?? throw new Exception("JWT Subject is not configured.");
            var jwtKey = configuration["Jwt:Key"] ?? throw new Exception("JWT Key is not configured.");
            var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer is not configured.");
            var jwtAudience = configuration["Jwt:Audience"] ?? throw new Exception("JWT Audience is not configured.");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, sub),
                new Claim("Id", user.UserId.ToString()),
                new Claim("UserName", user.Email ?? string.Empty)
            };

            var userRole = context.Roles.Where(v => v.R_Id == user.R_ID).Select(v => v.R_Name).FirstOrDefault();
            if (userRole == null)
            {
                return "User Role Not Found";
            }

            claims.Add(new Claim(ClaimTypes.Role, userRole));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
         issuer: jwtIssuer,
         audience: jwtAudience,
         claims: claims,
         expires: DateTime.UtcNow.AddHours(2), // ⬅ Increased expiry time
         signingCredentials: signinCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
