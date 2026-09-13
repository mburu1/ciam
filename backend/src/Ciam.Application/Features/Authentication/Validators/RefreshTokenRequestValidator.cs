using Ciam.Contracts.Requests.Auth;
using FluentValidation;

namespace Ciam.Application.Features.Authentication.Validators;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.RefreshToken).NotEmpty();
        RuleFor(request => request.DeviceName)
            .MaximumLength(256)
            .When(request => request.DeviceName is not null);
    }
}
