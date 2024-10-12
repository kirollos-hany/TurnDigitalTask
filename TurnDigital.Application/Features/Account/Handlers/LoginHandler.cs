using FluentValidation;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Common.Interfaces;
using TurnDigital.Application.Features.Account.Commands;
using TurnDigital.Application.Features.Account.Responses;
using TurnDigital.Application.Responses;
using TurnDigital.Application.Validations.Mapping;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.Security.Interfaces;
using TurnDigital.Domain.Security.Specifications;

namespace TurnDigital.Application.Features.Account.Handlers;

internal sealed class LoginHandler : ICommandHandler<Login, Either<FailureResponse, LoginResponse>>
{
    private readonly IRepository _repository;

    private readonly IClaimsProvider _claimsProvider;

    private readonly IJwtProvider _jwtProvider;

    private readonly UserManager<User> _userManager;

    private readonly ICookieAuthenticationService _cookieAuthenticationService;

    private readonly IValidator<Login> _validator;

    public LoginHandler(IRepository repository, IClaimsProvider claimsProvider, IJwtProvider jwtProvider,
        UserManager<User> userManager, ICookieAuthenticationService cookieAuthenticationService, IValidator<Login> validator)
    {
        _repository = repository;
        _claimsProvider = claimsProvider;
        _jwtProvider = jwtProvider;
        _userManager = userManager;
        _cookieAuthenticationService = cookieAuthenticationService;
        _validator = validator;
    }

    public async Task<Either<FailureResponse, LoginResponse>> Handle(Login request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResponse();
        }
        
        var user = await _repository.GetEntitySet<User>().ByEmail(request.Email).FirstAsync(cancellationToken);

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            user.Login(null);

            await _repository.SaveChangesAsync(cancellationToken);

            return FailureResponse<ValidationFailureResponse>.ValidationFailure(new ValidationFailureResponse(
                new Dictionary<string, string>
                    { { nameof(request.Password), Constants.ValidationMessages.PasswordValidation.Incorrect } }));
        }

        var claims = (await _claimsProvider.GetClaimsAsync(user)).ToList();

        var jwt = _jwtProvider.GenerateAccessToken(claims);

        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.Login(refreshToken);

        await _cookieAuthenticationService.SignInAsync(claims);

        return new LoginResponse
        {
            AccessToken = jwt.token,
            RefreshToken = refreshToken,
            AccessTokenExpiration = jwt.expiration
        };
    }
}