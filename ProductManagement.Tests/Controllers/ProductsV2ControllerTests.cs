using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Controllers.V2;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using System.Security.Claims;

namespace ProductManagement.Tests.Controllers;

public class ProductsV2ControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsV2ControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductsController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithProductsIncludingPackages()
    {
        // Arrange
        var products = new List<Product>
        {
            new()
            {
                ProductId = 1,
                ProductName = "Product A",
                ProductPrice = 50,
                Packages = new List<Package>
                {
                    new()
                    {
                        PackageId = 1,
                        ProductId = 1,
                        PackageTypeId = 1,
                        PackageType = new PackageType { PackageTypeId = 1, PackageTypeName = "Box" },
                        Items = new List<Item> { new() { ItemId = 1, ItemName = "Item 1" } },
                        PackageList = new List<Package>()
                    }
                }
            },
            new() { ProductId = 2, ProductName = "Product B", ProductPrice = 75, Packages = new() }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var items = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductV2ResponseDto>>().Subject;
        items.Should().HaveCount(2);

        var first = items.First();
        first.Packages.Should().HaveCount(1);
        first.Packages[0].PackageTypeName.Should().Be("Box");
        first.Packages[0].Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsOkWithEmptyCollection()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Product>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var items = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductV2ResponseDto>>().Subject;
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithNestedPackages()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            ProductName = "Test Product",
            ProductPrice = 100,
            Packages = new List<Package>
            {
                new()
                {
                    PackageId = 1,
                    ProductId = 1,
                    PackageTypeId = 2,
                    PackageType = new PackageType { PackageTypeId = 2, PackageTypeName = "Crate" },
                    Items = new List<Item>(),
                    PackageList = new List<Package>
                    {
                        new()
                        {
                            PackageId = 2,
                            ProductId = 1,
                            ParentPackageId = 1,
                            PackageTypeId = 1,
                            PackageType = new PackageType { PackageTypeId = 1, PackageTypeName = "Box" },
                            Items = new List<Item> { new() { ItemId = 5, ItemName = "Nested Item" } },
                            PackageList = new List<Package>()
                        }
                    }
                }
            }
        };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<ProductV2ResponseDto>().Subject;
        dto.ProductId.Should().Be(1);
        dto.ProductName.Should().Be("Test Product");
        dto.Packages.Should().HaveCount(1);
        dto.Packages[0].PackageList.Should().HaveCount(1);
        dto.Packages[0].PackageList[0].Items.Should().HaveCount(1);
        dto.Packages[0].PackageList[0].Items[0].ItemName.Should().Be("Nested Item");
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Add_ValidProduct_ReturnsCreatedAtActionWithV2Response()
    {
        // Arrange
        var dto = new ProductCreateDto { ProductName = "New Product", ProductPrice = 200 };
        var created = new Product
        {
            ProductId = 5,
            ProductName = "New Product",
            ProductPrice = 200,
            Packages = new List<Package>()
        };
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<Product>())).ReturnsAsync(created);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<ProductV2ResponseDto>().Subject;
        response.ProductId.Should().Be(5);
        response.ProductName.Should().Be("New Product");
        response.ProductPrice.Should().Be(200);
        response.Packages.Should().BeEmpty();
    }

    [Fact]
    public async Task Add_SetsCreatedByFromAuthenticatedUser()
    {
        // Arrange
        var dto = new ProductCreateDto { ProductName = "User Product", ProductPrice = 50 };
        Product? capturedProduct = null;
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p)
            .ReturnsAsync(new Product { ProductId = 1, ProductName = "User Product", ProductPrice = 50, Packages = new() });

        // Act
        await _controller.Add(dto);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct!.CreatedBy.Should().Be(1); // From the ClaimTypes.NameIdentifier set in constructor
    }

    [Fact]
    public async Task Edit_ExistingProduct_ReturnsOkWithMessage()
    {
        // Arrange
        var dto = new ProductCreateDto { ProductName = "Updated", ProductPrice = 300 };
        _serviceMock.Setup(s => s.EditAsync(1, It.IsAny<Product>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Edit(1, dto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { message = "Product successfully updated." });
    }

    [Fact]
    public async Task Edit_NonExistingProduct_ReturnsNotFoundWithMessage()
    {
        // Arrange
        var dto = new ProductCreateDto { ProductName = "Updated", ProductPrice = 300 };
        _serviceMock.Setup(s => s.EditAsync(99, It.IsAny<Product>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Edit(99, dto);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "Product with ID 99 not found." });
    }

    [Fact]
    public async Task Delete_ExistingProduct_ReturnsOkWithMessage()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { message = "Product successfully deleted." });
    }

    [Fact]
    public async Task Delete_NonExistingProduct_ReturnsNotFoundWithMessage()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "Product with ID 99 not found." });
    }
}
