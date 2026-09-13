using Ciam.Domain.Common;
using Ciam.Domain.Exceptions;

namespace Ciam.Domain.ValueObjects;

public sealed record PreferredUsername : ValueObject
{
    private PreferredUsername(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static PreferredUsername Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidUsernameException("Username is required.");
        }

        var normalized = value.Trim();

        if (normalized.Length is < 3 or > 64)
        {
            throw new InvalidUsernameException("Username length must be between 3 and 64 characters.");
        }

        if (!normalized.All(c => char.IsLetterOrDigit(c) || c is '-' or '_' or '.'))
        {
            throw new InvalidUsernameException("Username may only contain letters, numbers, dots, dashes, and underscores.");
        }

        return new PreferredUsername(normalized);
    }

    public override string ToString() => Value;

    public override IEnumerable<object?> GetAtomicValues() => [Value];

    public static implicit operator string(PreferredUsername username) => username.Value;
}
