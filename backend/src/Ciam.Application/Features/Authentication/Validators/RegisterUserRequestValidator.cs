using Ciam.Contracts.Requests.Auth;
using FluentValidation;

namespace Ciam.Application.Features.Authentication.Validators;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);
        RuleFor(request => request.PreferredUsername)
            .NotEmpty()
            .Length(3, 64)
            .Matches("^[A-Za-z0-9._-]+$");
        RuleFor(request => request.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber));
        RuleFor(request => request.Locale)
            .MaximumLength(20)
            .When(request => request.Locale is not null);
    }
}
