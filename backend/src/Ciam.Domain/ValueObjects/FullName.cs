using Ciam.Domain.Common;
using Ciam.Domain.Exceptions;

namespace Ciam.Domain.ValueObjects;

public sealed record FullName : ValueObject
{
    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }
    public string LastName { get; }

    public string DisplayName => $"{FirstName} {LastName}".Trim();

    public static FullName Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new InvalidPersonNameException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new InvalidPersonNameException("Last name is required.");
        }

        var normalizedFirstName = firstName.Trim();
        var normalizedLastName = lastName.Trim();

        if (normalizedFirstName.Length is < 1 or > 100)
        {
            throw new InvalidPersonNameException("First name length must be between 1 and 100 characters.");
        }

        if (normalizedLastName.Length is < 1 or > 100)
        {
            throw new InvalidPersonNameException("Last name length must be between 1 and 100 characters.");
        }

        return new FullName(normalizedFirstName, normalizedLastName);
    }

    public override string ToString() => DisplayName;

    public override IEnumerable<object?> GetAtomicValues() => [FirstName, LastName];

    public static implicit operator string(FullName fullName) => fullName.DisplayName;
}
