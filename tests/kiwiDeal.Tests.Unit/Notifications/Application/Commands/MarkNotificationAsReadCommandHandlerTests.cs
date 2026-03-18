using FluentAssertions;
using kiwiDeal.Notifications.Application.Commands;
using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Notifications.Application.Commands;

public class MarkNotificationAsReadCommandHandlerTests
{
    private readonly INotificationRepository _notificationRepository = Substitute.For<INotificationRepository>();
    private readonly INotificationsUnitOfWork _unitOfWork = Substitute.For<INotificationsUnitOfWork>();
    private readonly MarkNotificationAsReadCommandHandler _handler;

    public MarkNotificationAsReadCommandHandlerTests()
    {
        _handler = new MarkNotificationAsReadCommandHandler(_notificationRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_OwnedNotification_MarksAsReadAndSaves()
    {
        var userId = Guid.NewGuid();
        var notification = Notification.Create(userId, NotificationType.AuctionWon, "Title", "Message", null);
        var command = new MarkNotificationAsReadCommand(notification.Id.Value, userId);

        _notificationRepository.GetByIdAsync(NotificationId.From(notification.Id.Value), Arg.Any<CancellationToken>())
            .Returns(notification);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        notification.IsRead.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotificationNotFound_ReturnsFailure()
    {
        var command = new MarkNotificationAsReadCommand(Guid.NewGuid(), Guid.NewGuid());

        _notificationRepository.GetByIdAsync(Arg.Any<NotificationId>(), Arg.Any<CancellationToken>())
            .Returns((Notification?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.NotFound);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotificationBelongsToAnotherUser_ReturnsFailure()
    {
        var notification = Notification.Create(Guid.NewGuid(), NotificationType.AuctionWon, "Title", "Message", null);
        var command = new MarkNotificationAsReadCommand(notification.Id.Value, Guid.NewGuid());

        _notificationRepository.GetByIdAsync(NotificationId.From(notification.Id.Value), Arg.Any<CancellationToken>())
            .Returns(notification);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.NotFound);
        notification.IsRead.Should().BeFalse();

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
