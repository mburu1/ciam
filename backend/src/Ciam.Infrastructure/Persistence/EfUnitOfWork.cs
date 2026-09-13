using Ciam.Domain.Interfaces;

namespace Ciam.Infrastructure.Persistence;

public sealed class EfUnitOfWork(CiamDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
