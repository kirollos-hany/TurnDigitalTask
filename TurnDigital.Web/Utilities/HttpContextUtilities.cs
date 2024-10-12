using System.Security.Claims;

namespace TurnDigital.Web.Utilities;

public static class HttpContextUtilities
{
    public static bool IsAuthenticated(this ClaimsPrincipal user) => user.Identity is not null && user.Identity.IsAuthenticated;
}