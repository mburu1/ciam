using Ciam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ciam.Infrastructure.Persistence.Configurations;

public sealed class AuthenticationChallengeConfiguration : IEntityTypeConfiguration<AuthenticationChallenge>
{
    public void Configure(EntityTypeBuilder<AuthenticationChallenge> builder)
    {
        builder.ToTable("authentication_challenges");
        builder.HasKey(challenge => challenge.Id);
        builder.Property(challenge => challenge.Id).ValueGeneratedNever();
        builder.Property(challenge => challenge.Purpose).HasConversion<string>().IsRequired();
        builder.Property(challenge => challenge.ChallengeCode).HasMaxLength(512).IsRequired();
        builder.Property(challenge => challenge.Status).HasConversion<string>().IsRequired();
        builder.HasIndex(challenge => new { challenge.UserId, challenge.CreatedAtUtc });
    }
}
