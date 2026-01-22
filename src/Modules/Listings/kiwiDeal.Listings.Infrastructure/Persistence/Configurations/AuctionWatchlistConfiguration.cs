using kiwiDeal.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Listings.Infrastructure.Persistence.Configurations;

public sealed class AuctionWatchlistConfiguration : IEntityTypeConfiguration<AuctionWatchlist>
{
    public void Configure(EntityTypeBuilder<AuctionWatchlist> builder)
    {
        builder.ToTable("auction_watchlist", "listings");

        builder.HasKey(w => new { w.UserId, w.AuctionId });

        builder.Property(w => w.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(w => w.AuctionId)
            .HasColumnName("auction_id")
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(w => w.UserId)
            .HasDatabaseName("ix_auction_watchlist_user_id");

        builder.HasIndex(w => w.AuctionId)
            .HasDatabaseName("ix_auction_watchlist_auction_id");
    }
}
