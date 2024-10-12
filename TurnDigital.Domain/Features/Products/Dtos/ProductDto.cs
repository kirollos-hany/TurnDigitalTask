using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.IO.Attributes;
using TurnDigital.Domain.Utilities;

namespace TurnDigital.Domain.Features.Products.Dtos;

public record ProductDto
{
    public required int Id { get; init; }

    public required CategoryDto Category { get; init; }

    [RequiresBaseUrl] public required string Image { get; init; }

    public required string Slug { get; init; }

    public required double Price { get; init; }

    public required string? Description { get; init; }

    public required string Name { get; init; }

    public required DateTime UtcCreatedDate { get; init; }
};