namespace Ciam.Domain.Exceptions;

public sealed class AuthenticationChallengeExpiredException : DomainException
{
    public AuthenticationChallengeExpiredException(string message)
        : base(message)
    {
    }
}
