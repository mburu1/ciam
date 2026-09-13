using Ciam.Contracts.Requests.Auth;
using FluentValidation;

namespace Ciam.Application.Features.Authentication.Validators;

public sealed class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
        RuleFor(request => request.Code)
            .NotEmpty()
            .Length(6, 12);
    }
}
