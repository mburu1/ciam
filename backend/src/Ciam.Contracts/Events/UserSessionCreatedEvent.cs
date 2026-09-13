namespace Ciam.Contracts.Events;

public sealed record UserSessionCreatedEvent
{
    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }
    public string SessionType { get; init; } = "Bearer";
    public DateTimeOffset ExpiresAtUtc { get; init; }
    public DateTimeOffset OccurredOnUtc { get; init; }
}
