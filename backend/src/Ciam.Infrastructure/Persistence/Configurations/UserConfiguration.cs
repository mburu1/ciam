using Ciam.Domain.Entities;
using Ciam.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ciam.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();
        builder.Property(user => user.Status).HasConversion<string>().IsRequired();
        builder.Property(user => user.Subject).HasMaxLength(255);
        builder.Property(user => user.Locale).HasMaxLength(20);
        builder.HasIndex(user => user.Subject).IsUnique().HasFilter("\"Subject\" IS NOT NULL");

        builder.OwnsOne(user => user.Email, email =>
        {
            email.Property(value => value.Value).HasColumnName("email").HasMaxLength(254).IsRequired();
            email.HasIndex(value => value.Value).IsUnique();
        });
        builder.OwnsOne(user => user.FullName, name =>
        {
            name.Property(value => value.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            name.Property(value => value.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        });
        builder.OwnsOne(user => user.PreferredUsername, username =>
        {
            username.Property(value => value.Value).HasColumnName("preferred_username").HasMaxLength(64).IsRequired();
            username.HasIndex(value => value.Value).IsUnique();
        });
        builder.OwnsOne(user => user.PhoneNumber, phone =>
            phone.Property(value => value.Value).HasColumnName("phone_number").HasMaxLength(20));

        builder.Property<HashSet<UserRole>>("_roles")
            .HasColumnName("roles")
            .HasColumnType("text")
            .HasConversion(
                roles => string.Join(',', roles.Select(role => role.ToString())),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Enum.Parse<UserRole>)
                    .ToHashSet());
        builder.Property<HashSet<MfaMethod>>("_mfaMethods")
            .HasColumnName("mfa_methods")
            .HasColumnType("text")
            .HasConversion(
                methods => string.Join(',', methods.Select(method => method.ToString())),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Enum.Parse<MfaMethod>)
                    .ToHashSet());
        builder.Property<HashSet<AuthenticationMethod>>("_authenticationMethods")
            .HasColumnName("authentication_methods")
            .HasColumnType("text")
            .HasConversion(
                methods => string.Join(',', methods.Select(method => method.ToString())),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Enum.Parse<AuthenticationMethod>)
                    .ToHashSet());
    }
}
