using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Application.Common;
using TurnDigital.Application.Features.Account.Commands;
using TurnDigital.Application.Validations.Validators;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Security.Entities;
using TurnDigital.Domain.Security.Specifications;

namespace TurnDigital.Application.Features.Account.Validators;

public class LoginValidator : AbstractValidator<Login>
{
    public LoginValidator(IReadRepository repository)
    {
        RuleFor(cmd => cmd.Email)
            .ValidateEmail()
            .DependentRules(() =>
            {
                RuleFor(cmd => cmd.Email)
                    .MustAsync(async (email, cancellationToken) =>
                        await repository.GetQueryable<User>().ByEmail(email).AnyAsync(cancellationToken))
                    .WithMessage(Constants.ValidationMessages.EmailValidation.NotRegistered);
            });
    }
}