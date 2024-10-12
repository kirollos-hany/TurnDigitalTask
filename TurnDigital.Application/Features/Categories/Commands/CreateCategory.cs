using LanguageExt;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Responses;
using TurnDigital.Domain.Features.Categories.Dtos;

namespace TurnDigital.Application.Features.Categories.Commands;

public record CreateCategory(string Name) : ICommand<Either<FailureResponse, CategoryDto>>;