using TurnDigital.Domain.Entities.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;

namespace TurnDigital.Domain.Features.Products.Entities;

public class Product : IEntity
{
    public const string DefaultImage = "product/default.png";
    public int Id { get; private set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public required string? Description { get; set; }

    public required double Price { get; set; }

    public required string Image { get; set; }

    public DateTime UtcCreatedDate { get; private set; } = DateTime.UtcNow;

    public int CategoryId { get; private set; }

    public required Category? Category { get; set; }
}