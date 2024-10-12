using FluentValidation;
using TurnDigital.Application.Common;
using TurnDigital.Domain.IO;
using TurnDigital.Domain.Utilities;

namespace TurnDigital.Application.Validations.Validators;

internal static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> ValidateEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .ValidateRequiredString(Constants.ValidationMessages.EmailValidation.Required)
            .EmailAddress()
            .WithMessage(Constants.ValidationMessages.EmailValidation.Format);
    }

    public static IRuleBuilderOptions<T, double> ValidatePrice<T>(this IRuleBuilder<T, double> ruleBuilder,
        string name = "")
    {
        return string.IsNullOrEmpty(name)
            ? ruleBuilder
                .GreaterThan(0)
                .WithMessage(Constants.ValidationMessages.ProductMessages.PriceCantBeEqualToZero)
            : ruleBuilder
                .GreaterThan(0)
                .WithName(name)
                .WithMessage(Constants.ValidationMessages.ProductMessages.PriceCantBeEqualToZero);
    }

    public static IRuleBuilderOptions<T, double?> ValidatePrice<T>(this IRuleBuilder<T, double?> ruleBuilder)
    {
        return ruleBuilder
            .NotNull()
            .GreaterThan(0)
            .WithMessage(Constants.ValidationMessages.ProductMessages.PriceCantBeEqualToZero);
    }

    public static IRuleBuilderOptions<T, FileModel> ValidateImage<T>(this IRuleBuilder<T, FileModel> ruleBuilder)
    {
        return ruleBuilder
            .NotNull()
            .WithMessage(Constants.ValidationMessages.ProductMessages.ImageRequired)
            .DependentRules(() =>
            {
                ruleBuilder
                    .Must(file =>
                    {
                        var fileInfo = new FileInfo(file.Name);

                        return ImageUtilities.AllowedExtensions.Contains(fileInfo.Extension,
                            StringComparer.OrdinalIgnoreCase);
                    })
                    .WithMessage(Constants.ValidationMessages.ProductMessages.ImageExtensionInvalid);
            });
    }

    public static IRuleBuilderOptions<T, string> ValidatePassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .ValidateRequiredString(Constants.ValidationMessages.PasswordValidation.Required)
            .Matches(Constants.ValidationRegex.Password)
            .WithMessage(Constants.ValidationMessages.PasswordValidation.Format);
    }

    public static IRuleBuilderOptions<T, string> ValidatePhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .ValidateRequiredString(Constants.ValidationMessages.PhoneNumberValidation.Required)
            .Matches(Constants.ValidationRegex.PhoneNumber)
            .WithMessage(Constants.ValidationMessages.PhoneNumberValidation.Format);
    }

    public static IRuleBuilderOptions<T, string> ValidateRequiredString<T>(this IRuleBuilder<T, string> ruleBuilder,
        string errorMessage)
    {
        return ruleBuilder
            .NotEmpty()
            .NotNull()
            .WithMessage(errorMessage);
    }
}