using System.Text.RegularExpressions;

namespace TurnDigital.Application.Common;

public static class Constants
{
    public static class ViewDataKeys
    {
        public const string Title = "Title";

        public const string Message = "Message";
    }

    public static class Messages
    {
        public const string CreateSuccessful = "Create successful";

        public const string UpdateSuccessful = "Update successful";

        public const string DeleteSuccessful = "Delete successsful";
    }
    
    public static class ValidationMessages
    {
        public static class Messages
        {
            public const string NameRequired = "Name is required";

            public const string NameExists = "Name already exists";

            public const string NameFormat = "Name can't contain symbols, only alphabet letters and digits";
        }
        
        public static class CategoryMessages
        {
            public const string CategoryNotFound = "Category not found";
        }
        
        public static class ProductMessages
        {
            public const string ProductNotFound = "Product not found";

            public const string PriceCantBeEqualToZero = "Price must be greater than 0";

            public const string ImageRequired = "Product image is required";

            public const string ImageExtensionInvalid = "Allowed images are jpg, jpeg, webp and png";
        }
        
        public static class UserMessages
        {
            public const string UserNotFound = "Account couldn't be found.";
        }

        public static class EmailValidation
        {
            public const string Required = "Email is required.";

            public const string Format = "Email is invalid.";

            public const string NotRegistered = "Email provided is not registered.";
        }

        public static class PasswordValidation
        {
            public const string Required = "Password is required.";

            public const string Format =
                "Password must be at least 8 characters, contains a symbol, uppercase and a number";

            public const string Incorrect = "Password provided is incorrect.";

            public const string PasswordsNotMatch = "Password and Confirm Password doesn't match";
        }
        
        public static class PhoneNumberValidation
        {
            public const string Required = "Phone number is required.";

            public const string Format =
                "Phone number must be of format: +(countryCode)(phoneNumber)";

            public const string NotRegistered = "Phone number provided is not registered.";
        }

        public static class RefreshTokenValidation
        {
            public const string Invalid = "Refresh token is invalid";
        }
    }

    public static class ValidationRegex
    {
        // regex for minimum length of 8, a symbol, uppercase and a number
        public static readonly Regex Password = new ("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$");

        // regex to match phone number with format: +(countryCode)(phoneNumber) example: +201201523156
        public static readonly Regex PhoneNumber = new("^\\+\\d{1,4}\\d{9,15}$");

        public static readonly Regex StringNoSymbols = new("^[a-zA-Z0-9\\s]+$");
    }
    

    public static class ResponseMessages
    {
        public const string InternalServerErrorMessage = "Something went wrong. we're working on fixing it asap!" ;
    }
    
    public static class TurnDigitalAdmin
    {
        public const string Email = "admin@turndigital.com";

        public const string Password = "Admin#123";

        public const string UserName = "TurnDigitalAdmin";

        public const string DisplayName = "TurnDigital Administrator";
    }
    
    public static class AuthenticationSchemes
    {
        public const string JwtOrCookies = "JWT_OR_COOKIES";
    }
}