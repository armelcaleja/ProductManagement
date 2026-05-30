using System.Net;
using System.Net.Http.Json;
using ProductManagement.DTOs;

namespace ProductManagement.Tests.Integration;

public class ProductsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ProductsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/Products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_Authenticated_ReturnsOk()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "prodgetall", "TestPass123!");

        // Act
        var response = await client.GetAsync("/api/v1/Products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAndGetProduct_V1_FullCycle()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "prodcycle1", "TestPass123!");
        var createDto = new ProductCreateDto { ProductName = "Integration Product", ProductPrice = 500 };

        // Act - Create
        var createResponse = await client.PostAsJsonAsync("/api/v1/Products", createDto);

        // Assert - Create
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductV1ResponseDto>();
        created.Should().NotBeNull();
        created!.ProductName.Should().Be("Integration Product");
        created.ProductPrice.Should().Be(500);

        // Act - Get by ID
        var getResponse = await client.GetAsync($"/api/v1/Products/{created.ProductId}");

        // Assert - Get
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductV1ResponseDto>();
        fetched!.ProductId.Should().Be(created.ProductId);
    }

    [Fact]
    public async Task CreateUpdateDelete_V1_FullLifecycle()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "prodlifecycle", "TestPass123!");

        // Create
        var createDto = new ProductCreateDto { ProductName = "Lifecycle Product", ProductPrice = 100 };
        var createResponse = await client.PostAsJsonAsync("/api/v1/Products", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductV1ResponseDto>();

        // Update
        var updateDto = new ProductCreateDto { ProductName = "Updated Lifecycle", ProductPrice = 200 };
        var updateResponse = await client.PutAsJsonAsync($"/api/v1/Products/{created!.ProductId}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify update
        var getResponse = await client.GetAsync($"/api/v1/Products/{created.ProductId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ProductV1ResponseDto>();
        updated!.ProductName.Should().Be("Updated Lifecycle");
        updated.ProductPrice.Should().Be(200);

        // Delete
        var deleteResponse = await client.DeleteAsync($"/api/v1/Products/{created.ProductId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getDeletedResponse = await client.GetAsync($"/api/v1/Products/{created.ProductId}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "prodnotfound", "TestPass123!");

        // Act
        var response = await client.GetAsync("/api/v1/Products/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProduct_V2_ReturnsWithEmptyPackages()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "prodv2create", "TestPass123!");
        var createDto = new ProductCreateDto { ProductName = "V2 Product", ProductPrice = 300 };

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/Products", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ProductV2ResponseDto>();
        created!.ProductName.Should().Be("V2 Product");
        created.Packages.Should().BeEmpty();
    }
}
