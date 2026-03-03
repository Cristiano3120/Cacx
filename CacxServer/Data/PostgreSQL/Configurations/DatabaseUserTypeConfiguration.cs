using CacxServer.Data.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CacxServer.Data.PostgreSQL.Configurations;

public class DatabaseUserTypeConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        _ = builder.Property(x => x.Id)
            .IsRequired();

        _ = builder.Property(x => x.Email)
            .IsRequired();

        _ = builder.Property(x => x.Username)
            .IsRequired();

        _ = builder.Property(x => x.PasswordHash)
            .IsRequired();

        _ = builder.Property(x => x.DisplayName)
            .IsRequired();

        _ = builder.Property(x => x.CreatedAt)
            .IsRequired();

        _ = builder
            .HasIndex(x => x.Email)
            .IsUnique();

        _ = builder
            .HasIndex(x => x.Username)
            .IsUnique();
    }
}
