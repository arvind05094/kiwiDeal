using FluentAssertions;
using kiwiDeal.Messages.Application.Commands;
using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Messages.Application.Commands;

public class SendMessageCommandHandlerTests
{
    private readonly IConversationRepository _conversationRepository = Substitute.For<IConversationRepository>();
    private readonly IMessagesUnitOfWork _unitOfWork = Substitute.For<IMessagesUnitOfWork>();
    private readonly IMessageHubContext _hubContext = Substitute.For<IMessageHubContext>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly ILogger<SendMessageCommandHandler> _logger = Substitute.For<ILogger<SendMessageCommandHandler>>();
    private readonly SendMessageCommandHandler _handler;

    public SendMessageCommandHandlerTests()
    {
        _handler = new SendMessageCommandHandler(_conversationRepository, _unitOfWork, _hubContext, _publisher, _logger);
    }

    [Fact]
    public async Task Handle_ValidMessage_ReturnsSuccessAndSaves()
    {
        var senderId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var conversation = Conversation.Create(senderId, "Sender", recipientId, "Recipient").Value;

        _conversationRepository.GetByIdAsync(conversation.Id, Arg.Any<CancellationToken>())
            .Returns(conversation);

        var command = new SendMessageCommand(conversation.Id.Value, senderId, "Sender", "Hello there");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Content.Should().Be("Hello there");
        result.Value.SenderId.Should().Be(senderId);

        _conversationRepository.Received(1).Update(conversation);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _hubContext.Received(1).SendMessageReceived(
            conversation.Id.Value, Arg.Any<Guid>(), senderId, "Sender", "Hello there", Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConversationNotFound_ReturnsFailure()
    {
        _conversationRepository.GetByIdAsync(Arg.Any<ConversationId>(), Arg.Any<CancellationToken>())
            .Returns((Conversation?)null);

        var command = new SendMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "Sender", "Hello");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.NotFound);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SenderNotParticipant_ReturnsFailure()
    {
        var conversation = Conversation.Create(Guid.NewGuid(), "Sender", Guid.NewGuid(), "Recipient").Value;

        _conversationRepository.GetByIdAsync(conversation.Id, Arg.Any<CancellationToken>())
            .Returns(conversation);

        var command = new SendMessageCommand(conversation.Id.Value, Guid.NewGuid(), "Stranger", "Hello");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.Forbidden);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
