using Ciam.Domain.Entities;
using Ciam.Domain.Interfaces;
using Ciam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ciam.Infrastructure.Repositories;

public sealed class UserRepository(CiamDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Email.Value == email, cancellationToken);

    public Task<User?> GetBySubjectAsync(string subject, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Subject == subject, cancellationToken);

    public Task AddAsync(User entity, CancellationToken cancellationToken = default) =>
        dbContext.Users.AddAsync(entity, cancellationToken).AsTask();

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Users.Where(user => user.Id == id).ExecuteDeleteAsync(cancellationToken);
}
