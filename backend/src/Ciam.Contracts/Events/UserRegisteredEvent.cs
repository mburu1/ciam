namespace Ciam.Contracts.Events;

public sealed record UserRegisteredEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public DateTimeOffset OccurredOnUtc { get; init; }
}
