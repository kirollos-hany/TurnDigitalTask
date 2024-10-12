using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.UsersRoles.Commands;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.Security.Enums;

namespace TurnDigital.Application.Features.UsersRoles.Handlers;

internal sealed class SeedRolesCommandHandler : ICommandHandler<SeedRolesCommand>
{
    private readonly RoleManager<Role> _roleManager;

    public SeedRolesCommandHandler(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task Handle(SeedRolesCommand request, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

        var newRoles = Enum.GetNames<Roles>().Where(role => roles.All(savedRole => savedRole.Name != role)).Select(roleName => new Role()
        {
            Name = roleName,
        }).ToList();

        foreach (var role in newRoles)
        {
            await _roleManager.CreateAsync(role);
        }
    }
}