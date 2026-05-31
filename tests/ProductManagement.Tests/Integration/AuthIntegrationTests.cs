using System.Net;
using System.Net.Http.Json;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOkWithUserId()
    {
        // Arrange
        var dto = new UserRegistrationDto
        {
            Username = "integrationuser",
            Password = "SecurePass1!",
            ConfirmPassword = "SecurePass1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/register", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserRegistrationResponseDto>();
        result.Should().NotBeNull();
        result!.UserId.Should().BeGreaterThan(0);
        result.Username.Should().Be("integrationuser");
    }

    [Fact]
    public async Task Register_DuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UserRegistrationDto
        {
            Username = "duplicateuser",
            Password = "SecurePass1!",
            ConfirmPassword = "SecurePass1!"
        };

        // Register first time
        await _client.PostAsJsonAsync("/api/v1/Auth/register", dto);

        // Act - register again with same username
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/register", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_AfterRegistration_ReturnsToken()
    {
        // Arrange - register first
        var registerDto = new UserRegistrationDto
        {
            Username = "logintest1",
            Password = "SecurePass1!",
            ConfirmPassword = "SecurePass1!"
        };
        await _client.PostAsJsonAsync("/api/v1/Auth/register", registerDto);

        var loginDto = new UserLoginDto
        {
            Username = "logintest1",
            Password = "SecurePass1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("token");
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Username = "nonexistent",
            Password = "WrongPass1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange - register first
        var registerDto = new UserRegistrationDto
        {
            Username = "wrongpassuser",
            Password = "CorrectPass1!",
            ConfirmPassword = "CorrectPass1!"
        };
        await _client.PostAsJsonAsync("/api/v1/Auth/register", registerDto);

        var loginDto = new UserLoginDto
        {
            Username = "wrongpassuser",
            Password = "WrongPass1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
