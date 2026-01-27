using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Messages.Infrastructure.Persistence.Repositories;

public class ConversationRepository(MessagesDbContext context) : IConversationRepository
{
    public async Task<Conversation?> GetByIdAsync(ConversationId id, CancellationToken ct = default)
    {
        return await context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Conversation?> GetByParticipantsAndListingAsync(
        Guid listingId, Guid userA, Guid userB, CancellationToken ct = default)
    {
        return await context.Conversations
            .FirstOrDefaultAsync(c =>
                c.ListingId == listingId &&
                ((c.SenderId == userA && c.RecipientId == userB) ||
                 (c.SenderId == userB && c.RecipientId == userA)), ct);
    }

    public async Task<List<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Conversations
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Where(c => c.SenderId == userId || c.RecipientId == userId)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task MarkMessagesAsReadAsync(ConversationId conversationId, Guid userId, CancellationToken ct = default)
    {
        await context.Messages
            .Where(m => m.ConversationId == conversationId &&
                        m.SenderId != userId &&
                        !m.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true), ct);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken ct = default)
    {
        await context.Conversations.AddAsync(conversation, ct);
    }

    public void Update(Conversation conversation)
    {
        context.Conversations.Update(conversation);
    }
}
