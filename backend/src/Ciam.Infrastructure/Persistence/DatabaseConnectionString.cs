using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Ciam.Infrastructure.Persistence;

public static class DatabaseConnectionString
{
    public static string Resolve(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Ciam")
            ?? throw new InvalidOperationException("ConnectionStrings:Ciam is required.");
        var password = configuration["Database:Password"];
        if (!IsUsablePassword(password))
        {
            password = configuration["Database__Password"];
        }

        if (!IsUsablePassword(password))
        {
            password = Environment.GetEnvironmentVariable("Database__Password");
        }

        if (!IsUsablePassword(password))
        {
            password = new NpgsqlConnectionStringBuilder(connectionString).Password;
        }

        if (!IsUsablePassword(password))
        {
            throw new InvalidOperationException(
                "A database password is required. Configure Database:Password or Database__Password.");
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Password = password
        };

        return builder.ConnectionString;
    }

    private static bool IsUsablePassword(string? password) =>
        !string.IsNullOrWhiteSpace(password)
        && !string.Equals(password, "******", StringComparison.Ordinal)
        && !string.Equals(password, "replace-for-local-use", StringComparison.OrdinalIgnoreCase);
}
