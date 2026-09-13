using System.ComponentModel.DataAnnotations;

namespace Ciam.Contracts.Requests.Auth;

public sealed record VerifyEmailRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    [MinLength(6)]
    [MaxLength(12)]
    public string Code { get; init; } = string.Empty;
}
