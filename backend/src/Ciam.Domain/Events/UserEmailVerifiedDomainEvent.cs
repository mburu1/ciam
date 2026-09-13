namespace Ciam.Domain.Events;

public sealed record UserEmailVerifiedDomainEvent(Guid UserId) : DomainEvent;
