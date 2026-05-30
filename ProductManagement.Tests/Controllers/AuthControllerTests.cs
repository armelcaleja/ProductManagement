using Microsoft.AspNetCore.Mvc;
using ProductManagement.Controllers;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;

namespace ProductManagement.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    #region Register Tests

    [Fact]
    public async Task Register_ValidUser_ReturnsOkWithResponse()
    {
        // Arrange
        var dto = new UserRegistrationDto
        {
            Username = "newuser01",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        _authServiceMock.Setup(s => s.RegisterUserAsync(dto)).ReturnsAsync(1);

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<UserRegistrationResponseDto>().Subject;
        response.UserId.Should().Be(1);
        response.Username.Should().Be("newuser01");
        response.Message.Should().Contain("successfully");
    }

    [Fact]
    public async Task Register_DuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var dto = new UserRegistrationDto
        {
            Username = "existing_user",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };
        _authServiceMock.Setup(s => s.RegisterUserAsync(dto)).ReturnsAsync((int?)null);

        // Act
        var result = await _controller.Register(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var dto = new UserLoginDto { Username = "testuser", Password = "Password123!" };
        _authServiceMock.Setup(s => s.LoginUserAsync(dto)).ReturnsAsync("fake-jwt-token");

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new UserLoginDto { Username = "baduser", Password = "wrong" };
        _authServiceMock.Setup(s => s.LoginUserAsync(dto)).ReturnsAsync((string?)null);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion
}
