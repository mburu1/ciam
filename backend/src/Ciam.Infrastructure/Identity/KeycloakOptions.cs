namespace Ciam.Infrastructure.Identity;

public sealed class KeycloakOptions
{
    public const string SectionName = "Keycloak";
    public string BaseUrl { get; init; } = string.Empty;
    public string Realm { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string? AdminClientId { get; init; }
    public string? AdminClientSecret { get; init; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(15);
}
