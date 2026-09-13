using Ciam.Application.Abstractions.Services;

namespace Ciam.Infrastructure.Telemetry;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
