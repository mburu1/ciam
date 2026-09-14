using Ciam.Application.Abstractions.Identity;
using Ciam.Application.Common.Exceptions;
using Ciam.Application.Features.Authentication.Commands;
using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Responses.Auth;
using Ciam.Domain.Entities;
using Ciam.Domain.Interfaces;

namespace Ciam.Application.Tests;

public sealed class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_creates_local_user_with_identity_subject_and_saves_once()
    {
        var subject = Guid.NewGuid();
        var repository = new InMemoryUserRepository();
        var identity = new StubIdentityProvider
        {
            RegistrationResult = new IdentityRegistrationResult(subject, "verification-id")
        };
        var unitOfWork = new StubUnitOfWork();
        var handler = new RegisterUserCommandHandler(identity, repository, unitOfWork);

        var response = await handler.Handle(
            new RegisterUserCommand(new RegisterUserRequest
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Email = "ada@example.com",
                PreferredUsername = "ada.lovelace",
                PhoneNumber = "+254712345678",
                Locale = "en"
            }),
            CancellationToken.None);

        var user = Assert.Single(repository.Users);
        Assert.Equal(subject, user.Id);
        Assert.Equal(subject.ToString("D"), user.Subject);
        Assert.Equal("Ada", response.FirstName);
        Assert.Equal("Lovelace", response.LastName);
        Assert.Equal(1, unitOfWork.SaveCount);
        Assert.Equal(1, identity.RegisterCount);
    }

    [Fact]
    public async Task Handle_rejects_duplicate_email_without_calling_identity_provider()
    {
        var repository = new InMemoryUserRepository();
        repository.Users.Add(CreateExistingUser());
        var identity = new StubIdentityProvider();
        var handler = new RegisterUserCommandHandler(identity, repository, new StubUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new RegisterUserCommand(new RegisterUserRequest
            {
                FirstName = "Grace",
                LastName = "Hopper",
                Email = "ada@example.com",
                PreferredUsername = "grace.hopper"
            }),
            CancellationToken.None));

        Assert.Equal(0, identity.RegisterCount);
    }

    private static User CreateExistingUser() =>
        User.Create(
            Guid.NewGuid(),
            Ciam.Domain.ValueObjects.FullName.Create("Ada", "Lovelace"),
            Ciam.Domain.ValueObjects.EmailAddress.Create("ada@example.com"),
            Ciam.Domain.ValueObjects.PreferredUsername.Create("ada.lovelace"));

    private sealed class InMemoryUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.SingleOrDefault(user => user.Id == id));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.SingleOrDefault(user => user.Email.Value == email));

        public Task<User?> GetBySubjectAsync(string subject, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.SingleOrDefault(user => user.Subject == subject));

        public Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            Users.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User entity, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Users.RemoveAll(user => user.Id == id);
            return Task.CompletedTask;
        }
    }

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class StubIdentityProvider : IIdentityProvider
    {
        public IdentityRegistrationResult RegistrationResult { get; init; } =
            new(Guid.NewGuid(), null);

        public int RegisterCount { get; private set; }

        public Task<IdentityRegistrationResult> RegisterAsync(
            RegisterUserRequest request,
            CancellationToken cancellationToken = default)
        {
            RegisterCount++;
            return Task.FromResult(RegistrationResult);
        }

        public Task<AuthTokensResponse> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<AuthTokensResponse> RefreshAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task SendEmailVerificationAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task VerifyEmailAsync(
            Guid userId,
            string code,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
