namespace TurnDigital.Domain.Utilities;

public static class NumberUtilities
{
    public static string FormatDouble(this double value) => value.ToString("F1");
}