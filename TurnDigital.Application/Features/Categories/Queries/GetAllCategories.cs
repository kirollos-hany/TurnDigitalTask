using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Categories.Dtos;

namespace TurnDigital.Application.Features.Categories.Queries;

public record GetAllCategories() : IQuery<CollectionWrapper<CategoryDto>>;