using Ciam.Domain.Entities;

namespace Ciam.Domain.Interfaces;

public interface IUserSessionRepository : IRepository<UserSession>
{
    Task<UserSession?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
