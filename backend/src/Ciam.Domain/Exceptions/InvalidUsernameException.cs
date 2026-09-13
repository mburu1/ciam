namespace Ciam.Domain.Exceptions;

public sealed class InvalidUsernameException : DomainException
{
    public InvalidUsernameException(string message)
        : base(message)
    {
    }
}
