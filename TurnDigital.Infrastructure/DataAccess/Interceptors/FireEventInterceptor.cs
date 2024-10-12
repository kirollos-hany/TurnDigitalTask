using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TurnDigital.Domain.Entities.Interfaces;

namespace TurnDigital.Infrastructure.DataAccess.Interceptors;

public class FireEventInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public FireEventInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new ())
    {
        if (eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        
        var tasks = new List<Task>();

        var entries = eventData.Context.ChangeTracker.Entries();
        
        foreach (var entry in entries)
        {
            if (entry.Entity is not IEntityWithDomainEvents entity) 
                continue;
            
            var events = entity.Events;

            tasks.AddRange(events.Select(domainEvent => _mediator.Publish(domainEvent, cancellationToken)));
        }

        await Task.WhenAll(tasks);
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}