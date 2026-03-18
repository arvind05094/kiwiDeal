using FluentAssertions;
using kiwiDeal.Listings.Application.Queries;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Enums;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.Listings.Domain.ValueObjects;
using kiwiDeal.SharedKernel.Contracts;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Listings.Application.Queries;

public class GetListingSummariesQueryHandlerTests
{
    private readonly IListingRepository _listingRepository = Substitute.For<IListingRepository>();
    private readonly GetListingSummariesQueryHandler _handler;

    public GetListingSummariesQueryHandlerTests()
    {
        _handler = new GetListingSummariesQueryHandler(_listingRepository);
    }

    [Fact]
    public async Task Handle_EmptyIds_ReturnsEmptyListWithoutCallingRepository()
    {
        var result = await _handler.Handle(new GetListingSummariesQuery([]), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        await _listingRepository.DidNotReceive().GetByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingIds_ReturnsSummariesWithFirstImageByDisplayOrder()
    {
        var listing = Listing.Create(
            SellerId.From(Guid.NewGuid()),
            "Seller",
            "Cool Bike",
            "A great bike",
            ListingType.FixedPrice,
            100m,
            ListingCategory.Sports,
            ListingRegion.Auckland).Value;

        listing.AddImage(ListingImage.Create("https://example.com/2.png", 2));
        listing.AddImage(ListingImage.Create("https://example.com/1.png", 1));

        _listingRepository.GetByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Listing> { listing });

        var result = await _handler.Handle(new GetListingSummariesQuery([listing.Id.Value]), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var summary = result.Value[0];
        summary.Id.Should().Be(listing.Id.Value);
        summary.Title.Should().Be("Cool Bike");
        summary.ThumbnailUrl.Should().Be("https://example.com/1.png");
    }
}
