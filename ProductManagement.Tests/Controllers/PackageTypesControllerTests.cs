using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Controllers.V2;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using System.Security.Claims;

namespace ProductManagement.Tests.Controllers;

public class PackageTypesControllerTests
{
    private readonly Mock<IPackageTypeService> _serviceMock;
    private readonly PackageTypesController _controller;

    public PackageTypesControllerTests()
    {
        _serviceMock = new Mock<IPackageTypeService>();
        _controller = new PackageTypesController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithPackageTypes()
    {
        // Arrange
        var types = new List<PackageType>
        {
            new() { PackageTypeId = 1, PackageTypeName = "Box" },
            new() { PackageTypeId = 2, PackageTypeName = "Crate" },
            new() { PackageTypeId = 3, PackageTypeName = "Pallet" }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(types);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<PackageTypeResponseDto>>().Subject;
        response.Should().HaveCount(3);
        response.First().PackageTypeName.Should().Be("Box");
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsOkWithEmptyCollection()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PackageType>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<PackageTypeResponseDto>>().Subject;
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithPackageType()
    {
        // Arrange
        var type = new PackageType { PackageTypeId = 1, PackageTypeName = "Box" };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(type);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<PackageTypeResponseDto>().Subject;
        dto.PackageTypeId.Should().Be(1);
        dto.PackageTypeName.Should().Be("Box");
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((PackageType?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Add_ValidPackageType_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new PackageTypeCreateDto { PackageTypeName = "Envelope" };
        var created = new PackageType { PackageTypeId = 4, PackageTypeName = "Envelope" };
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<PackageType>())).ReturnsAsync(created);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<PackageTypeResponseDto>().Subject;
        response.PackageTypeId.Should().Be(4);
        response.PackageTypeName.Should().Be("Envelope");
    }

    [Fact]
    public async Task Add_SetsCreatedByFromAuthenticatedUser()
    {
        // Arrange
        var dto = new PackageTypeCreateDto { PackageTypeName = "Bag" };
        PackageType? capturedType = null;
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<PackageType>()))
            .Callback<PackageType>(t => capturedType = t)
            .ReturnsAsync(new PackageType { PackageTypeId = 5, PackageTypeName = "Bag" });

        // Act
        await _controller.Add(dto);

        // Assert
        capturedType.Should().NotBeNull();
        capturedType!.CreatedBy.Should().Be(1);
        capturedType.PackageTypeName.Should().Be("Bag");
    }

    [Fact]
    public async Task Edit_ExistingPackageType_ReturnsOkWithMessage()
    {
        // Arrange
        var dto = new PackageTypeCreateDto { PackageTypeName = "Updated Box" };
        _serviceMock.Setup(s => s.EditAsync(1, It.IsAny<PackageType>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Edit(1, dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { message = "PackageType successfully updated." });
    }

    [Fact]
    public async Task Edit_NonExistingPackageType_ReturnsNotFoundWithMessage()
    {
        // Arrange
        var dto = new PackageTypeCreateDto { PackageTypeName = "Updated" };
        _serviceMock.Setup(s => s.EditAsync(99, It.IsAny<PackageType>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Edit(99, dto);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "PackageType with ID 99 not found." });
    }

    [Fact]
    public async Task Delete_ExistingPackageType_ReturnsNoContent()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NonExistingPackageType_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
