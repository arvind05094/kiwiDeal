namespace kiwiDeal.Messages.Api.Requests;

public record StartConversationRequest(
    Guid ListingId,
    Guid RecipientId,
    string InitialMessage);
