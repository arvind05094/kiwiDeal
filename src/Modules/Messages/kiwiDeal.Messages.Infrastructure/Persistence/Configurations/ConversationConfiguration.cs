using kiwiDeal.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Messages.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("conversations", "messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => ConversationId.From(value));

        builder.Property(x => x.ListingId).IsRequired();
        builder.Property(x => x.SenderId).IsRequired();
        builder.Property(x => x.RecipientId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.DeletedAt);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Messages)
            .WithOne()
            .HasForeignKey(x => x.ConversationId);

        builder.HasIndex(x => x.SenderId).HasDatabaseName("ix_conversations_sender_id");
        builder.HasIndex(x => x.RecipientId).HasDatabaseName("ix_conversations_recipient_id");
        builder.HasIndex(x => new { x.ListingId, x.SenderId, x.RecipientId })
            .HasDatabaseName("ix_conversations_listing_sender_recipient");
    }
}
