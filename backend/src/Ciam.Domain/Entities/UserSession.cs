using Ciam.Domain.Common;
using Ciam.Domain.Enums;
using Ciam.Domain.Events;
using Ciam.Domain.Exceptions;

namespace Ciam.Domain.Entities;

public sealed class UserSession : AggregateRoot
{
    private UserSession(
        Guid id,
        Guid userId,
        string sessionTokenHash,
        string refreshTokenHash,
        DateTimeOffset expiresAtUtc)
        : base(id)
    {
        UserId = userId;
        SessionTokenHash = sessionTokenHash;
        RefreshTokenHash = refreshTokenHash;
        StartedAtUtc = DateTimeOffset.UtcNow;
        LastSeenAtUtc = StartedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        Status = SessionStatus.Active;
        CreatedAtUtc = StartedAtUtc;
        UpdatedAtUtc = StartedAtUtc;
    }

    public Guid UserId { get; private set; }
    public string SessionTokenHash { get; private set; }
    public string RefreshTokenHash { get; private set; }
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset LastSeenAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public SessionStatus Status { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static UserSession Start(
        Guid sessionId,
        Guid userId,
        string sessionTokenHash,
        string refreshTokenHash,
        DateTimeOffset expiresAtUtc,
        string? ipAddress = null,
        string? userAgent = null)
    {
        if (sessionId == Guid.Empty)
        {
            throw new InvalidSessionException("Session id cannot be empty.");
        }

        if (userId == Guid.Empty)
        {
            throw new InvalidSessionException("User id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(sessionTokenHash))
        {
            throw new InvalidSessionException("Session token hash is required.");
        }

        if (string.IsNullOrWhiteSpace(refreshTokenHash))
        {
            throw new InvalidSessionException("Refresh token hash is required.");
        }

        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new InvalidSessionException("Session expiry time must be in the future.");
        }

        var session = new UserSession(sessionId, userId, sessionTokenHash, refreshTokenHash, expiresAtUtc)
        {
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        session.AddDomainEvent(new UserSessionCreatedDomainEvent(userId, sessionId, expiresAtUtc));
        return session;
    }

    public void Refresh(DateTimeOffset newExpiresAtUtc)
    {
        if (newExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new InvalidSessionException("New session expiry must be in the future.");
        }

        ExpiresAtUtc = newExpiresAtUtc;
        LastSeenAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Touch()
    {
        if (Status != SessionStatus.Active)
        {
            throw new InvalidSessionException("Only active sessions can be touched.");
        }

        LastSeenAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Revoke(string? reason = null)
    {
        if (Status == SessionStatus.Revoked)
        {
            return;
        }

        Status = SessionStatus.Revoked;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new UserSessionRevokedDomainEvent(UserId, Id));
    }

    public void Expire()
    {
        if (Status == SessionStatus.Revoked)
        {
            return;
        }

        Status = SessionStatus.Expired;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public bool IsExpired() => Status == SessionStatus.Expired || ExpiresAtUtc <= DateTimeOffset.UtcNow;
}
