namespace TurnDigital.Domain.Utilities;

public static class ImageUtilities
{
    public static readonly IReadOnlyList<string> AllowedExtensions = new List<string>
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };
}