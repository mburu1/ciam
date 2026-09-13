using Ciam.Domain.Common;
using Ciam.Domain.Enums;
using Ciam.Domain.Events;
using Ciam.Domain.Exceptions;
using Ciam.Domain.ValueObjects;

namespace Ciam.Domain.Entities;

public sealed class User : AggregateRoot
{
    private readonly HashSet<UserRole> _roles = [];
    private readonly HashSet<MfaMethod> _mfaMethods = [];
    private readonly HashSet<AuthenticationMethod> _authenticationMethods = [];

    private User()
        : base()
    {
        FullName = null!;
        Email = null!;
        PreferredUsername = null!;
    }

    private User(Guid id, FullName fullName, EmailAddress email, PreferredUsername preferredUsername)
        : base(id)
    {
        FullName = fullName;
        Email = email;
        PreferredUsername = preferredUsername;
        Status = AccountStatus.PendingVerification;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
        _roles.Add(UserRole.Customer);
        _authenticationMethods.Add(AuthenticationMethod.UsernamePassword);
        AddDomainEvent(new UserRegisteredDomainEvent(id, email, fullName.DisplayName));
    }

    public FullName FullName { get; private set; }
    public EmailAddress Email { get; private set; }
    public PreferredUsername PreferredUsername { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public AccountStatus Status { get; private set; }
    public bool EmailVerified { get; private set; }
    public bool IsLocked => Status == AccountStatus.Locked;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public string? Locale { get; private set; }
    public string? Subject { get; private set; }
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<MfaMethod> MfaMethods => _mfaMethods.AsReadOnly();
    public IReadOnlyCollection<AuthenticationMethod> AuthenticationMethods => _authenticationMethods.AsReadOnly();

    public static User Create(
        Guid id,
        FullName fullName,
        EmailAddress email,
        PreferredUsername preferredUsername,
        string? locale = null,
        string? subject = null)
    {
        ArgumentNullException.ThrowIfNull(fullName);
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(preferredUsername);

        var user = new User(id, fullName, email, preferredUsername)
        {
            Locale = locale,
            Subject = subject
        };

        return user;
    }

    public void UpdateProfile(FullName fullName, PhoneNumber? phoneNumber = null, string? locale = null)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        PhoneNumber = phoneNumber;
        Locale = locale;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void UpdatePreferredUsername(PreferredUsername username)
    {
        PreferredUsername = username ?? throw new ArgumentNullException(nameof(username));
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void VerifyEmail()
    {
        if (Status == AccountStatus.Disabled)
        {
            throw new UserAccountLockedException("Cannot verify email for a disabled account.");
        }

        EmailVerified = true;
        if (Status == AccountStatus.PendingVerification)
        {
            Status = AccountStatus.Active;
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new UserEmailVerifiedDomainEvent(Id));
    }

    public void AddRole(UserRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        _roles.Add(role);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void RemoveRole(UserRole role)
    {
        _roles.Remove(role);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void EnableMfa(MfaMethod method)
    {
        if (!Enum.IsDefined(method))
        {
            throw new ArgumentOutOfRangeException(nameof(method));
        }

        _mfaMethods.Add(method);
        _authenticationMethods.Add(method switch
        {
            MfaMethod.Totp => AuthenticationMethod.Totp,
            MfaMethod.EmailOtp => AuthenticationMethod.EmailOtp,
            MfaMethod.WebAuthn => AuthenticationMethod.Passkey,
            _ => throw new InvalidOperationException($"Unsupported MFA method: {method}")
        });

        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void DisableMfa(MfaMethod method)
    {
        _mfaMethods.Remove(method);

        var authenticationMethod = method switch
        {
            MfaMethod.Totp => AuthenticationMethod.Totp,
            MfaMethod.EmailOtp => AuthenticationMethod.EmailOtp,
            MfaMethod.WebAuthn => AuthenticationMethod.Passkey,
            _ => throw new InvalidOperationException($"Unsupported MFA method: {method}")
        };

        _authenticationMethods.Remove(authenticationMethod);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Lock(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new InvalidOperationException("Lock reason cannot be empty.");
        }

        Status = AccountStatus.Locked;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new UserLockedDomainEvent(Id, reason));
    }

    public void Unlock()
    {
        if (Status != AccountStatus.Locked)
        {
            return;
        }

        Status = EmailVerified ? AccountStatus.Active : AccountStatus.PendingVerification;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new InvalidOperationException("Suspension reason cannot be empty.");
        }

        Status = AccountStatus.Suspended;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Resume()
    {
        if (Status != AccountStatus.Suspended)
        {
            return;
        }

        Status = EmailVerified ? AccountStatus.Active : AccountStatus.PendingVerification;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Disable()
    {
        Status = AccountStatus.Disabled;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void SetSubject(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new InvalidOperationException("Subject cannot be empty.");
        }

        Subject = subject;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void RecordSuccessfulLogin()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
