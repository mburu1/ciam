namespace Ciam.Application.Abstractions.Services;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
