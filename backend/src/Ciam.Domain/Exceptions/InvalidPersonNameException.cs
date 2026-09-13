namespace Ciam.Domain.Exceptions;

public sealed class InvalidPersonNameException : DomainException
{
    public InvalidPersonNameException(string message)
        : base(message)
    {
    }
}
