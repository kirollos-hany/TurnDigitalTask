namespace TurnDigital.Domain.Features.Categories.Dtos;

public record CategoryDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }
};