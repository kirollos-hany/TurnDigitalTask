using Bogus;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Utilities;

namespace TurnDigital.Infrastructure.DataAccess.Seed;

public static class FakeDataSeed
{
    public static async Task SeedFakeData(this IRepository repository)
    {
        var categories = new Faker<Category>()
            .RuleFor(category => category.Name, faker => faker.Commerce.Categories(1).First())
            .Generate(30)
            .DistinctBy(category => category.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        categories.ForEach(category =>
        {
            var products = new Faker<Product>()
                .RuleFor(product => product.Image, _ => "product/default.png")
                .RuleFor(product => product.Description, faker => faker.Commerce.ProductDescription())
                .RuleFor(product => product.Price, faker => faker.Random.Double(1, 1_000))
                .RuleFor(product => product.Name, faker => faker.Commerce.ProductName())
                .RuleFor(product => product.Slug, (faker, product) => product.Name.Slugify())
                .Generate(30)
                .DistinctBy(product => product.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            
            category.Products.AddRange(products);
        });
        
        repository.GetEntitySet<Category>().AddRange(categories);

        await repository.SaveChangesAsync();
    }
}