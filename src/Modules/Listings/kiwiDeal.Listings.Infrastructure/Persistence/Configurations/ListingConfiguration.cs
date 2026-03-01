using kiwiDeal.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Listings.Infrastructure.Persistence.Configurations;

public sealed class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("listings", "listings");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(l => l.SellerId)
            .HasColumnName("seller_id")
            .IsRequired();

        builder.Property(l => l.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.Description)
            .HasColumnName("description")
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(l => l.ListingType)
            .HasColumnName("listing_type")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.BuyNowPrice)
            .HasColumnName("buy_now_price")
            .HasColumnType("numeric(18,2)");

        builder.Property(l => l.Category)
            .HasColumnName("category")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.Region)
            .HasColumnName("region")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.AuctionId)
            .HasColumnName("auction_id");

        builder.Property(l => l.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.OwnsMany(l => l.Images, imageBuilder =>
        {
            imageBuilder.ToTable("listing_images", "listings");

            imageBuilder.WithOwner().HasForeignKey("listing_id")
                .HasConstraintName("fk_listing_images_listings");

            imageBuilder.Property<int>("id")
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            imageBuilder.HasKey("id");

            imageBuilder.Property(i => i.Url)
                .HasColumnName("url")
                .HasMaxLength(2048)
                .IsRequired();

            imageBuilder.Property(i => i.DisplayOrder)
                .HasColumnName("display_order")
                .IsRequired();
        });

        builder.HasIndex(l => l.SellerId)
            .HasDatabaseName("ix_listings_seller_id");

        builder.HasIndex(l => l.Status)
            .HasDatabaseName("ix_listings_status");
        builder.HasIndex(l => l.Category)
            .HasDatabaseName("ix_listings_category");

        builder.HasIndex(l => l.Region)
            .HasDatabaseName("ix_listings_region");
        builder.Ignore(l => l.DomainEvents);
    }
}
