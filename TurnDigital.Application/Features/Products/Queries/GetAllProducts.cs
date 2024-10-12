using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Domain.Features.Products.Dtos;
using TurnDigital.Domain.Query.Pagination;

namespace TurnDigital.Application.Features.Products.Queries;

public record GetAllProducts
(int? CategoryId, string? Name,
    PaginationParameters PaginationParameters) : IQuery<PaginatedResult<ProductDto>>;