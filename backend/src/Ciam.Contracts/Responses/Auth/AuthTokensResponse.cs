namespace Ciam.Contracts.Responses.Auth;

public sealed record AuthTokensResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
    public DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }
    public string TokenType { get; init; } = "Bearer";
}
