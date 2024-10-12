using LanguageExt;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Account.Responses;
using TurnDigital.Application.Responses;

namespace TurnDigital.Application.Features.Account.Commands;

public record Login(string Email, string Password) : ICommand<Either<FailureResponse, LoginResponse>>;