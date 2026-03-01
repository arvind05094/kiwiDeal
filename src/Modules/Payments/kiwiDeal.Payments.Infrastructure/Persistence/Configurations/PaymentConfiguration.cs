using kiwiDeal.Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Payments.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", "payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.AuctionId)
            .HasColumnName("auction_id");

        builder.Property(p => p.ListingId)
            .HasColumnName("listing_id")
            .IsRequired();

        builder.Property(p => p.BuyerId)
            .HasColumnName("buyer_id")
            .IsRequired();

        builder.Property(p => p.PaymentType)
            .HasColumnName("payment_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.SellerId)
            .HasColumnName("seller_id")
            .IsRequired();

        builder.Property(p => p.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.StripeSessionId)
            .HasColumnName("stripe_session_id")
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(p => p.PaidAt)
            .HasColumnName("paid_at");

        builder.HasIndex(p => p.AuctionId)
            .HasDatabaseName("ix_payments_auction_id");

        builder.HasIndex(p => p.StripeSessionId)
            .HasDatabaseName("ix_payments_stripe_session_id");
    }
}
