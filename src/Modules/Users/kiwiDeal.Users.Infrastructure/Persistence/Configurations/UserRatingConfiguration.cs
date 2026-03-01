using kiwiDeal.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Users.Infrastructure.Persistence.Configurations;

public sealed class UserRatingConfiguration : IEntityTypeConfiguration<UserRating>
{
    public void Configure(EntityTypeBuilder<UserRating> builder)
    {
        builder.ToTable("user_ratings", "users");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(r => r.RaterId)
            .HasColumnName("rater_id")
            .IsRequired();

        builder.Property(r => r.RateeId)
            .HasColumnName("ratee_id")
            .IsRequired();

        builder.Property(r => r.Stars)
            .HasColumnName("stars")
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasColumnName("comment")
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(r => r.RateeId)
            .HasDatabaseName("ix_user_ratings_ratee_id");

        builder.HasIndex(r => new { r.RaterId, r.RateeId })
            .IsUnique()
            .HasDatabaseName("uq_user_ratings_rater_ratee");
    }
}
