namespace kiwiDeal.Messages.Api.Requests;

public record StartConversationRequest(
    Guid ListingId,
    string ListingTitle,
    Guid RecipientId,
    string RecipientName,
    string InitialMessage);
