using kiwiDeal.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kiwiDeal.Messages.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages", "messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => MessageId.From(value));

        builder.Property(x => x.ConversationId)
            .HasConversion(
                id => id.Value,
                value => ConversationId.From(value))
            .IsRequired();

        builder.Property(x => x.SenderId).IsRequired();
        builder.Property(x => x.Content).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.Property(x => x.SenderName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.IsRead).IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.ConversationId).HasDatabaseName("ix_messages_conversation_id");
    }
}
