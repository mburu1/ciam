using Ciam.Domain.Entities;
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

        builder.Ignore("_roles");
        builder.Ignore("_mfaMethods");
        builder.Ignore("_authenticationMethods");
    }
}
