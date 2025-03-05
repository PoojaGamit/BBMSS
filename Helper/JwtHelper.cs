using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace BBMS1MVC.Helper
{
    public static class JwtHelper
    {
        public static string? GetClaimValue(string token, string claimType)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        public static string? GetUserId(string token)
        {
            return GetClaimValue(token, "Id");
        }

        public static string GetUserName(string token)
        {
            return GetClaimValue(token, "UserName") ?? "Guest";
        }

        public static string GetUserRole(string token)
        {
            return GetClaimValue(token, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") ?? "User";
        }
    }
}
