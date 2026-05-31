using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Controllers.V2;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using System.Security.Claims;

namespace ProductManagement.Tests.Controllers;

public class ItemsControllerTests
{
    private readonly Mock<IItemService> _serviceMock;
    private readonly ItemsController _controller;

    public ItemsControllerTests()
    {
        _serviceMock = new Mock<IItemService>();
        _controller = new ItemsController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithItems()
    {
        // Arrange
        var items = new List<Item>
        {
            new() { ItemId = 1, ItemName = "Item A" },
            new() { ItemId = 2, ItemName = "Item B" }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<ItemResponseDto>>().Subject;
        response.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        // Arrange
        var item = new Item { ItemId = 1, ItemName = "Test Item" };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(item);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<ItemResponseDto>().Subject;
        dto.ItemName.Should().Be("Test Item");
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Item?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Add_ValidItem_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new ItemCreateDto { ItemName = "New Item" };
        var created = new Item { ItemId = 3, ItemName = "New Item" };
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<Item>())).ReturnsAsync(created);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<ItemResponseDto>().Subject;
        response.ItemId.Should().Be(3);
    }

    [Fact]
    public async Task AssignToPackage_Success_ReturnsOk()
    {
        // Arrange
        var dto = new PackageItemAssignDto { PackageId = 1, ItemId = 2 };
        _serviceMock.Setup(s => s.AssignToPackageAsync(It.IsAny<PackageItem>())).ReturnsAsync(true);

        // Act
        var result = await _controller.AssignToPackage(dto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AssignToPackage_Failure_ReturnsBadRequest()
    {
        // Arrange
        var dto = new PackageItemAssignDto { PackageId = 1, ItemId = 2 };
        _serviceMock.Setup(s => s.AssignToPackageAsync(It.IsAny<PackageItem>())).ReturnsAsync(false);

        // Act
        var result = await _controller.AssignToPackage(dto);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Edit_ExistingItem_ReturnsOkWithMessage()
    {
        // Arrange
        var dto = new ItemCreateDto { ItemName = "Updated" };
        _serviceMock.Setup(s => s.EditAsync(1, It.IsAny<Item>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Edit(1, dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { message = "Item successfully updated." });
    }

    [Fact]
    public async Task Edit_NonExistingItem_ReturnsNotFound()
    {
        // Arrange
        var dto = new ItemCreateDto { ItemName = "Updated" };
        _serviceMock.Setup(s => s.EditAsync(99, It.IsAny<Item>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Edit(99, dto);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "Item with ID 99 not found." });
    }

    [Fact]
    public async Task Delete_ExistingItem_ReturnsOkWithMessage()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { message = "Item successfully deleted." });
    }

    [Fact]
    public async Task Delete_NonExistingItem_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "Item with ID 99 not found." });
    }
}
