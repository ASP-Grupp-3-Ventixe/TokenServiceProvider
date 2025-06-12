using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using Presentation.Models;
using Presentation.Services;

namespace TokenServiceProvider.Tests;

public class TokenServiceTests
{
    private readonly TokenService _sut; // SUT = System Under Test
    private readonly Mock<ILogger<TokenService>> _loggerMock;

    public TokenServiceTests()
    {
        // Setup environment variables
        Environment.SetEnvironmentVariable("Issuer", "https://localhost:7176");
        Environment.SetEnvironmentVariable("Audience", "Ventixe");
        Environment.SetEnvironmentVariable("SecretKey", "5d0493bd-dfbd-4fc4-822c-74095ce53d56");

        // Mock logger
        _loggerMock = new Mock<ILogger<TokenService>>();

        // Create instance of the service with mocked logger
        _sut = new TokenService(_loggerMock.Object);
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnToken_WhenValidRequest()
    {
        // Arrange
        var request = new TokenRequest
        {
            UserId = "12345",
            Email = "test@example.com",
            Role = "Admin"
        };

        // Act
        var response = await _sut.GenerateTokenAsync(request);

        // Assert
        response.Succeeded.Should().BeTrue();
        response.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.Message.Should().Be("Token generated.");
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldFail_WhenUserIdMissing()
    {
        // Arrange
        var request = new TokenRequest
        {
            UserId = "",
            Email = "test@example.com",
            Role = "Admin"
        };

        // Act
        var response = await _sut.GenerateTokenAsync(request);

        // Assert
        response.Succeeded.Should().BeFalse();
        response.Message.Should().Be("UserId is required");
    }

    [Fact]
    public async Task ValidateTokenAsync_ShouldReturnSuccess_WhenTokenValid()
    {
        // Arrange
        var request = new TokenRequest
        {
            UserId = "12345",
            Email = "test@example.com",
            Role = "Admin"
        };

        var generateResponse = await _sut.GenerateTokenAsync(request);

        var validationRequest = new ValidationRequest
        {
            AccessToken = generateResponse.AccessToken,
            UserId = "12345"
        };

        // Act
        var validationResponse = await _sut.ValidateTokenAsync(validationRequest);

        // Assert
        validationResponse.Succeeded.Should().BeTrue();
        validationResponse.Message.Should().Be("Access granted");
    }

    [Fact]
    public async Task ValidateTokenAsync_ShouldFail_WhenUserIdMismatch()
    {
        // Arrange
        var request = new TokenRequest
        {
            UserId = "12345",
            Email = "test@example.com",
            Role = "Admin"
        };

        var generateResponse = await _sut.GenerateTokenAsync(request);

        var validationRequest = new ValidationRequest
        {
            AccessToken = generateResponse.AccessToken,
            UserId = "wronguserid"
        };

        // Act
        var validationResponse = await _sut.ValidateTokenAsync(validationRequest);

        // Assert
        validationResponse.Succeeded.Should().BeFalse();
        validationResponse.Message.Should().Be("UserId does not match token.");
    }
}
