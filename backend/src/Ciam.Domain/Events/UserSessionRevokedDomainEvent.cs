namespace Ciam.Domain.Events;

public sealed record UserSessionRevokedDomainEvent(Guid UserId, Guid SessionId) : DomainEvent;
