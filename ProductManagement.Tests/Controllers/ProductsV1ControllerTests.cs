using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Controllers.V1;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;
using System.Security.Claims;

namespace ProductManagement.Tests.Controllers;

public class ProductsV1ControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsV1ControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductsController(_serviceMock.Object);

        // Set up a fake authenticated user
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, ProductName = "Product A", ProductPrice = 50, Packages = new() },
            new() { ProductId = 2, ProductName = "Product B", ProductPrice = 75, Packages = new() }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var items = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductV1ResponseDto>>().Subject;
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithProduct()
    {
        // Arrange
        var product = new Product { ProductId = 1, ProductName = "Test", ProductPrice = 100, Packages = new() };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<ProductV1ResponseDto>().Subject;
        dto.ProductId.Should().Be(1);
        dto.ProductName.Should().Be("Test");
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
    public async Task Add_ValidProduct_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new ProductCreateDto { ProductName = "New", ProductPrice = 200 };
        var created = new Product { ProductId = 5, ProductName = "New", ProductPrice = 200, Packages = new() };
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<Product>())).ReturnsAsync(created);

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<ProductV1ResponseDto>().Subject;
        response.ProductId.Should().Be(5);
        response.ProductName.Should().Be("New");
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
    public async Task Delete_ExistingProduct_ReturnsNoContent()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
