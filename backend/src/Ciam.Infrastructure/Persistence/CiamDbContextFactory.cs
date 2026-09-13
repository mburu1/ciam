using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ciam.Infrastructure.Persistence;

public sealed class CiamDbContextFactory : IDesignTimeDbContextFactory<CiamDbContext>
{
    public CiamDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CIAM_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=ciam;Username=ciam;Password=local-ciam-db-password";

        var options = new DbContextOptionsBuilder<CiamDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(CiamDbContext).Assembly.FullName))
            .Options;

        return new CiamDbContext(options);
    }
}
