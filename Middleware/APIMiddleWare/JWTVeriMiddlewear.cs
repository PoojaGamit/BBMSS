using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBMSDATA1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BBMSDATA1.Middleware.APIMiddleWare
{
   public class JWTVeriMiddlewear
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public JWTVeriMiddlewear(RequestDelegate next,IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        { 
          var token = context.Request.Headers["Authorization"].FirstOrDefault();
            if (token != null)
            {
                AttachUserToContext(context, token);
            }
               
            await next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyString = configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(keyString))
                    throw new Exception("JWT key is missing in configuration.");
                var key=Encoding.UTF8.GetBytes(keyString);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);
                var username = jwtToken.Claims.First(x => x.Type == "UserName").Value;
                var role = jwtToken.Claims.First(x => x.Type == "role").Value;
                context.Items["UserId"] = userId;
                context.Items["UserName"] = username;
                context.Items["Roles"] = role;
            }
            catch {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsync("Unauthorized: Invalid token").Wait();
            }
        
        }
    }
}
