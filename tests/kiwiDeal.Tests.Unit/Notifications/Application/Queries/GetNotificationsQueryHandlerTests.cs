using FluentAssertions;
using kiwiDeal.Notifications.Application.Queries;
using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Notifications.Application.Queries;

public class GetNotificationsQueryHandlerTests
{
    private readonly INotificationRepository _notificationRepository = Substitute.For<INotificationRepository>();
    private readonly GetNotificationsQueryHandler _handler;

    public GetNotificationsQueryHandlerTests()
    {
        _handler = new GetNotificationsQueryHandler(_notificationRepository);
    }

    [Fact]
    public async Task Handle_ReturnsMappedNotificationsAndUnreadCount()
    {
        var userId = Guid.NewGuid();
        var notifications = new List<Notification>
        {
            Notification.Create(userId, NotificationType.AuctionWon, "Title 1", "Message 1", "/auctions/1"),
            Notification.Create(userId, NotificationType.NewMessage, "Title 2", "Message 2", null)
        };

        _notificationRepository.GetRecentForUserAsync(userId, 20, Arg.Any<CancellationToken>())
            .Returns(notifications);

        _notificationRepository.GetUnreadCountAsync(userId, Arg.Any<CancellationToken>())
            .Returns(2);

        var result = await _handler.Handle(new GetNotificationsQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.UnreadCount.Should().Be(2);
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items[0].Title.Should().Be("Title 1");
        result.Value.Items[0].LinkUrl.Should().Be("/auctions/1");
        result.Value.Items[1].Title.Should().Be("Title 2");
    }
}
