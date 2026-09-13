using Ciam.Application.Features.Authentication.Validators;
using Ciam.Application.Features.Users.Validators;
using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Requests.Users;

namespace Ciam.Application.Tests;

public sealed class ValidationTests
{
    private readonly RegisterUserRequestValidator _registrationValidator = new();
    private readonly UpdateProfileRequestValidator _profileValidator = new();

    [Fact]
    public void Registration_validator_accepts_valid_request()
    {
        var result = _registrationValidator.Validate(new RegisterUserRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            PreferredUsername = "ada.lovelace",
            PhoneNumber = "+254712345678",
            Locale = "en"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Registration_validator_reports_all_invalid_fields()
    {
        var result = _registrationValidator.Validate(new RegisterUserRequest
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = "invalid",
            PreferredUsername = "a b",
            PhoneNumber = "123"
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserRequest.FirstName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserRequest.LastName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserRequest.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserRequest.PreferredUsername));
    }

    [Fact]
    public void Profile_validator_rejects_oversized_locale()
    {
        var result = _profileValidator.Validate(new UpdateProfileRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Locale = new string('x', 21)
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProfileRequest.Locale));
    }
}
