using kiwiDeal.Auctions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Configurations;

public sealed class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.ToTable("auctions", "auctions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => AuctionId.From(value));

        builder.Property(a => a.ListingId)
            .HasColumnName("listing_id")
            .IsRequired();

        builder.Property(a => a.ListingTitle)
            .HasColumnName("listing_title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.SellerId)
            .HasColumnName("seller_id")
            .IsRequired();

        builder.Property(a => a.StartingPrice)
            .HasColumnName("starting_price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(a => a.CurrentHighestBid)
            .HasColumnName("current_highest_bid")
            .HasColumnType("numeric(18,2)");

        builder.Property(a => a.CurrentHighestBidderId)
            .HasColumnName("current_highest_bidder_id");

        builder.Property(a => a.StartTime)
            .HasColumnName("start_time")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(a => a.EndTime)
            .HasColumnName("end_time")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.OwnsMany(a => a.Bids, bidBuilder =>
        {
            bidBuilder.ToTable("auction_bids", "auctions");

            bidBuilder.WithOwner().HasForeignKey("auction_id")
                .HasConstraintName("fk_auction_bids_auctions");

            bidBuilder.HasKey(b => b.Id);

            bidBuilder.Property(b => b.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => AuctionBidId.From(value));

            bidBuilder.Ignore(b => b.AuctionId);

            bidBuilder.Property(b => b.BidderId)
                .HasColumnName("bidder_id")
                .IsRequired();

            bidBuilder.Property(b => b.BidderName)
                .HasColumnName("bidder_name")
                .HasMaxLength(200)
                .IsRequired();

            bidBuilder.Property(b => b.Amount)
                .HasColumnName("amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            bidBuilder.Property(b => b.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            bidBuilder.Property(b => b.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamptz")
                .IsRequired();

            bidBuilder.HasIndex("auction_id")
                .HasDatabaseName("ix_auction_bids_auction_id");
        });

        builder.HasIndex(a => a.ListingId)
            .HasDatabaseName("ix_auctions_listing_id");

        builder.HasIndex(a => a.SellerId)
            .HasDatabaseName("ix_auctions_seller_id");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("ix_auctions_status");

        builder.Property(a => a.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
        builder.Ignore(a => a.DomainEvents);
    }
}
