using Ciam.Contracts.Responses.Users;
using Ciam.Domain.Entities;

namespace Ciam.Application.Mappings;

public static class UserMappings
{
    public static UserProfileResponse ToResponse(this User user) =>
        new()
        {
            UserId = user.Id,
            FirstName = user.FullName.FirstName,
            LastName = user.FullName.LastName,
            Email = user.Email.Value,
            PreferredUsername = user.PreferredUsername.Value,
            PhoneNumber = user.PhoneNumber?.Value,
            Locale = user.Locale,
            EmailVerified = user.EmailVerified,
            Status = user.Status.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc
        };
}
