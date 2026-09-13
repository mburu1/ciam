using Ciam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ciam.Infrastructure.Persistence.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever();
        builder.Property(session => session.SessionTokenHash).HasMaxLength(512).IsRequired();
        builder.Property(session => session.RefreshTokenHash).HasMaxLength(512).IsRequired();
        builder.Property(session => session.Status).HasConversion<string>().IsRequired();
        builder.Property(session => session.IpAddress).HasMaxLength(64);
        builder.Property(session => session.UserAgent).HasMaxLength(1024);
        builder.HasIndex(session => session.UserId);
        builder.HasIndex(session => session.SessionTokenHash).IsUnique();
        builder.HasIndex(session => session.RefreshTokenHash).IsUnique();
    }
}
