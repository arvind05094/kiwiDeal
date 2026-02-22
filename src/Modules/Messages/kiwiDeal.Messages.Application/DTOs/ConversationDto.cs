namespace kiwiDeal.Messages.Application.DTOs;

public class ConversationDto
{
    public Guid Id { get; init; }
    public Guid OtherUserId { get; init; }
    public string OtherUserName { get; init; } = string.Empty;
    public string LastMessagePreview { get; init; } = string.Empty;
    public int UnreadCount { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}