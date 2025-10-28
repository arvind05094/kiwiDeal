using FluentAssertions;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.Commands;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Users.Application;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(
            _userRepository,
            _passwordHasher,
            _jwtTokenGenerator,
            _unitOfWork,
            NullLogger<LoginCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccess()
    {
        var command = new LoginCommand("test@test.com", "Password123");
        var user = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer).Value;

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("access-token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh-token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access-token");
        result.Value.RefreshToken.Should().Be("refresh-token");
        result.Value.User.Email.Should().Be("test@test.com");

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var command = new LoginCommand("notfound@test.com", "Password123");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.Unauthorised);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        var command = new LoginCommand("test@test.com", "WrongPassword");
        var user = User.Create("test@test.com", "hashedpassword", "John", "Doe", Roles.Buyer).Value;

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.Unauthorised);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
