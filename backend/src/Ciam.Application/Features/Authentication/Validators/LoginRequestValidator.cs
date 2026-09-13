using Ciam.Contracts.Requests.Auth;
using FluentValidation;

namespace Ciam.Application.Features.Authentication.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.UsernameOrEmail)
            .NotEmpty()
            .MaximumLength(254);
        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8);
        RuleFor(request => request.DeviceName)
            .MaximumLength(256)
            .When(request => request.DeviceName is not null);
    }
}
