using Ciam.Contracts.Requests.Auth;

namespace Ciam.Integration.Tests;

public sealed class InfrastructureSmokeTests
{
    [Fact]
    public void Integration_test_project_targets_backend_contracts()
    {
        var request = new RegisterUserRequest
        {
            FirstName = "Integration",
            LastName = "Smoke",
            Email = "integration@example.com",
            PreferredUsername = "integration.smoke"
        };

        Assert.Equal("integration@example.com", request.Email);
        Assert.Equal("integration.smoke", request.PreferredUsername);
    }
}
