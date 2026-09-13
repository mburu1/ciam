using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Responses.Auth;

namespace Ciam.Application.Abstractions.Identity;

public interface IIdentityProvider
{
    Task<IdentityRegistrationResult> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthTokensResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthTokensResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    Task SendEmailVerificationAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task VerifyEmailAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default);
}

public sealed record IdentityRegistrationResult(Guid Subject, string? VerificationChallengeId);
