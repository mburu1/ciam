namespace Ciam.Application.Abstractions.Services;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Subject { get; }
    bool IsAuthenticated { get; }
}
