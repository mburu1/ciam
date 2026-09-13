using Ciam.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ciam.Infrastructure.Persistence;

public sealed class CiamDbContext(DbContextOptions<CiamDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<AuthenticationChallenge> AuthenticationChallenges => Set<AuthenticationChallenge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CiamDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
