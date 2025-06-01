using FluentAssertions;
using Presentation.Models;
using Presentation.Services;

namespace TokenServiceProvider.Tests;

// Jag har bett ChatGpt skriva testkoden i denna klass enligt mina instruktioner och min struktur
// och sedan klistrat in det.

public class TokenServiceTests
{
    public TokenServiceTests()
    {
        Environment.SetEnvironmentVariable("Issuer", "https://localhost:7176");
        Environment.SetEnvironmentVariable("Audience", "Ventixe");
        Environment.SetEnvironmentVariable("SecretKey", "5d0493bd-dfbd-4fc4-822c-74095ce53d56");
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnToken_WhenValidRequest()
    {
        // Arrange
        var tokenService = new TokenService();

        var request = new TokenRequest
        {
            UserId = "12345",
            Email = "test@example.com",
            Role = "Admin"
        };

        // Act
        var response = await tokenService.GenerateTokenAsync(request);

        // Assert
        response.Succeeded.Should().BeTrue();
        response.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.Message.Should().Be("Token generated.");
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldFail_WhenUserIdMissing()
    {
        // Arrange
        var tokenService = new TokenService();

        var request = new TokenRequest
        {
            UserId = "",
            Email = "test@example.com",
            Role = "Admin"
        };

        // Act
        var response = await tokenService.GenerateTokenAsync(request);

        // Assert
        response.Succeeded.Should().BeFalse();
        response.Message.Should().Be("UserId is required");
    }

    [Fact]
    public async Task ValidateTokenAsync_ShouldReturnSuccess_WhenTokenValid()
    {
        // Arrange
        var tokenService = new TokenService();

        var request = new TokenRequest
        {
            UserId = "12345",
            Email = "test@example.com",
            Role = "Admin"
        };

        var generateResponse = await tokenService.GenerateTokenAsync(request);

        var validationRequest = new ValidationRequest
        {
            AccessToken = generateResponse.AccessToken,
            UserId = "12345"
        };

        // Act
        var validationResponse = await tokenService.ValidateTokenAsync(validationRequest);

        // Assert
        validationResponse.Succeeded.Should().BeTrue();
        validationResponse.Message.Should().Be("Access granted");
    }

    [Fact]
    public async Task ValidateTokenAsync_ShouldFail_WhenUserIdMismatch()
    {
        // Arrange
        var tokenService = new TokenService();

        var request = new TokenRequest
        {
            UserId = "12345",
            Email = "test@example.com",
            Role = "Admin"
        };

        var generateResponse = await tokenService.GenerateTokenAsync(request);

        var validationRequest = new ValidationRequest
        {
            AccessToken = generateResponse.AccessToken,
            UserId = "wronguserid"
        };

        // Act
        var validationResponse = await tokenService.ValidateTokenAsync(validationRequest);

        // Assert
        validationResponse.Succeeded.Should().BeFalse();
        validationResponse.Message.Should().Be("UserId does not match token.");
    }
}
