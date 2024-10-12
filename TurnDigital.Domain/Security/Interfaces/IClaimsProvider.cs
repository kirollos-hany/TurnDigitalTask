using System.Security.Claims;
using TurnDigital.Domain.Security.Entities;

namespace TurnDigital.Domain.Security.Interfaces;

public interface IClaimsProvider
{
    Task<IEnumerable<Claim>> GetClaimsAsync(User user);
}