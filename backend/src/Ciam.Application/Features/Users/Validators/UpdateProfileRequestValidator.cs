using Ciam.Contracts.Requests.Users;
using FluentValidation;

namespace Ciam.Application.Features.Users.Validators;

public sealed class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(request => request.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber));
        RuleFor(request => request.Locale)
            .MaximumLength(20)
            .When(request => request.Locale is not null);
    }
}
