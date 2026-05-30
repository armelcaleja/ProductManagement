using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using ProductManagement.Services;

namespace ProductManagement.Tests.Services;

public class AuthServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _tokenServiceMock = new Mock<ITokenService>();
        _authService = new AuthService(_context, _tokenServiceMock.Object);
    }

    #region Register Tests

    [Fact]
    public async Task RegisterUserAsync_NewUser_ReturnsUserId()
    {
        // Arrange
        var dto = new UserRegistrationDto
        {
            Username = "newuser01",
            Password = "SecurePass1!",
            ConfirmPassword = "SecurePass1!"
        };

        // Act
        var result = await _authService.RegisterUserAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeGreaterThan(0);

        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser01");
        savedUser.Should().NotBeNull();
        savedUser!.Password.Should().NotBe("SecurePass1!"); // Should be hashed
    }

    [Fact]
    public async Task RegisterUserAsync_DuplicateUsername_ReturnsNull()
    {
        // Arrange
        _context.Users.Add(new User { Username = "existing", Password = "hashed" });
        await _context.SaveChangesAsync();

        var dto = new UserRegistrationDto
        {
            Username = "existing",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var result = await _authService.RegisterUserAsync(dto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterUserAsync_DuplicateUsername_CaseInsensitive_ReturnsNull()
    {
        // Arrange
        _context.Users.Add(new User { Username = "TestUser", Password = "hashed" });
        await _context.SaveChangesAsync();

        var dto = new UserRegistrationDto
        {
            Username = "testuser",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var result = await _authService.RegisterUserAsync(dto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterUserAsync_TrimsUsername()
    {
        // Arrange
        var dto = new UserRegistrationDto
        {
            Username = "  spaceduser  ",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        await _authService.RegisterUserAsync(dto);

        // Assert
        var user = await _context.Users.FirstOrDefaultAsync();
        user!.Username.Should().Be("spaceduser");
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task LoginUserAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
        _context.Users.Add(new User { UserId = 1, Username = "loginuser", Password = hashedPassword });
        await _context.SaveChangesAsync();

        _tokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<UserDto>()))
            .Returns("generated-jwt-token");

        var dto = new UserLoginDto { Username = "loginuser", Password = "Password123!" };

        // Act
        var result = await _authService.LoginUserAsync(dto);

        // Assert
        result.Should().Be("generated-jwt-token");
        _tokenServiceMock.Verify(t => t.GenerateToken(It.Is<UserDto>(u =>
            u.UserId == 1 && u.Username == "loginuser")), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_InvalidUsername_ReturnsNull()
    {
        // Arrange
        var dto = new UserLoginDto { Username = "nonexistent", Password = "Password123!" };

        // Act
        var result = await _authService.LoginUserAsync(dto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginUserAsync_InvalidPassword_ReturnsNull()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!");
        _context.Users.Add(new User { UserId = 1, Username = "user1", Password = hashedPassword });
        await _context.SaveChangesAsync();

        var dto = new UserLoginDto { Username = "user1", Password = "WrongPassword!" };

        // Act
        var result = await _authService.LoginUserAsync(dto);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
