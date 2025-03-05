using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BBMSDATA1.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BBMSDATA1.Repository.Services
{
  public class Auth
    {
        private readonly MYDBContext context;
        private readonly IConfiguration configuration;
        private readonly HashPassword hashPassword;

        public Auth(MYDBContext context,IConfiguration configuration,HashPassword hashPassword)
        {
            this.context = context;
            this.configuration = configuration;
            this.hashPassword = hashPassword;
        }
        public async Task<string> Login(Models.LoginRequest loginRequest)
        {
            if (loginRequest.Username != null & loginRequest.Password != null)
            {
                var user=context.Users.FirstOrDefault(s=>s.Email==loginRequest.Username);
                Console.WriteLine(user);
                if (user != null)
                {
                    var ispassword=await hashPassword.VerifyPasswordAsync(user, loginRequest.Password);
                    if (ispassword)
                    {
                        var sub = configuration["Jwt:Subject"] ?? throw new Exception("JWT is not configured.");
                        var claims = new List<Claim>
                        {
                          new Claim(JwtRegisteredClaimNames.Sub,sub),
                          new Claim("Id",user.UserId.ToString()),
                          new Claim("UserName",user.Email??string.Empty)
                        };
                        var id = user.R_ID;
                        var userrole = context.Users.Where(u => u.UserId == user.UserId).Select(u => u.R_ID).FirstOrDefault();
                        var role = context.Roles.Where(v => v.R_Id == id).Select(v =>v.R_Name).FirstOrDefault();

                        
                        if (role != null)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        else
                        {
                            return "User Role is Not Found";
                        }
                        var jwtKey = configuration["Jwt:Key"];
                        if (jwtKey == null)
                            return "JWT Key is Not Configured";
                        var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                        var signin=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            configuration["Jwt:Issuer"],
                            configuration["Jwt:Audience"],
                            claims,
                            expires:DateTime.UtcNow.AddMinutes(10),
                            signingCredentials:signin);
                        var jwtToken=new JwtSecurityTokenHandler().WriteToken(token);
                        return jwtToken;
                    }
                    else 
                    {
                        return "InValid Password";
                    }
                }
                else {

                    return "User Not Found";
                }
            }
            else
            {
                return "Credentials are not Valid";
            }
        }
    }
}
