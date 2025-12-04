using FluentAssertions;
using kiwiDeal.Auctions.Infrastructure.Persistence;
using kiwiDeal.Tests.Integration.Users;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace kiwiDeal.Tests.Integration.Auctions;

[Collection("Integration Tests")]
public class AuctionsEndpointTests : IAsyncLifetime
{
    private readonly KiwiDealWebApplicationFactory _factory;
    private HttpClient _client = null!;
    private HttpClient _bidderClient = null!;

    public AuctionsEndpointTests(KiwiDealWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateSellerClient();
        _bidderClient = _factory.CreateBidderClient();

        var options = new DbContextOptionsBuilder<AuctionsDbContext>()
            .UseNpgsql(_factory.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var db = new AuctionsDbContext(options);
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE auctions.auctions RESTART IDENTITY CASCADE");
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _bidderClient.Dispose();
        return Task.CompletedTask;
    }

    private async Task<Guid> CreateListingAsync()
    {
        var request = new
        {
            Title = "Auction Test Listing",
            Description = "A listing for auction tests",
            StartingPrice = 100.00m
        };

        var response = await _client.PostAsJsonAsync("/api/v1/listings", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = await response.Content.ReadAsStringAsync();
        return Guid.Parse(id.Trim('"'));
    }

    private async Task<Guid> CreateAuctionAsync(Guid listingId)
    {
        var request = new
        {
            ListingId = listingId,
            StartingPrice = 100.00m,
            StartTime = DateTimeOffset.UtcNow.AddSeconds(-1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auctions", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = await response.Content.ReadAsStringAsync();
        return Guid.Parse(id.Trim('"'));
    }

    [Fact]
    public async Task CreateAuction_ValidRequest_Returns201()
    {
        var listingId = await CreateListingAsync();

        var request = new
        {
            ListingId = listingId,
            StartingPrice = 100.00m,
            StartTime = DateTimeOffset.UtcNow.AddSeconds(-1),
            EndTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auctions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetAuction_ExistingAuction_Returns200()
    {
        var listingId = await CreateListingAsync();
        var auctionId = await CreateAuctionAsync(listingId);

        var response = await _client.GetAsync($"/api/v1/auctions/{auctionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAuction_NonExistentAuction_Returns404()
    {
        var response = await _client.GetAsync($"/api/v1/auctions/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PlaceBid_ValidBid_Returns204()
    {
        var listingId = await CreateListingAsync();
        var auctionId = await CreateAuctionAsync(listingId);

        var request = new { Amount = 150.00m };

        var response = await _bidderClient.PostAsJsonAsync($"/api/v1/auctions/{auctionId}/bids", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task PlaceBid_BidTooLow_Returns400()
    {
        var listingId = await CreateListingAsync();
        var auctionId = await CreateAuctionAsync(listingId);

        var request = new { Amount = 50.00m };

        var response = await _bidderClient.PostAsJsonAsync($"/api/v1/auctions/{auctionId}/bids", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
