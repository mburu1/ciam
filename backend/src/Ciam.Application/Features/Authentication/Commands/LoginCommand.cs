using Ciam.Application.Abstractions.Identity;
using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Responses.Auth;
using MediatR;

namespace Ciam.Application.Features.Authentication.Commands;

public sealed record LoginCommand(LoginRequest Request) : IRequest<AuthTokensResponse>;

public sealed class LoginCommandHandler(IIdentityProvider identityProvider)
    : IRequestHandler<LoginCommand, AuthTokensResponse>
{
    public Task<AuthTokensResponse> Handle(
        LoginCommand command,
        CancellationToken cancellationToken) =>
        identityProvider.LoginAsync(command.Request, cancellationToken);
}
