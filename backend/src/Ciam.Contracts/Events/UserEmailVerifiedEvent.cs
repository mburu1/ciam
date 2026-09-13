namespace Ciam.Contracts.Events;

public sealed record UserEmailVerifiedEvent
{
    public Guid UserId { get; init; }
    public DateTimeOffset OccurredOnUtc { get; init; }
}
