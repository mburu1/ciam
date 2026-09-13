namespace Ciam.Domain.Events;

public sealed record UserSessionCreatedDomainEvent(Guid UserId, Guid SessionId, DateTimeOffset ExpiresAtUtc) : DomainEvent;
