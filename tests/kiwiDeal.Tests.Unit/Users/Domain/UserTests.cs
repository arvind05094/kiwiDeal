using FluentAssertions;
using kiwiDeal.Users.Domain.Entities;

namespace kiwiDeal.Tests.Unit.Users.Domain;

public class UserTests
{
    [Fact]
    public void Create_ValidInput_ReturnsSuccess()
    {
        var result = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("test@test.com");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
        result.Value.Role.Should().Be(Roles.Buyer);
        result.Value.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_EmptyEmail_ReturnsFailure()
    {
        var result = User.Create("", "hashedpassword", "John", "Doe", Roles.Buyer);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Email is required");
    }

    [Fact]
    public void Create_EmptyPasswordHash_ReturnsFailure()
    {
        var result = User.Create("test@test.com", "", "John", "Doe", Roles.Buyer);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Password hash is required");
    }

    [Fact]
    public void Create_EmptyFirstName_ReturnsFailure()
    {
        var result = User.Create("test@test.com", "hashedpassword", "", "Doe", Roles.Buyer);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("First name is required");
    }

    [Fact]
    public void Create_EmptyLastName_ReturnsFailure()
    {
        var result = User.Create("test@test.com", "hashedpassword", "John", "", Roles.Buyer);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Last name is required");
    }

    [Fact]
    public void AddRefreshToken_ValidToken_AddsToCollection()
    {
        var user = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer).Value;
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        user.AddRefreshToken("token123", expiresAt);

        user.RefreshTokens.Should().HaveCount(1);
        user.RefreshTokens[0].Token.Should().Be("token123");
        user.RefreshTokens[0].IsActive.Should().BeTrue();
    }

    [Fact]
    public void AddRefreshToken_SecondToken_RevokesFirst()
    {
        var user = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer).Value;
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        user.AddRefreshToken("token1", expiresAt);
        user.AddRefreshToken("token2", expiresAt);

        user.RefreshTokens[0].IsRevoked.Should().BeTrue();
        user.RefreshTokens[1].IsActive.Should().BeTrue();
    }

    [Fact]
    public void GetActiveRefreshToken_ValidToken_ReturnsSuccess()
    {
        var user = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer).Value;
        user.AddRefreshToken("token123", DateTimeOffset.UtcNow.AddDays(7));

        var result = user.GetActiveRefreshToken("token123");

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("token123");
    }

    [Fact]
    public void GetActiveRefreshToken_InvalidToken_ReturnsFailure()
    {
        var user = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer).Value;

        var result = user.GetActiveRefreshToken("nonexistent");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Delete_SetsIsDeletedAndRevokesTokens()
    {
        var user = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer).Value;
        user.AddRefreshToken("token123", DateTimeOffset.UtcNow.AddDays(7));

        user.Delete();

        user.IsDeleted.Should().BeTrue();
        user.DeletedAt.Should().NotBeNull();
        user.RefreshTokens[0].IsRevoked.Should().BeTrue();
    }
}
