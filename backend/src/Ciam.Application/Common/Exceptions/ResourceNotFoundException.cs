namespace Ciam.Application.Common.Exceptions;

public sealed class ResourceNotFoundException(string resourceName, object resourceKey)
    : Exception($"{resourceName} with key '{resourceKey}' was not found.")
{
    public string ResourceName { get; } = resourceName;
    public object ResourceKey { get; } = resourceKey;
}
