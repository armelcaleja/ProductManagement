using System.Net;
using System.Net.Http.Json;
using ProductManagement.DTOs;

namespace ProductManagement.Tests.Integration;

public class ItemsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ItemsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v2/Items");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_Authenticated_ReturnsOk()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "itemgetall", "TestPass123!");

        // Act
        var response = await client.GetAsync("/api/v2/Items");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAndGetItem_FullCycle()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "itemcycle1", "TestPass123!");
        var createDto = new ItemCreateDto { ItemName = "Integration Widget" };

        // Act - Create
        var createResponse = await client.PostAsJsonAsync("/api/v2/Items", createDto);

        // Assert - Create
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ItemResponseDto>();
        created.Should().NotBeNull();
        created!.ItemName.Should().Be("Integration Widget");

        // Act - Get by ID
        var getResponse = await client.GetAsync($"/api/v2/Items/{created.ItemId}");

        // Assert - Get
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ItemResponseDto>();
        fetched!.ItemId.Should().Be(created.ItemId);
        fetched.ItemName.Should().Be("Integration Widget");
    }

    [Fact]
    public async Task CreateUpdateDelete_FullLifecycle()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "itemlifecycle", "TestPass123!");

        // Create
        var createDto = new ItemCreateDto { ItemName = "Lifecycle Item" };
        var createResponse = await client.PostAsJsonAsync("/api/v2/Items", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ItemResponseDto>();

        // Update
        var updateDto = new ItemCreateDto { ItemName = "Updated Lifecycle Item" };
        var updateResponse = await client.PutAsJsonAsync($"/api/v2/Items/{created!.ItemId}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify update
        var getResponse = await client.GetAsync($"/api/v2/Items/{created.ItemId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ItemResponseDto>();
        updated!.ItemName.Should().Be("Updated Lifecycle Item");

        // Delete
        var deleteResponse = await client.DeleteAsync($"/api/v2/Items/{created.ItemId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deleted
        var getDeletedResponse = await client.GetAsync($"/api/v2/Items/{created.ItemId}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "itemnotfound", "TestPass123!");

        // Act
        var response = await client.GetAsync("/api/v2/Items/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignToPackage_ValidIds_ReturnsOk()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "itemassign1", "TestPass123!");

        // Create product
        var productResponse = await client.PostAsJsonAsync("/api/v2/Products",
            new ProductCreateDto { ProductName = "Item Assign Product", ProductPrice = 80 });
        var product = await productResponse.Content.ReadFromJsonAsync<ProductV2ResponseDto>();

        // Create package type
        var typeResponse = await client.PostAsJsonAsync("/api/v2/PackageTypes",
            new PackageTypeCreateDto { PackageTypeName = "Assign Test Box" });
        var packageType = await typeResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();

        // Create package
        var packageResponse = await client.PostAsJsonAsync("/api/v2/Packages",
            new PackageCreateDto { ProductId = product!.ProductId, PackageTypeId = packageType!.PackageTypeId });
        var package = await packageResponse.Content.ReadFromJsonAsync<PackageResponseDto>();

        // Create item
        var itemResponse = await client.PostAsJsonAsync("/api/v2/Items",
            new ItemCreateDto { ItemName = "Assignable Widget" });
        var item = await itemResponse.Content.ReadFromJsonAsync<ItemResponseDto>();

        // Act - Assign
        var assignDto = new PackageItemAssignDto { PackageId = package!.PackageId, ItemId = item!.ItemId };
        var assignResponse = await client.PostAsJsonAsync("/api/v2/Items/assign-to-package", assignDto);

        // Assert
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
