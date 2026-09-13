namespace Ciam.Domain.Exceptions;

public sealed class UserAccountLockedException : DomainException
{
    public UserAccountLockedException(string message)
        : base(message)
    {
    }
}
