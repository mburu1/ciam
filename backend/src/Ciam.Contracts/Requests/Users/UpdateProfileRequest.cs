using System.ComponentModel.DataAnnotations;

namespace Ciam.Contracts.Requests.Users;

public sealed record UpdateProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; init; }

    [MaxLength(20)]
    public string? Locale { get; init; }
}
