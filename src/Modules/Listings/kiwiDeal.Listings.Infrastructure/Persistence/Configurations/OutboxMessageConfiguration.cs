using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Listings.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", "listings");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(o => o.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(o => o.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(o => o.OccurredOn)
            .HasColumnName("occurred_on")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(o => o.ProcessedOn)
            .HasColumnName("processed_on")
            .HasColumnType("timestamptz");


        builder.Property(o => o.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(o => o.Error)
            .HasColumnName("error")
            .HasColumnType("text");
        builder.HasIndex(o => o.ProcessedOn)
            .HasDatabaseName("ix_outbox_messages_processed_on")
            .HasFilter("processed_on IS NULL");
    }
}
