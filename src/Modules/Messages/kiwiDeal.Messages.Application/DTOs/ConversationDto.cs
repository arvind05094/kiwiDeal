namespace kiwiDeal.Messages.Application.DTOs;

public class ConversationDto
{
    public Guid Id { get; init; }
    public Guid ListingId { get; init; }
    public string ListingTitle { get; init; } = string.Empty;
    public Guid OtherUserId { get; init; }
    public string OtherUserName { get; init; } = string.Empty;
    public string LastMessagePreview { get; init; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; init; }
}
