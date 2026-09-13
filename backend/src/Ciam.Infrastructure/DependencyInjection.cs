using Ciam.Application.Abstractions.Identity;
using Ciam.Application.Abstractions.Services;
using Ciam.Domain.Interfaces;
using Ciam.Infrastructure.Email;
using Ciam.Infrastructure.Identity;
using Ciam.Infrastructure.Persistence;
using Ciam.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ciam.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Ciam")
            ?? throw new InvalidOperationException("ConnectionStrings:Ciam is required.");

        services.AddDbContext<CiamDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(CiamDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IAuthenticationChallengeRepository, AuthenticationChallengeRepository>();
        services.AddScoped<IEmailSender, LoggingEmailSender>();

        services.AddOptions<KeycloakOptions>()
            .Bind(configuration.GetSection(KeycloakOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _), "Keycloak:BaseUrl must be an absolute URI.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Realm), "Keycloak:Realm is required.")
            .ValidateOnStart();

        services.AddHttpClient<IIdentityProvider, KeycloakIdentityProvider>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = options.Timeout;
        });

        return services;
    }
}
