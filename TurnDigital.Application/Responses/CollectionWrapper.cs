namespace TurnDigital.Application.Responses;

public record CollectionWrapper<T>
{
    public required IReadOnlyList<T> Results { get; init; }
};