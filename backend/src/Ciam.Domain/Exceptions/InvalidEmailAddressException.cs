namespace Ciam.Domain.Exceptions;

public sealed class InvalidEmailAddressException : DomainException
{
    public InvalidEmailAddressException(string message)
        : base(message)
    {
    }
}
