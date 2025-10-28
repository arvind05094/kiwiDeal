using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using kiwiDeal.Users.Api.Requests;
using kiwiDeal.Users.Application.DTOs;

namespace kiwiDeal.Tests.Integration.Users;

public class AuthControllerTests : IClassFixture<KiwiDealWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(KiwiDealWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidRequest_Returns201()
    {
        var request = new RegisterRequest("newuser@test.com", "Password123", "New", "User");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<UserResponse>();
        body!.Email.Should().Be("newuser@test.com");
        body.Role.Should().Be("Buyer");
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var request = new RegisterRequest("duplicate@test.com", "Password123", "John", "Doe");

        await _client.PostAsJsonAsync("/api/v1/auth/register", request);
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        var request = new RegisterRequest("notanemail", "Password123", "John", "Doe");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var registerRequest = new RegisterRequest("login@test.com", "Password123", "Login", "User");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginRequest("login@test.com", "Password123");
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var registerRequest = new RegisterRequest("wrongpass@test.com", "Password123", "Wrong", "Pass");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginRequest("wrongpass@test.com", "WrongPassword");
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WithoutCookie_Returns401()
    {
        var response = await _client.PostAsync("/api/v1/auth/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
