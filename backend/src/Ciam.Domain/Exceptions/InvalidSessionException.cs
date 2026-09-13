namespace Ciam.Domain.Exceptions;

public sealed class InvalidSessionException : DomainException
{
    public InvalidSessionException(string message)
        : base(message)
    {
    }
}
