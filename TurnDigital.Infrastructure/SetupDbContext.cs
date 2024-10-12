using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TurnDigital.Infrastructure.DataAccess;
using TurnDigital.Infrastructure.DataAccess.Interceptors;

namespace TurnDigital.Infrastructure;

public static class SetupDbContext
{
    public static void AddDbContext(this IServiceCollection serviceCollection, bool isDevelopment)
    {
        serviceCollection.AddDbContext<TurnDigitalDbContext>((sp, options) =>
        {
            if (isDevelopment)
            {
                options.EnableSensitiveDataLogging();
                options.LogTo(Console.WriteLine, LogLevel.Information);
                options.EnableDetailedErrors();
            }

            options.UseInMemoryDatabase("TurnDigital").AddInterceptors(sp.GetRequiredService<ActionAuditInterceptor>(),
                sp.GetRequiredService<FireEventInterceptor>());
        });
    }
}