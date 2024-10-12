using LanguageExt;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Responses;
using Unit = MediatR.Unit;

namespace TurnDigital.Application.Features.Products.Commands;

public record DeleteProduct(int Id) : ICommand<Either<FailureResponse, Unit>>;