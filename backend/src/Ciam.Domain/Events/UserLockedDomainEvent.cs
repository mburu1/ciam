namespace Ciam.Domain.Events;

public sealed record UserLockedDomainEvent(Guid UserId, string Reason) : DomainEvent;
