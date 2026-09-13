namespace Ciam.Domain.Exceptions;

public sealed class InvalidPhoneNumberException : DomainException
{
    public InvalidPhoneNumberException(string message)
        : base(message)
    {
    }
}
