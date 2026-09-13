namespace Ciam.Domain.Events;

public sealed record AuthenticationChallengeCompletedDomainEvent(Guid UserId, Guid ChallengeId, string Purpose) : DomainEvent;
