using System.ComponentModel.DataAnnotations;

namespace Ciam.Contracts.Requests.Auth;

public sealed record RegisterUserRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(3)]
    [MaxLength(64)]
    public string PreferredUsername { get; init; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; init; }

    [MaxLength(20)]
    public string? Locale { get; init; }
}
