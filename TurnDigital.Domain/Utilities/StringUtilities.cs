using System.Security.Cryptography;

namespace TurnDigital.Domain.Utilities;

public static class StringUtilities
{
    public static string GenerateRandomString(uint byteCount)
    {
        var randomNumber = new byte[byteCount];
        
        using var rng = RandomNumberGenerator.Create();
        
        rng.GetBytes(randomNumber);
        
        var randomString =  Convert.ToBase64String(randomNumber);

        return randomString;
    }

    public static string Slugify(this string value) => value.Replace(' ', '-');
}