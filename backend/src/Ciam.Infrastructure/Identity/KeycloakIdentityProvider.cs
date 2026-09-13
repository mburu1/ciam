using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Ciam.Application.Abstractions.Identity;
using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Responses.Auth;
using Microsoft.Extensions.Options;

namespace Ciam.Infrastructure.Identity;

public sealed class KeycloakIdentityProvider(HttpClient httpClient, IOptions<KeycloakOptions> options)
    : IIdentityProvider
{
    private readonly KeycloakOptions _options = options.Value;

    public async Task<IdentityRegistrationResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            AdminPath("users"),
            new
            {
                username = request.PreferredUsername,
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName,
                enabled = true,
                emailVerified = false
            },
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var subject = response.Headers.Location?.Segments.LastOrDefault()
            ?? throw new InvalidOperationException("Keycloak did not return the created user location.");
        return new IdentityRegistrationResult(Guid.Parse(subject), null);
    }

    public Task<AuthTokensResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default) =>
        RequestTokensAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["username"] = request.UsernameOrEmail,
                ["password"] = request.Password,
                ["scope"] = "openid"
            },
            cancellationToken);

    public Task<AuthTokensResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default) =>
        RequestTokensAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["refresh_token"] = request.RefreshToken
            },
            cancellationToken);

    public async Task SendEmailVerificationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            AdminPath($"users/{userId:D}/execute-actions-email"),
            new[] { "VERIFY_EMAIL" },
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public Task VerifyEmailAsync(Guid userId, string code, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException(
            "Keycloak verifies email-action links. Verification codes require an application-owned challenge flow.");

    private async Task<AuthTokensResponse> RequestTokensAsync(
        Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsync(
            TokenPath(),
            new FormUrlEncodedContent(values),
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Keycloak returned an empty token response.");
        var now = DateTimeOffset.UtcNow;

        return new AuthTokensResponse
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            AccessTokenExpiresAtUtc = now.AddSeconds(token.ExpiresIn),
            RefreshTokenExpiresAtUtc = now.AddSeconds(token.RefreshExpiresIn)
        };
    }

    private string TokenPath() => $"realms/{Uri.EscapeDataString(_options.Realm)}/protocol/openid-connect/token";

    private string AdminPath(string path) =>
        $"admin/realms/{Uri.EscapeDataString(_options.Realm)}/{path.TrimStart('/')}";

    private sealed record KeycloakTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("refresh_expires_in")] int RefreshExpiresIn);
}
