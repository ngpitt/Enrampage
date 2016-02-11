using System;
using System.Security.Claims;

namespace Enrampage.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool Admin(this ClaimsPrincipal Principal)
        {
            return Principal.HasClaim(ClaimTypes.Role, "Admin");
        }

        public static int UserId(this ClaimsPrincipal Principal)
        {
            return Convert.ToInt32(Principal.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}