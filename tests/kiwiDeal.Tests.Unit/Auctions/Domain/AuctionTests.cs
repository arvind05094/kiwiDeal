using FluentAssertions;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Tests.Unit.Auctions.Domain;

public class AuctionTests
{
    private static Auction CreateActiveAuction(
        Guid? sellerId = null,
        decimal startingPrice = 100m,
        DateTimeOffset? endTime = null)
    {
        var seller = sellerId ?? Guid.NewGuid();
        var start = DateTimeOffset.UtcNow.AddMinutes(-10);
        var end = endTime ?? DateTimeOffset.UtcNow.AddDays(1);
        var auction = Auction.Create(Guid.NewGuid(), "Test Listing", seller, startingPrice, start, end).Value;
        auction.Activate();
        return auction;
    }

    [Fact]
    public void Create_ValidInput_ReturnsSuccess()
    {
        var sellerId = Guid.NewGuid();
        var start = DateTimeOffset.UtcNow.AddMinutes(5);
        var end = start.AddDays(1);
        var result = Auction.Create(Guid.NewGuid(), "Test Listing", sellerId, 100m, start, end);
        result.IsSuccess.Should().BeTrue();
        result.Value.SellerId.Should().Be(sellerId);
        result.Value.StartingPrice.Should().Be(100m);
        result.Value.Status.Should().Be(AuctionStatus.Scheduled);
    }

    [Fact]
    public void Create_EndTimeBeforeStartTime_ReturnsFailure()
    {
        var start = DateTimeOffset.UtcNow.AddDays(1);
        var end = DateTimeOffset.UtcNow;
        var result = Auction.Create(Guid.NewGuid(), "Test Listing", Guid.NewGuid(), 100m, start, end);
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("End time must be after start time");
    }

    [Fact]
    public void PlaceBid_ValidBid_ReturnsSuccess()
    {
        var auction = CreateActiveAuction(startingPrice: 100m);
        var bidderId = Guid.NewGuid();
        var result = auction.PlaceBid(bidderId, "Jane Doe", 150m);
        result.IsSuccess.Should().BeTrue();
        auction.CurrentHighestBid.Should().Be(150m);
        auction.CurrentHighestBidderId.Should().Be(bidderId);
    }

    [Fact]
    public void PlaceBid_BidTooLow_ReturnsFailure()
    {
        var auction = CreateActiveAuction(startingPrice: 100m);
        var result = auction.PlaceBid(Guid.NewGuid(), "Jane Doe", 50m);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.BidTooLow);
    }

    [Fact]
    public void PlaceBid_BidderIsSeller_ReturnsFailure()
    {
        var sellerId = Guid.NewGuid();
        var auction = CreateActiveAuction(sellerId: sellerId);
        var result = auction.PlaceBid(sellerId, "Jane Doe", 150m);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.BidderIsSeller);
    }

    [Fact]
    public void PlaceBid_AuctionClosed_ReturnsFailure()
    {
        var auction = CreateActiveAuction();
        auction.Close();
        var result = auction.PlaceBid(Guid.NewGuid(), "Jane Doe", 150m);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.AuctionClosed);
    }

    [Fact]
    public void PlaceBid_InFinalFiveMinutes_ExtendsEndTime()
    {
        var endTime = DateTimeOffset.UtcNow.AddMinutes(3);
        var auction = CreateActiveAuction(endTime: endTime);
        var originalEndTime = auction.EndTime;
        auction.PlaceBid(Guid.NewGuid(), "Jane Doe", 150m);
        auction.EndTime.Should().BeAfter(originalEndTime);
        auction.EndTime.Should().BeCloseTo(originalEndTime.AddMinutes(5), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void PlaceBid_RaisesBidPlacedEvent()
    {
        var auction = CreateActiveAuction();
        auction.PlaceBid(Guid.NewGuid(), "Jane Doe", 150m);
        auction.DomainEvents.Should().HaveCount(1);
        auction.DomainEvents[0].Should().BeOfType<kiwiDeal.Auctions.Domain.Events.BidPlacedEvent>();
    }
}
