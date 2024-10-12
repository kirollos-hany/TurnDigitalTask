using LanguageExt;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Products.Dtos;

namespace TurnDigital.Application.Features.Products.Queries;

public record GetProductBySlug(string Slug) : IQuery<Either<FailureResponse, ProductDto>>;