using Ciam.Domain.ValueObjects;

namespace Ciam.Domain.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId, EmailAddress Email, string DisplayName) : DomainEvent;
