using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Account.Commands;
using TurnDigital.Domain.Security.Interfaces;

namespace TurnDigital.Application.Features.Account.Handlers;

internal sealed class SignOutCookieAuthenticationHandler : ICommandHandler<SignOutCookieAuthentication>
{
    private readonly ICookieAuthenticationService _cookieAuthenticationService;

    public SignOutCookieAuthenticationHandler(ICookieAuthenticationService cookieAuthenticationService)
    {
        _cookieAuthenticationService = cookieAuthenticationService;
    }

    public async Task Handle(SignOutCookieAuthentication request, CancellationToken cancellationToken)
    {
        await _cookieAuthenticationService.SignOutAsync();
    }
}