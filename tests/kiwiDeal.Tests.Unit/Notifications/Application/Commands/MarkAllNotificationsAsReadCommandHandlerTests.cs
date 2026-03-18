using FluentAssertions;
using kiwiDeal.Notifications.Application.Commands;
using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Notifications.Application.Commands;

public class MarkAllNotificationsAsReadCommandHandlerTests
{
    private readonly INotificationRepository _notificationRepository = Substitute.For<INotificationRepository>();
    private readonly INotificationsUnitOfWork _unitOfWork = Substitute.For<INotificationsUnitOfWork>();
    private readonly MarkAllNotificationsAsReadCommandHandler _handler;

    public MarkAllNotificationsAsReadCommandHandlerTests()
    {
        _handler = new MarkAllNotificationsAsReadCommandHandler(_notificationRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_HasUnreadNotifications_MarksAllAsReadAndSaves()
    {
        var userId = Guid.NewGuid();
        var unread = new List<Notification>
        {
            Notification.Create(userId, NotificationType.AuctionWon, "Title 1", "Message 1", null),
            Notification.Create(userId, NotificationType.NewMessage, "Title 2", "Message 2", null)
        };

        _notificationRepository.GetUnreadForUserAsync(userId, Arg.Any<CancellationToken>())
            .Returns(unread);

        var result = await _handler.Handle(new MarkAllNotificationsAsReadCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        unread.Should().OnlyContain(n => n.IsRead);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoUnreadNotifications_StillSucceedsAndSaves()
    {
        var userId = Guid.NewGuid();

        _notificationRepository.GetUnreadForUserAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<Notification>());

        var result = await _handler.Handle(new MarkAllNotificationsAsReadCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
