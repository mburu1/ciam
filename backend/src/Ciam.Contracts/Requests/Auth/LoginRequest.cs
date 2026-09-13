using System.ComponentModel.DataAnnotations;

namespace Ciam.Contracts.Requests.Auth;

public sealed record LoginRequest
{
    [Required]
    [MaxLength(254)]
    public string UsernameOrEmail { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;

    [MaxLength(256)]
    public string? DeviceName { get; init; }

    public bool RememberMe { get; init; }
}
