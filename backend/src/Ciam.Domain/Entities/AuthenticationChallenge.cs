using Ciam.Domain.Common;
using Ciam.Domain.Enums;
using Ciam.Domain.Events;
using Ciam.Domain.Exceptions;

namespace Ciam.Domain.Entities;

public sealed class AuthenticationChallenge : AggregateRoot
{
    private AuthenticationChallenge(
        Guid id,
        Guid userId,
        ChallengePurpose purpose,
        string challengeCode,
        DateTimeOffset expiresAtUtc)
        : base(id)
    {
        UserId = userId;
        Purpose = purpose;
        ChallengeCode = challengeCode;
        ExpiresAtUtc = expiresAtUtc;
        Status = ChallengeStatus.Pending;
        AttemptCount = 0;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid UserId { get; private set; }
    public ChallengePurpose Purpose { get; private set; }
    public string ChallengeCode { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public ChallengeStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public static AuthenticationChallenge Create(
        Guid challengeId,
        Guid userId,
        ChallengePurpose purpose,
        string challengeCode,
        DateTimeOffset expiresAtUtc)
    {
        if (challengeId == Guid.Empty)
        {
            throw new InvalidSessionException("Challenge id cannot be empty.");
        }

        if (userId == Guid.Empty)
        {
            throw new InvalidSessionException("User id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(challengeCode))
        {
            throw new InvalidSessionException("Challenge code is required.");
        }

        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new AuthenticationChallengeExpiredException("Challenge expiry must be in the future.");
        }

        return new AuthenticationChallenge(challengeId, userId, purpose, challengeCode, expiresAtUtc);
    }

    public void Verify(string providedCode)
    {
        if (Status == ChallengeStatus.Completed)
        {
            return;
        }

        if (IsExpired())
        {
            Status = ChallengeStatus.Expired;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
            throw new AuthenticationChallengeExpiredException("This challenge has expired.");
        }

        if (!string.Equals(providedCode.Trim(), ChallengeCode.Trim(), StringComparison.Ordinal))
        {
            AttemptCount += 1;
            Status = ChallengeStatus.Failed;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
            throw new InvalidSessionException("The provided challenge code is invalid.");
        }

        Status = ChallengeStatus.Completed;
        CompletedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new AuthenticationChallengeCompletedDomainEvent(UserId, Id, Purpose.ToString()));
    }

    public void Expire()
    {
        if (Status == ChallengeStatus.Completed)
        {
            return;
        }

        Status = ChallengeStatus.Expired;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public bool IsExpired() => ExpiresAtUtc <= DateTimeOffset.UtcNow;
}
