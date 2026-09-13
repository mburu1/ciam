using System.Text.RegularExpressions;
using Ciam.Domain.Common;
using Ciam.Domain.Exceptions;

namespace Ciam.Domain.ValueObjects;

public sealed record PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex = new(
        "^\\+?[1-9]\\d{1,14}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPhoneNumberException("Phone number is required.");
        }

        var normalized = value.Trim();

        if (!PhoneRegex.IsMatch(normalized))
        {
            throw new InvalidPhoneNumberException($"'{value}' is not a valid phone number.");
        }

        return new PhoneNumber(normalized);
    }

    public override string ToString() => Value;

    public override IEnumerable<object?> GetAtomicValues() => [Value];

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
