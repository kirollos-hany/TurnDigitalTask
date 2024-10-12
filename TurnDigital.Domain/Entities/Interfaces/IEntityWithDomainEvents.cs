namespace TurnDigital.Domain.Entities.Interfaces;

public interface IEntityWithDomainEvents
{
    IReadOnlyList<IDomainEvent> Events { get; }
}