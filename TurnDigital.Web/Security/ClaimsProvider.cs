using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.Security.Enums;
using TurnDigital.Domain.Security.Interfaces;

namespace TurnDigital.Web.Security
{
    public class ClaimsProvider : IClaimsProvider
    {
        private readonly UserManager<User> _userManager;

        public ClaimsProvider(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
        
            var roles = await _userManager.GetRolesAsync(user);
        
            var userIdClaim = new Claim(ClaimsIdentity.DefaultNameClaimType, Convert.ToString(user.Id) ?? string.Empty);
        
            var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString());
        
            var emailClaim = new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty);

            var statusClaim = new Claim(nameof(ClaimType.AccountStatus), user.IsActive.ToString());

            var tokenClaims = claims.Concat(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r))).ToList();
        
            tokenClaims.AddRange(new [] { userIdClaim, emailClaim, jtiClaim, statusClaim });

            return tokenClaims;
        }
    }
}

