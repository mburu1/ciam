using Ciam.Domain.Entities;

namespace Ciam.Domain.Interfaces;

public interface IAuthenticationChallengeRepository : IRepository<AuthenticationChallenge>
{
    Task<AuthenticationChallenge?> GetLatestByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
