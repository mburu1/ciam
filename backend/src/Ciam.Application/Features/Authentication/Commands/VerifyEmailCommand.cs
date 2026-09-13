using Ciam.Application.Abstractions.Identity;
using Ciam.Contracts.Requests.Auth;
using MediatR;

namespace Ciam.Application.Features.Authentication.Commands;

public sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest;

public sealed class VerifyEmailCommandHandler(IIdentityProvider identityProvider)
    : IRequestHandler<VerifyEmailCommand>
{
    public Task Handle(VerifyEmailCommand command, CancellationToken cancellationToken) =>
        identityProvider.VerifyEmailAsync(
            command.Request.UserId,
            command.Request.Code,
            cancellationToken);
}
