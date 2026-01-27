using kiwiDeal.Messages.Domain.Entities;

namespace kiwiDeal.Messages.Domain.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(ConversationId id, CancellationToken ct = default);
    Task<Conversation?> GetByParticipantsAndListingAsync(Guid listingId, Guid userA, Guid userB, CancellationToken ct = default);
    Task<List<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task MarkMessagesAsReadAsync(ConversationId conversationId, Guid userId, CancellationToken ct = default);
    Task AddAsync(Conversation conversation, CancellationToken ct = default);
    void Update(Conversation conversation);
}
