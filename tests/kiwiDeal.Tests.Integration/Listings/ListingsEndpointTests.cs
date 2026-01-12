using FluentAssertions;
using kiwiDeal.Listings.Infrastructure.Persistence;
using kiwiDeal.Tests.Integration.Users;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace kiwiDeal.Tests.Integration.Listings;

[Collection("Integration Tests")]
public class ListingsEndpointTests : IAsyncLifetime
{
    private readonly KiwiDealWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public ListingsEndpointTests(KiwiDealWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateSellerClient();

        var options = new DbContextOptionsBuilder<ListingsDbContext>()
            .UseNpgsql(_factory.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var db = new ListingsDbContext(options);
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE listings.listings RESTART IDENTITY CASCADE");
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateListing_ValidRequest_Returns201()
    {
        var request = new
        {
            Title = "Test Listing",
            Description = "A test listing description",
            StartingPrice = 100.00m,
            ReservePrice = 150.00m,
            EndTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/listings", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateListing_MissingTitle_Returns400()
    {
        var request = new
        {
            Title = "",
            Description = "A test listing description",
            StartingPrice = 100.00m,
            EndTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/listings", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetListing_ExistingListing_Returns200()
    {
        var createRequest = new
        {
            Title = "Get Test Listing",
            Description = "A test listing description",
            StartingPrice = 100.00m,
            ReservePrice = 150.00m,
            EndTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/listings", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = await createResponse.Content.ReadAsStringAsync();
        id = id.Trim('"');
        var response = await _client.GetAsync($"/api/v1/listings/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetListing_NonExistentListing_Returns404()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/v1/listings/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetListings_Returns200WithPagedResult()
    {
        var response = await _client.GetAsync("/api/v1/listings?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteListing_ExistingListing_Returns204()
    {
        var createRequest = new
        {
            Title = "Delete Test Listing",
            Description = "A test listing description",
            StartingPrice = 100.00m,
            EndTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/listings", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = await createResponse.Content.ReadAsStringAsync();
        id = id.Trim('"');
        var response = await _client.PostAsync($"/api/v1/listings/{id}/cancel", null);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
