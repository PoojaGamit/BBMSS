using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BBMSDATA1.Middleware.MVCMiddleware
{
   public class JWTAPIVERIMVC
    {
        private readonly RequestDelegate next;

        public JWTAPIVERIMVC(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var token = context.Session.GetString("JWTToken");

            if (token != null)
            {
                // Validate the token
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken != null)
                    {
                        var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
                        var roles = string.Join(",", jwtToken.Claims.Where(x => x.Type == "role").Select(x => x.Value));

                        context.Items["User"] = userId;
                        context.Items["Roles"] = roles;
                        var identity = new ClaimsIdentity(jwtToken.Claims, "Bearer");
                        var principal = new ClaimsPrincipal(identity);
                        context.User = principal;
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid Token");
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    // context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("Invalid Token");
                    return;
                }
            }

            await next(context);
        }
    }
}
