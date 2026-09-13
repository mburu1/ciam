using System.ComponentModel.DataAnnotations;

namespace Ciam.Contracts.Requests.Auth;

public sealed record RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;

    [MaxLength(256)]
    public string? DeviceName { get; init; }
}
