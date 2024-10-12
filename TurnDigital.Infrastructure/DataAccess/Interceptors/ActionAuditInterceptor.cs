using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TurnDigital.Domain.Logging.Entities;
using TurnDigital.Domain.Logging.Enums;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.Security.Interfaces;
using TurnDigital.Domain.ValueObjects;

namespace TurnDigital.Infrastructure.DataAccess.Interceptors;

public class ActionAuditInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    private readonly List<EntityEntry> _addedEntries = new();

    public ActionAuditInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        await using var scope = _serviceProvider.CreateAsyncScope();

        var userDeviceDetector = scope.ServiceProvider.GetRequiredService<IUserDeviceDetector>();
        var userIpAddressProvider = scope.ServiceProvider.GetRequiredService<IUserIpAddressProvider>();
        var authenticatedUserService = scope.ServiceProvider.GetRequiredService<IAuthenticatedUserService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var changeTrackerEntities = eventData.Context!.ChangeTracker.Entries();

        var entries = changeTrackerEntities
            .Where(entry => entry.Entity is not LoginAudit)
            .Where(entry => entry.Entity is not ActionAudit)
            .Where(entry => !entry.Metadata.IsOwned())
            .Where(entry => entry.State is not (EntityState.Detached or EntityState.Unchanged))
            .ToList();

        var actionsAudits = new List<ActionAudit>(entries.Count);

        var userId = authenticatedUserService.GetId();

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var userRoles = await userManager.GetRolesAsync(user);

        var roles = string.Join(",", userRoles);

        var ipAddress = userIpAddressProvider.GetIpAddress();

        var deviceInfo = userDeviceDetector.DetectDevice();

        var userAgent = userDeviceDetector.UserAgent();

        foreach (var entry in entries)
        {
            var type = entry.State switch
            {
                EntityState.Added => AuditActionType.Create,
                EntityState.Deleted => AuditActionType.Delete,
                EntityState.Modified => AuditActionType.Update,
                _ => AuditActionType.Create
            };

            if (type == AuditActionType.Create)
            {
                _addedEntries.Add(entry);

                continue;
            }

            var audit = CreateAudit(entry, user.Id, user.DisplayName, roles, ipAddress, deviceInfo, userAgent, type);

            actionsAudits.Add(audit);
        }

        await eventData.Context.AddRangeAsync(actionsAudits, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }


    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context is null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var canSkip = _addedEntries.Count == 0;

        if (canSkip)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var entries = _addedEntries.ToList();

        _addedEntries.Clear();

        var audits = new List<ActionAudit>(entries.Count);

        await using var scope = _serviceProvider.CreateAsyncScope();

        var userDeviceDetector = scope.ServiceProvider.GetRequiredService<IUserDeviceDetector>();
        var userIpAddressProvider = scope.ServiceProvider.GetRequiredService<IUserIpAddressProvider>();
        var authenticatedUserService = scope.ServiceProvider.GetRequiredService<IAuthenticatedUserService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var userId = authenticatedUserService.GetId();

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var userRoles = await userManager.GetRolesAsync(user);

        var roles = string.Join(",", userRoles);

        var ipAddress = userIpAddressProvider.GetIpAddress();

        var deviceInfo = userDeviceDetector.DetectDevice();

        var userAgent = userDeviceDetector.UserAgent();

        audits.AddRange(entries.Select(entry => CreateAudit(entry, user.Id, user.DisplayName, roles, ipAddress,
            deviceInfo, userAgent, AuditActionType.Create)));

        eventData.Context.AddRange(audits);

        await eventData.Context.SaveChangesAsync(cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static ActionAudit CreateAudit(EntityEntry entry, int userId, string userDisplayName, string roles,
        string ipAddress, DeviceInfo? deviceInfo, string userAgent, AuditActionType type)
    {
        var valuesDict = new Dictionary<string, object?>();

        var oldValuesDict = new Dictionary<string, object?>();

        var entityId = string.Empty;

        foreach (var property in entry.Properties)
        {
            if (property.Metadata.IsPrimaryKey())
            {
                entityId = property.CurrentValue?.ToString();
            }

            if (property.IsModified)
            {
                oldValuesDict[property.Metadata.Name] = property.OriginalValue;
            }

            valuesDict[property.Metadata.Name] = property.CurrentValue;
        }

        var values = JsonConvert.SerializeObject(valuesDict, Formatting.Indented);

        var oldValues = JsonConvert.SerializeObject(oldValuesDict, Formatting.Indented);

        var audit = new ActionAudit(userId, ipAddress, userAgent, deviceInfo,
            entry.Metadata.ClrType.Name,
            entityId,
            userDisplayName, roles, type, oldValues, values, string.Join(", ", oldValuesDict.Keys.ToList()));

        return audit;
    }
}