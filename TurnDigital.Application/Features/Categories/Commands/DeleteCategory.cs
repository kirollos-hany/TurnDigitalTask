using LanguageExt;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Responses;

namespace TurnDigital.Application.Features.Categories.Commands;

public record DeleteCategory(int Id) : ICommand<Either<FailureResponse, Unit>>;