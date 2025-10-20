using kiwiDeal.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Users.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", "users");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(r => r.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                id => id.Value,
                value => UserId.From(value));

        builder.Property(r => r.Token)
            .HasColumnName("token")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(r => r.IsRevoked)
            .HasColumnName("is_revoked")
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(r => r.Token)
            .IsUnique()
            .HasDatabaseName("uq_refresh_tokens_token");

        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("ix_refresh_tokens_user_id");
    }
}
