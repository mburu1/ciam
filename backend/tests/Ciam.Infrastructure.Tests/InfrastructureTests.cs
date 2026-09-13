using Ciam.Infrastructure.Identity;
using Ciam.Infrastructure.Telemetry;

namespace Ciam.Infrastructure.Tests;

public sealed class InfrastructureTests
{
    [Fact]
    public void KeycloakOptions_exposes_expected_configuration_section()
    {
        Assert.Equal("Keycloak", KeycloakOptions.SectionName);
        Assert.Equal(TimeSpan.FromSeconds(15), new KeycloakOptions().Timeout);
    }

    [Fact]
    public void SystemDateTimeProvider_returns_a_recent_utc_timestamp()
    {
        var before = DateTimeOffset.UtcNow;
        var value = new SystemDateTimeProvider().UtcNow;
        var after = DateTimeOffset.UtcNow;

        Assert.InRange(value, before, after);
    }
}
