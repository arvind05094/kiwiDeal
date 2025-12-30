using FluentAssertions;
using kiwiDeal.Users.Api.Requests;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace kiwiDeal.Tests.Integration.Users;

[Collection("Integration Tests")]
public class AuthControllerTests : IAsyncLifetime
{
    private readonly KiwiDealWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public AuthControllerTests(KiwiDealWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE users.users RESTART IDENTITY CASCADE");
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Register_ValidRequest_Returns201()
    {
        var request = new RegisterRequest("validuser@test.com", "Password123", "New", "User");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.AccessToken.Should().NotBeNullOrEmpty();
        body.User.Email.Should().Be("validuser@test.com");
        body.User.FirstName.Should().Be("New");
        body.User.LastName.Should().Be("User");
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
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            var userExists = await dbContext.Users.AnyAsync(u => u.Email == "login@test.com");
            userExists.Should().BeTrue();
        }

        var loginRequest = new LoginRequest("login@test.com", "Password123");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        body!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var registerRequest = new RegisterRequest("wrongpass@test.com", "Password123", "Wrong", "Pass");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginRequest("wrongpass@test.com", "WrongPassword");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WithoutCookie_Returns401()
    {
        var response = await _client.PostAsync("/api/v1/auth/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
