using Microsoft.EntityFrameworkCore;
using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Categories.Entities;

namespace TurnDigital.Domain.Features.Categories.Specifications;

public static class CategoriesSpecifications
{
    public static IQueryable<Category> ByName(
        this IQueryable<Category> queryable, string name) =>
        queryable.Where(category => category.Name == name);

    public static IQueryable<Category> ById(this IQueryable<Category> queryable, int id) =>
        queryable.Where(category => category.Id == id);

    public static IQueryable<Category> WithProducts(this IQueryable<Category> queryable) =>
        queryable.Include(category => category.Products);

    public static IQueryable<Category> ByNameExcludeId(
        this IQueryable<Category> queryable, string name, int id) =>
        queryable.Where(category => category.Id != id && category.Name == name);

    public static IQueryable<CategoryDto> ToDto(this IQueryable<Category> queryable) => queryable.Select(category =>
        new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
        });

    public static IOrderedQueryable<Category> OrderByNewest(this IQueryable<Category> queryable) =>
        queryable.OrderByDescending(category => category.Id);
}