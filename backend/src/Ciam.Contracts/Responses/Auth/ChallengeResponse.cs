namespace Ciam.Contracts.Responses.Auth;

public sealed record ChallengeResponse
{
    public Guid ChallengeId { get; init; }
    public Guid UserId { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAtUtc { get; init; }
    public string? DeliveryChannel { get; init; }
}
