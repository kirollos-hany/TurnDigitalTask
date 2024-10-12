using MediatR;
using TurnDigital.Application.Features.TurnDigitalAdmin.Commands;
using TurnDigital.Application.Features.UsersRoles.Commands;

namespace TurnDigital.Web;

public static class Seeders
{
    public static async Task SeedRoles(this IServiceProvider serviceProvider)
    {
        var mediatr = serviceProvider.GetRequiredService<IMediator>();

        var seedRolesCommand = new SeedRolesCommand();

        await mediatr.Send(seedRolesCommand);
    }
    
    public static async Task CreateTurnDigitalAdmin(this IServiceProvider serviceProvider)
    {
        var mediatr = serviceProvider.GetRequiredService<IMediator>();

        var createAdminCommand = new CreateTurnDigitalAdminCommand();

        await mediatr.Send(createAdminCommand);
    }
}