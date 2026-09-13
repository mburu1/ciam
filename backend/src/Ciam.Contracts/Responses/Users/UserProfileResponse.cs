namespace Ciam.Contracts.Responses.Users;

public sealed record UserProfileResponse
{
    public Guid UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PreferredUsername { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Locale { get; init; }
    public bool EmailVerified { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; }
}
