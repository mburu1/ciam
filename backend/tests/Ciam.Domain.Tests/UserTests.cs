using Ciam.Domain.Entities;
using Ciam.Domain.Enums;
using Ciam.Domain.Events;
using Ciam.Domain.Exceptions;
using Ciam.Domain.ValueObjects;

namespace Ciam.Domain.Tests;

public sealed class UserTests
{
    [Fact]
    public void Create_sets_pending_status_customer_role_and_registration_event()
    {
        var user = CreateUser();

        Assert.Equal(AccountStatus.PendingVerification, user.Status);
        Assert.Contains(UserRole.Customer, user.Roles);
        Assert.Contains(AuthenticationMethod.UsernamePassword, user.AuthenticationMethods);
        Assert.Contains(user.DomainEvents, domainEvent => domainEvent is UserRegisteredDomainEvent);
    }

    [Fact]
    public void VerifyEmail_activates_pending_user_and_adds_event()
    {
        var user = CreateUser();
        user.ClearDomainEvents();

        user.VerifyEmail();

        Assert.True(user.EmailVerified);
        Assert.Equal(AccountStatus.Active, user.Status);
        Assert.Contains(user.DomainEvents, domainEvent => domainEvent is UserEmailVerifiedDomainEvent);
    }

    [Fact]
    public void EnableAndDisableMfa_updates_authentication_methods()
    {
        var user = CreateUser();

        user.EnableMfa(MfaMethod.Totp);
        Assert.Contains(MfaMethod.Totp, user.MfaMethods);
        Assert.Contains(AuthenticationMethod.Totp, user.AuthenticationMethods);

        user.DisableMfa(MfaMethod.Totp);
        Assert.DoesNotContain(MfaMethod.Totp, user.MfaMethods);
        Assert.DoesNotContain(AuthenticationMethod.Totp, user.AuthenticationMethods);
    }

    [Fact]
    public void Lock_requires_reason_and_unlock_restores_verified_status()
    {
        var user = CreateUser();
        user.VerifyEmail();

        Assert.Throws<InvalidOperationException>(() => user.Lock(" "));

        user.Lock("Suspicious activity");
        Assert.True(user.IsLocked);

        user.Unlock();
        Assert.Equal(AccountStatus.Active, user.Status);
    }

    [Fact]
    public void VerifyEmail_rejects_disabled_user()
    {
        var user = CreateUser();
        user.Disable();

        Assert.Throws<UserAccountLockedException>(() => user.VerifyEmail());
    }

    private static User CreateUser() =>
        User.Create(
            Guid.NewGuid(),
            FullName.Create("Ada", "Lovelace"),
            EmailAddress.Create("ada@example.com"),
            PreferredUsername.Create("ada.lovelace"));
}
