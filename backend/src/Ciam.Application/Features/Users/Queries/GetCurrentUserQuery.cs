using Ciam.Application.Abstractions.Services;
using Ciam.Application.Common.Exceptions;
using Ciam.Application.Mappings;
using Ciam.Contracts.Responses.Users;
using Ciam.Domain.Interfaces;
using MediatR;

namespace Ciam.Application.Features.Users.Queries;

public sealed record GetCurrentUserQuery : IRequest<UserProfileResponse>;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository)
    : IRequestHandler<GetCurrentUserQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("The current request has no authenticated user.");

        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new ResourceNotFoundException("User", userId);

        return user.ToResponse();
    }
}
