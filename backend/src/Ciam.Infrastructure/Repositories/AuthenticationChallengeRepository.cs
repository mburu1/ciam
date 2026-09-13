using Ciam.Domain.Entities;
using Ciam.Domain.Interfaces;
using Ciam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ciam.Infrastructure.Repositories;

public sealed class AuthenticationChallengeRepository(CiamDbContext dbContext) : IAuthenticationChallengeRepository
{
    public Task<AuthenticationChallenge?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.AuthenticationChallenges.SingleOrDefaultAsync(challenge => challenge.Id == id, cancellationToken);

    public Task<AuthenticationChallenge?> GetLatestByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.AuthenticationChallenges
            .Where(challenge => challenge.UserId == userId)
            .OrderByDescending(challenge => challenge.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public Task AddAsync(AuthenticationChallenge entity, CancellationToken cancellationToken = default) =>
        dbContext.AuthenticationChallenges.AddAsync(entity, cancellationToken).AsTask();

    public Task UpdateAsync(AuthenticationChallenge entity, CancellationToken cancellationToken = default)
    {
        dbContext.AuthenticationChallenges.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.AuthenticationChallenges.Where(challenge => challenge.Id == id).ExecuteDeleteAsync(cancellationToken);
}
