namespace TurnDigital.Web.Security;

public class CorsConfiguration
{
    public IReadOnlyList<string> AllowedOrigins { get; set; } = new List<string>();
}