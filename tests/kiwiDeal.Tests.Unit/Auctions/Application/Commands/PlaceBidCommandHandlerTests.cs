using FluentAssertions;
using kiwiDeal.Auctions.Application.Commands;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Auctions.Application.Commands;

public class PlaceBidCommandHandlerTests
{
    private readonly IAuctionRepository _auctionRepository = Substitute.For<IAuctionRepository>();
    private readonly IAuctionsUnitOfWork _unitOfWork = Substitute.For<IAuctionsUnitOfWork>();
    private readonly IAuctionHubContext _hubContext = Substitute.For<IAuctionHubContext>();
    private readonly PlaceBidCommandHandler _handler;

    public PlaceBidCommandHandlerTests()
    {
        _handler = new PlaceBidCommandHandler(_auctionRepository, _unitOfWork, _hubContext);
    }

    private static Auction CreateActiveAuction(Guid sellerId, decimal startingPrice = 100m)
    {
        var auction = Auction.Create(
            Guid.NewGuid(),
            "Test Listing",
            sellerId,
            startingPrice,
            DateTimeOffset.UtcNow.AddMinutes(-10),
            DateTimeOffset.UtcNow.AddHours(1)).Value;

        auction.Activate();
        return auction;
    }

    [Fact]
    public async Task Handle_ValidBid_PlacesBidSavesAndNotifies()
    {
        var sellerId = Guid.NewGuid();
        var bidderId = Guid.NewGuid();
        var auction = CreateActiveAuction(sellerId);

        _auctionRepository.GetByIdAsync(auction.Id, Arg.Any<CancellationToken>())
            .Returns(auction);

        var command = new PlaceBidCommand(auction.Id.Value, bidderId, "Bidder", 150m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        auction.CurrentHighestBid.Should().Be(150m);
        auction.CurrentHighestBidderId.Should().Be(bidderId);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _hubContext.Received(1).SendBidPlaced(
            auction.Id.Value.ToString(), Arg.Any<Guid>(), bidderId, "Bidder", 150m, Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AuctionNotFound_ReturnsFailure()
    {
        _auctionRepository.GetByIdAsync(Arg.Any<AuctionId>(), Arg.Any<CancellationToken>())
            .Returns((Auction?)null);

        var command = new PlaceBidCommand(Guid.NewGuid(), Guid.NewGuid(), "Bidder", 150m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.NotFound);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_BidTooLow_ReturnsFailureWithoutSaving()
    {
        var sellerId = Guid.NewGuid();
        var auction = CreateActiveAuction(sellerId, startingPrice: 100m);

        _auctionRepository.GetByIdAsync(auction.Id, Arg.Any<CancellationToken>())
            .Returns(auction);

        var command = new PlaceBidCommand(auction.Id.Value, Guid.NewGuid(), "Bidder", 50m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.BidTooLow);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
