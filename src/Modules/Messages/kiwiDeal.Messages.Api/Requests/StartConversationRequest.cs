namespace kiwiDeal.Messages.Api.Requests;

public record StartConversationRequest(
    Guid RecipientId,
    string RecipientName,
    string InitialMessage);