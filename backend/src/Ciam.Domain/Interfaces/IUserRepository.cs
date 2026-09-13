using Ciam.Domain.Entities;

namespace Ciam.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetBySubjectAsync(string subject, CancellationToken cancellationToken = default);
}
