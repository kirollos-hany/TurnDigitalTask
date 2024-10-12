using Microsoft.AspNetCore.Identity;
using TurnDigital.Domain.Entities.Interfaces;
using TurnDigital.Domain.Logging.Enums;
using TurnDigital.Domain.Security.Events;
using TurnDigital.Domain.ValueObjects;

namespace TurnDigital.Domain.Security.Entities;

public class User : IdentityUser<int>, IEntityWithDomainEvents, IEntity
{
    public bool IsActive { get; private set; } = true;

    public string DisplayName { get; set; } = string.Empty;

    public string ProfilePicture { get; set; } = string.Empty;

    private readonly List<IDomainEvent> _events = new ();
    public IReadOnlyList<IDomainEvent> Events => _events;

    public RefreshToken? RefreshToken { get; private set; }

    public void Login(string? refreshToken)
    {
        if (refreshToken is null)
        {
            _events.Add(new LoginEvent(this, LoginStatus.Failure));
            return;
        }
        
        _events.Add(new LoginEvent(this, LoginStatus.Success));
        
        if (RefreshToken is null)
        {
            RefreshToken = new RefreshToken(refreshToken);
            return;
        }

        RefreshToken.Revoke(refreshToken);
    }

    public void Revoke(string refreshToken)
    {
        RefreshToken!.Revoke(refreshToken);
    }
}
