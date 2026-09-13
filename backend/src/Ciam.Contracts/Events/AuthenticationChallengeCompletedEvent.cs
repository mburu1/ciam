namespace Ciam.Contracts.Events;

public sealed record AuthenticationChallengeCompletedEvent
{
    public Guid UserId { get; init; }
    public Guid ChallengeId { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public DateTimeOffset OccurredOnUtc { get; init; }
}
