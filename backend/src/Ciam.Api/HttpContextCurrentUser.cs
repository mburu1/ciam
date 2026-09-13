using System.Security.Claims;
using Ciam.Application.Abstractions.Services;

namespace Ciam.Api;

public sealed class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public string? Subject => Principal?.FindFirstValue("sub");

    public Guid? UserId =>
        Guid.TryParse(Subject, out var userId)
            ? userId
            : null;
}
