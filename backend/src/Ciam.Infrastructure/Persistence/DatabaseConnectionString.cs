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

        if (string.IsNullOrWhiteSpace(password))
        {
            return connectionString;
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Password = password
        };

        return builder.ConnectionString;
    }
}
