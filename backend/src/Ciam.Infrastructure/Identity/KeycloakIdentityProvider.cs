using System.Net.Http.Json;
using System.Net;
using System.Text.Json.Serialization;
using Ciam.Application.Abstractions.Identity;
using Ciam.Application.Common.Exceptions;
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
        using var adminRequest = new HttpRequestMessage(HttpMethod.Post, AdminPath("users"))
        {
            Content = JsonContent.Create(new
            {
                username = request.PreferredUsername,
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName,
                enabled = true,
                emailVerified = false
            })
        };
        adminRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                await GetAdminAccessTokenAsync(cancellationToken));

        using var response = await httpClient.SendAsync(adminRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var providerReason = await ReadProviderReasonAsync(response, cancellationToken);
            var statusCode = IsConflict(providerReason)
                ? HttpStatusCode.Conflict
                : response.StatusCode is >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.BadGateway;

            throw new IdentityProviderException("registration", statusCode, providerReason);
        }

        var subject = response.Headers.Location?.Segments.LastOrDefault()?.Trim('/')
            ?? throw new InvalidOperationException("Keycloak did not return the created user location.");
        if (!Guid.TryParse(subject, out var subjectId))
        {
            throw new InvalidOperationException("Keycloak returned an invalid created user identifier.");
        }

        return new IdentityRegistrationResult(subjectId, null);
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
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            AdminPath($"users/{userId:D}/execute-actions-email"))
        {
            Content = JsonContent.Create(new[] { "VERIFY_EMAIL" })
        };
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                await GetAdminAccessTokenAsync(cancellationToken));
        using var response = await httpClient.SendAsync(request, cancellationToken);
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

    private async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken)
    {
        var clientId = string.IsNullOrWhiteSpace(_options.AdminClientId)
            ? _options.ClientId
            : _options.AdminClientId;
        var clientSecret = string.IsNullOrWhiteSpace(_options.AdminClientSecret)
            ? _options.ClientSecret
            : _options.AdminClientSecret;

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new InvalidOperationException(
                "Keycloak admin client credentials are required for user provisioning.");
        }

        using var response = await httpClient.PostAsync(
            TokenPath(),
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret
            }),
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Keycloak returned an empty admin token response.");
        return token.AccessToken;
    }

    private string TokenPath() => $"realms/{Uri.EscapeDataString(_options.Realm)}/protocol/openid-connect/token";

    private string AdminPath(string path) =>
        $"admin/realms/{Uri.EscapeDataString(_options.Realm)}/{path.TrimStart('/')}";

    private static async Task<string?> ReadProviderReasonAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(body))
        {
            return $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase})";
        }

        return body.Length <= 512 ? body : body[..512];
    }

    private static bool IsConflict(string? providerReason) =>
        providerReason?.Contains("exist", StringComparison.OrdinalIgnoreCase) == true ||
        providerReason?.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
        providerReason?.Contains("already", StringComparison.OrdinalIgnoreCase) == true;

    private sealed record KeycloakTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("refresh_expires_in")] int RefreshExpiresIn);
}
