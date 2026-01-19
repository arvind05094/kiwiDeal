using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Messages.Domain.Errors;

public static class MessageErrors
{
    public static readonly Error CannotMessageSelf =
        new(ErrorCode.Conflict, "You cannot message yourself");

    public static readonly Error ContentEmpty =
        new(ErrorCode.ValidationFailed, "Message content cannot be empty");

    public static Error ConversationNotFound(Guid id) =>
        Error.NotFound($"Conversation {id} was not found");

    public static readonly Error NotParticipant =
        new(ErrorCode.Forbidden, "You are not a participant in this conversation");

    public static readonly Error ConversationAlreadyExists =
        new(ErrorCode.ConversationAlreadyExists, "A conversation already exists for this listing between these users");
}
