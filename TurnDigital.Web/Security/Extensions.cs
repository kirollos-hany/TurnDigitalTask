using System.Security.Claims;

namespace TurnDigital.Web.Security;

public static class Extensions
{
    public static int GetId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType);

        return userIdClaim is null ? default : int.Parse(userIdClaim.Value);
    }
}