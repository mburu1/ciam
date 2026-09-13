using System.Text.RegularExpressions;
using Ciam.Domain.Common;
using Ciam.Domain.Exceptions;

namespace Ciam.Domain.ValueObjects;

public sealed record EmailAddress : ValueObject
{
    private static readonly Regex EmailRegex = new(
        "^(?=.{1,254}$)(?=.{1,64}@)(?:[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*)@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private EmailAddress(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEmailAddressException("Email address is required.");
        }

        var normalizedValue = value.Trim();

        if (!EmailRegex.IsMatch(normalizedValue))
        {
            throw new InvalidEmailAddressException($"'{value}' is not a valid email address.");
        }

        return new EmailAddress(normalizedValue);
    }

    public override string ToString() => Value;

    public override IEnumerable<object?> GetAtomicValues() => [Value];

    public static implicit operator string(EmailAddress emailAddress) => emailAddress.Value;
}
