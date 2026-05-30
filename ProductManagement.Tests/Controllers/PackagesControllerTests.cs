using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Controllers.V2;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using System.Security.Claims;

namespace ProductManagement.Tests.Controllers;

public class PackagesControllerTests
{
    private readonly Mock<IPackageService> _serviceMock;
    private readonly PackagesController _controller;

    public PackagesControllerTests()
    {
        _serviceMock = new Mock<IPackageService>();
        _controller = new PackagesController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithPackages()
    {
        // Arrange
        var packages = new List<Package>
        {
            new()
            {
                PackageId = 1,
                ProductId = 1,
                PackageTypeId = 1,
                PackageType = new PackageType { PackageTypeId = 1, PackageTypeName = "Box" },
                Items = new List<Item> { new() { ItemId = 1, ItemName = "Item A" } },
                PackageList = new List<Package>()
            },
            new()
            {
                PackageId = 2,
                ProductId = 1,
                PackageTypeId = 2,
                PackageType = new PackageType { PackageTypeId = 2, PackageTypeName = "Crate" },
                Items = new List<Item>(),
                PackageList = new List<Package>()
            }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(packages);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<PackageResponseDto>>().Subject;
        response.Should().HaveCount(2);
        response.First().PackageTypeName.Should().Be("Box");
        response.First().Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsOkWithEmptyCollection()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Package>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<PackageResponseDto>>().Subject;
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithPackage()
    {
        // Arrange
        var package = new Package
        {
            PackageId = 1,
            ProductId = 1,
            ParentPackageId = null,
            PackageTypeId = 1,
            PackageType = new PackageType { PackageTypeId = 1, PackageTypeName = "Box" },
            Items = new List<Item> { new() { ItemId = 1, ItemName = "Widget" } },
            PackageList = new List<Package>()
        };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(package);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<PackageResponseDto>().Subject;
        dto.PackageId.Should().Be(1);
        dto.PackageTypeName.Should().Be("Box");
        dto.ParentPackageId.Should().BeNull();
        dto.Items.Should().HaveCount(1);
        dto.Items[0].ItemName.Should().Be("Widget");
    }

    [Fact]
    public async Task GetById_WithNestedSubPackages_ReturnsNestedStructure()
    {
        // Arrange
        var package = new Package
        {
            PackageId = 1,
            ProductId = 1,
            PackageTypeId = 1,
            PackageType = new PackageType { PackageTypeId = 1, PackageTypeName = "Crate" },
            Items = new List<Item>(),
            PackageList = new List<Package>
            {
                new()
                {
                    PackageId = 2,
                    ProductId = 1,
                    ParentPackageId = 1,
                    PackageTypeId = 2,
                    PackageType = new PackageType { PackageTypeId = 2, PackageTypeName = "Box" },
                    Items = new List<Item> { new() { ItemId = 3, ItemName = "Sub Item" } },
                    PackageList = new List<Package>()
                }
            }
        };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(package);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<PackageResponseDto>().Subject;
        dto.PackageList.Should().HaveCount(1);
        dto.PackageList[0].ParentPackageId.Should().Be(1);
        dto.PackageList[0].Items[0].ItemName.Should().Be("Sub Item");
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Package?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Add_ValidPackage_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new PackageCreateDto { ProductId = 1, ParentPackageId = null, PackageTypeId = 1 };
        var created = new Package
        {
            PackageId = 5,
            ProductId = 1,
            ParentPackageId = null,
            PackageTypeId = 1,
            PackageType = new PackageType { PackageTypeId = 1, PackageTypeName = "Box" },
            Items = new List<Item>(),
            PackageList = new List<Package>()
        };
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<Package>())).ReturnsAsync(created);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<PackageResponseDto>().Subject;
        response.PackageId.Should().Be(5);
        response.ProductId.Should().Be(1);
        response.ParentPackageId.Should().BeNull();
    }

    [Fact]
    public async Task Add_WithParentPackageId_SetsParentCorrectly()
    {
        // Arrange
        var dto = new PackageCreateDto { ProductId = 1, ParentPackageId = 3, PackageTypeId = 2 };
        Package? capturedPackage = null;
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<Package>()))
            .Callback<Package>(p => capturedPackage = p)
            .ReturnsAsync(new Package
            {
                PackageId = 6,
                ProductId = 1,
                ParentPackageId = 3,
                PackageTypeId = 2,
                PackageType = new PackageType { PackageTypeId = 2, PackageTypeName = "Crate" },
                Items = new List<Item>(),
                PackageList = new List<Package>()
            });

        // Act
        await _controller.Add(dto);

        // Assert
        capturedPackage.Should().NotBeNull();
        capturedPackage!.ParentPackageId.Should().Be(3);
        capturedPackage.CreatedBy.Should().Be(1);
    }

    [Fact]
    public async Task Edit_ExistingPackage_ReturnsNoContent()
    {
        // Arrange
        var dto = new PackageCreateDto { ProductId = 1, ParentPackageId = null, PackageTypeId = 2 };
        _serviceMock.Setup(s => s.EditAsync(1, It.IsAny<Package>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Edit(1, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Edit_NonExistingPackage_ReturnsNotFound()
    {
        // Arrange
        var dto = new PackageCreateDto { ProductId = 1, ParentPackageId = null, PackageTypeId = 2 };
        _serviceMock.Setup(s => s.EditAsync(99, It.IsAny<Package>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Edit(99, dto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ExistingPackage_ReturnsNoContent()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NonExistingPackage_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
