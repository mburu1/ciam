using Ciam.Application.Abstractions.Identity;
using Ciam.Application.Common.Exceptions;
using Ciam.Application.Abstractions.Services;
using Ciam.Application.Mappings;
using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Responses.Users;
using Ciam.Domain.Entities;
using Ciam.Domain.Interfaces;
using Ciam.Domain.ValueObjects;
using MediatR;

namespace Ciam.Application.Features.Authentication.Commands;

public sealed record RegisterUserCommand(RegisterUserRequest Request) : IRequest<UserProfileResponse>;

public sealed class RegisterUserCommandHandler(
    IIdentityProvider identityProvider,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterUserCommand, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var email = EmailAddress.Create(request.Email);

        if (await userRepository.GetByEmailAsync(email.Value, cancellationToken) is not null)
        {
            throw new ConflictException("A user with this email address already exists.");
        }

        var identity = await identityProvider.RegisterAsync(request, cancellationToken);
        var user = User.Create(
            identity.Subject,
            FullName.Create(request.FirstName, request.LastName),
            email,
            PreferredUsername.Create(request.PreferredUsername),
            request.Locale,
            identity.Subject.ToString("D"));

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            user.UpdateProfile(
                user.FullName,
                PhoneNumber.Create(request.PhoneNumber),
                request.Locale);
        }

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }
}
