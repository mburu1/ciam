using Ciam.Domain.Entities;
using Ciam.Domain.Enums;
using Ciam.Domain.Interfaces;
using Ciam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ciam.Infrastructure.Repositories;

public sealed class UserSessionRepository(CiamDbContext dbContext) : IUserSessionRepository
{
    public Task<UserSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.UserSessions.SingleOrDefaultAsync(session => session.Id == id, cancellationToken);

    public Task<UserSession?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.UserSessions.SingleOrDefaultAsync(
            session => session.UserId == userId && session.Status == SessionStatus.Active,
            cancellationToken);

    public async Task<IEnumerable<UserSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.UserSessions
            .Where(session => session.UserId == userId)
            .OrderByDescending(session => session.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task AddAsync(UserSession entity, CancellationToken cancellationToken = default) =>
        dbContext.UserSessions.AddAsync(entity, cancellationToken).AsTask();

    public Task UpdateAsync(UserSession entity, CancellationToken cancellationToken = default)
    {
        dbContext.UserSessions.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.UserSessions.Where(session => session.Id == id).ExecuteDeleteAsync(cancellationToken);
}
