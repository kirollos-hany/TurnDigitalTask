using TurnDigital.Domain.Entities.Interfaces;
using TurnDigital.Domain.Features.Products.Entities;

namespace TurnDigital.Domain.Features.Categories.Entities;

public class Category : IEntity
{
    public int Id { get; private set; }
    
    public required string Name { get; set; }

    public List<Product> Products { get; private set; } = new ();
}