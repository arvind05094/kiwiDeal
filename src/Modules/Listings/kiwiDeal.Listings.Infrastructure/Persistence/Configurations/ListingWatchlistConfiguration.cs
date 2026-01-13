using kiwiDeal.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Listings.Infrastructure.Persistence.Configurations;

public sealed class ListingWatchlistConfiguration : IEntityTypeConfiguration<ListingWatchlist>
{
    public void Configure(EntityTypeBuilder<ListingWatchlist> builder)
    {
        builder.ToTable("listing_watchlist", "listings");

        builder.HasKey(w => new { w.UserId, w.ListingId });

        builder.Property(w => w.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(w => w.ListingId)
            .HasColumnName("listing_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => ListingId.From(value));

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasOne(w => w.Listing)
            .WithMany()
            .HasForeignKey(w => w.ListingId)
            .HasConstraintName("fk_listing_watchlist_listings");

        builder.HasIndex(w => w.UserId)
            .HasDatabaseName("ix_listing_watchlist_user_id");
    }
}
