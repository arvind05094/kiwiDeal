using FluentAssertions;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.Commands;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace kiwiDeal.Tests.Unit.Users.Application;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();

    private readonly RegisterCommandHandler _handler;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(
            _userRepository,
            _unitOfWork,
            _passwordHasher,
            _jwtTokenGenerator,
            NullLogger<RegisterCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        var command = new RegisterCommand("test@test.com", "Password123", "John", "Doe");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        _passwordHasher.Hash(command.Password).Returns("hashedpassword");
        _jwtTokenGenerator.GenerateAccessToken(Arg.Any<User>()).Returns("access-token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh-token");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.User.Email.Should().Be("test@test.com");
        result.Value.User.FirstName.Should().Be("John");
        result.Value.User.LastName.Should().Be("Doe");
        result.Value.AccessToken.Should().Be("access-token");
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_ReturnsFailure()
    {
        var command = new RegisterCommand("test@test.com", "Password123", "John", "Doe");

        var existingUser = User.Create("test@test.com", "hashedpassword", "John", "Doe").Value;

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(existingUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.Conflict);

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
