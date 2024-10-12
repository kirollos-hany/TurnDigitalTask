using Microsoft.EntityFrameworkCore;
using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Features.Products.Entities;

namespace TurnDigital.Domain.Features.Products.Specifications;

public static class ProductsSpecifications
{
    public static IQueryable<Product> ById(this IQueryable<Product> queryable, int id) =>
        queryable.Where(product => product.Id == id);
    
    public static IQueryable<Product> WithCategory(this IQueryable<Product> queryable) =>
        queryable.Include(product => product.Category);

    public static IQueryable<Product> ByName(this IQueryable<Product> queryable, string name) =>
        queryable.Where(product => product.Name == name);

    public static IQueryable<Product> ByNameAndSelfCategoryIdExcludeId(this IQueryable<Product> queryable, string name,
        int productId) =>
        queryable.Where(product => product.Id != productId && product.Name == name &&
                                   product.CategoryId == queryable.ById(productId).Select(p => p.CategoryId).FirstOrDefault());

    public static IQueryable<Product> NameIncludes(this IQueryable<Product> queryable, string name) =>
        queryable.Where(product => product.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

    public static IQueryable<Product> ByNameExcludeId(this IQueryable<Product> queryable, string name, int id) =>
        queryable.Where(product => product.Id != id && product.Name == name);

    public static IQueryable<Product> BySlug(this IQueryable<Product> queryable, string slug) =>
        queryable.Where(product => product.Slug == slug);

    public static IQueryable<Product> ByCategoryId(this IQueryable<Product> queryable, int categoryId) =>
        queryable.Where(product => product.CategoryId == categoryId);

    public static IQueryable<ProductDto> ToDto(this IQueryable<Product> queryable) => queryable.Select(product =>
        new ProductDto
        {
            Category = new CategoryDto
            {
                Id = product.CategoryId,
                Name = product.Category.Name
            },
            Description = product.Description,
            Id = product.Id,
            Image = product.Image,
            Name = product.Name,
            Price = product.Price,
            UtcCreatedDate = product.UtcCreatedDate,
            Slug = product.Slug
        });

    public static IOrderedQueryable<Product> OrderByNewest(this IQueryable<Product> queryable) =>
        queryable.OrderByDescending(product => product.UtcCreatedDate);
}