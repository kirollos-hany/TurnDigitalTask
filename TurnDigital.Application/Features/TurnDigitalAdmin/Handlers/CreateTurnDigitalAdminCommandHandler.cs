using Microsoft.AspNetCore.Identity;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.TurnDigitalAdmin.Commands;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.Security.Enums;

namespace TurnDigital.Application.Features.TurnDigitalAdmin.Handlers;

internal sealed class CreateTurnDigitalAdminCommandHandler : ICommandHandler<CreateTurnDigitalAdminCommand>
{
    private readonly UserManager<User> _userManager;

    public CreateTurnDigitalAdminCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(CreateTurnDigitalAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userManager.FindByEmailAsync(Constants.TurnDigitalAdmin.Email);

        if (admin is not null)
            return;

        var newAdmin = new User
        {
            Email = Constants.TurnDigitalAdmin.Email,
            UserName = Constants.TurnDigitalAdmin.UserName,
            EmailConfirmed = true,
            DisplayName = Constants.TurnDigitalAdmin.DisplayName
        };

        var result = await _userManager.CreateAsync(newAdmin, Constants.TurnDigitalAdmin.Password);

        if (result.Succeeded)
        {
            var roles = new [] { nameof(Roles.TurnDigitalAdmin) };
            
            var _ = await _userManager.AddToRolesAsync(newAdmin, roles);
        }
    }
}