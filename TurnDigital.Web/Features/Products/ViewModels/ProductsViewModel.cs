using TurnDigital.Domain.Features.Categories.Dtos;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Query.Pagination;

namespace TurnDigital.Web.Features.Products.ViewModels;

public class ProductsViewModel
{
    public required PaginatedResult<ProductDto> Products { get; init; }
    
    public required IReadOnlyList<CategoryDto> Categories { get; init; }

    public string? Name { get; set; }
    
    public int? CategoryId { get; set; }
}