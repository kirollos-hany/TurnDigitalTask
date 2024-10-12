using FluentValidation;
using TurnDigital.Application.Common.Interfaces;

namespace TurnDigital.Application.Validations.Validators;

public class EmailValidator : AbstractValidator<IHasEmail>
{
    public EmailValidator()
    {
        RuleFor(cmd => cmd.Email)
            .ValidateEmail();
    }
}