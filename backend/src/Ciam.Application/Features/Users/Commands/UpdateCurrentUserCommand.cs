using Ciam.Application.Abstractions.Services;
using Ciam.Application.Common.Exceptions;
using Ciam.Application.Mappings;
using Ciam.Contracts.Requests.Users;
using Ciam.Contracts.Responses.Users;
using Ciam.Domain.Interfaces;
using Ciam.Domain.ValueObjects;
using MediatR;

namespace Ciam.Application.Features.Users.Commands;

public sealed record UpdateCurrentUserCommand(UpdateProfileRequest Request) : IRequest<UserProfileResponse>;

public sealed class UpdateCurrentUserCommandHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCurrentUserCommand, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(
        UpdateCurrentUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("The current request has no authenticated user.");

        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new ResourceNotFoundException("User", userId);

        var request = command.Request;
        user.UpdateProfile(
            FullName.Create(request.FirstName, request.LastName),
            string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : PhoneNumber.Create(request.PhoneNumber),
            request.Locale);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }
}
