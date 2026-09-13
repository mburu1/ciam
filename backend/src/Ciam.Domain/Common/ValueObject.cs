namespace Ciam.Domain.Common;

public abstract record ValueObject
{
    public abstract IEnumerable<object?> GetAtomicValues();

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in GetAtomicValues())
        {
            hash.Add(value);
        }

        return hash.ToHashCode();
    }
}
