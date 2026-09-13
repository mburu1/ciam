using System.Diagnostics;

namespace Ciam.Infrastructure.Telemetry;

public static class CiamActivitySource
{
    public const string Name = "Ciam.Infrastructure";
    public static readonly ActivitySource Source = new(Name);
}
