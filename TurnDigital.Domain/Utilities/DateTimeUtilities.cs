namespace TurnDigital.Domain.Utilities;

public static class DateTimeUtilities
{
    public static string FormatDate(this DateTime value)
    {
        return value.ToShortDateString();
    }
}