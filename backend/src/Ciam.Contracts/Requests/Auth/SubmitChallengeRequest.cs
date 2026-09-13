using System.ComponentModel.DataAnnotations;

namespace Ciam.Contracts.Requests.Auth;

public sealed record SubmitChallengeRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid ChallengeId { get; init; }

    [Required]
    [MinLength(4)]
    [MaxLength(12)]
    public string Code { get; init; } = string.Empty;
}
