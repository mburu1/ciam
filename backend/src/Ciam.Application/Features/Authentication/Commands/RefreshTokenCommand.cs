using Ciam.Application.Abstractions.Identity;
using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Responses.Auth;
using MediatR;

namespace Ciam.Application.Features.Authentication.Commands;

public sealed record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<AuthTokensResponse>;

public sealed class RefreshTokenCommandHandler(IIdentityProvider identityProvider)
    : IRequestHandler<RefreshTokenCommand, AuthTokensResponse>
{
    public Task<AuthTokensResponse> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken) =>
        identityProvider.RefreshAsync(command.Request, cancellationToken);
}
