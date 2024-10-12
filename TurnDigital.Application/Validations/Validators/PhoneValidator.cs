using FluentValidation;
using TurnDigital.Application.Common.Interfaces;

namespace TurnDigital.Application.Validations.Validators;

public class PhoneValidator : AbstractValidator<IHasPhone>
{
    public PhoneValidator()
    {
        RuleFor(cmd => cmd.Phone)
            .ValidatePhoneNumber();
    }
}