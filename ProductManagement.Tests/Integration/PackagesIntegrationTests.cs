using System.Net;
using System.Net.Http.Json;
using ProductManagement.DTOs;

namespace ProductManagement.Tests.Integration;

public class PackagesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PackagesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v2/Packages");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreatePackage_WithProductAndType_ReturnsCreated()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "pkgcreate1", "TestPass123!");

        // Create a product first
        var productDto = new ProductCreateDto { ProductName = "Pkg Test Product", ProductPrice = 100 };
        var productResponse = await client.PostAsJsonAsync("/api/v2/Products", productDto);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductV2ResponseDto>();

        // Create a package type
        var typeDto = new PackageTypeCreateDto { PackageTypeName = "Test Box" };
        var typeResponse = await client.PostAsJsonAsync("/api/v2/PackageTypes", typeDto);
        var packageType = await typeResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();

        // Act - Create package
        var packageDto = new PackageCreateDto
        {
            ProductId = product!.ProductId,
            ParentPackageId = null,
            PackageTypeId = packageType!.PackageTypeId
        };
        var response = await client.PostAsJsonAsync("/api/v2/Packages", packageDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<PackageResponseDto>();
        created!.ProductId.Should().Be(product.ProductId);
        created.PackageTypeId.Should().Be(packageType.PackageTypeId);
        created.ParentPackageId.Should().BeNull();
    }

    [Fact]
    public async Task CreateNestedPackage_WithParent_ReturnsCreated()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "pkgnested1", "TestPass123!");

        // Create product
        var productResponse = await client.PostAsJsonAsync("/api/v2/Products",
            new ProductCreateDto { ProductName = "Nested Pkg Product", ProductPrice = 50 });
        var product = await productResponse.Content.ReadFromJsonAsync<ProductV2ResponseDto>();

        // Create package type
        var typeResponse = await client.PostAsJsonAsync("/api/v2/PackageTypes",
            new PackageTypeCreateDto { PackageTypeName = "Outer Crate" });
        var packageType = await typeResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();

        // Create parent package
        var parentResponse = await client.PostAsJsonAsync("/api/v2/Packages",
            new PackageCreateDto { ProductId = product!.ProductId, PackageTypeId = packageType!.PackageTypeId });
        var parent = await parentResponse.Content.ReadFromJsonAsync<PackageResponseDto>();

        // Act - Create child package
        var childDto = new PackageCreateDto
        {
            ProductId = product.ProductId,
            ParentPackageId = parent!.PackageId,
            PackageTypeId = packageType.PackageTypeId
        };
        var response = await client.PostAsJsonAsync("/api/v2/Packages", childDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var child = await response.Content.ReadFromJsonAsync<PackageResponseDto>();
        child!.ParentPackageId.Should().Be(parent.PackageId);
    }

    [Fact]
    public async Task DeleteProduct_WithPackages_SucceedsInMemory_ButFailsOnRealDb()
    {
        // NOTE: InMemory database does NOT enforce FK constraints.
        // On a real SQL Server, this would return 500 (or a handled error) due to
        // the REFERENCE constraint between Products and Packages.
        // This test documents the current behavior and verifies the endpoint works.

        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "pkgdelconflict", "TestPass123!");

        // Create product
        var productResponse = await client.PostAsJsonAsync("/api/v2/Products",
            new ProductCreateDto { ProductName = "Delete Conflict Product", ProductPrice = 75 });
        var product = await productResponse.Content.ReadFromJsonAsync<ProductV2ResponseDto>();

        // Create package type
        var typeResponse = await client.PostAsJsonAsync("/api/v2/PackageTypes",
            new PackageTypeCreateDto { PackageTypeName = "Conflict Box" });
        var packageType = await typeResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();

        // Create package linked to product
        await client.PostAsJsonAsync("/api/v2/Packages",
            new PackageCreateDto { ProductId = product!.ProductId, PackageTypeId = packageType!.PackageTypeId });

        // Act - Try to delete the product that has packages
        var deleteResponse = await client.DeleteAsync($"/api/v2/Products/{product.ProductId}");

        // Assert - InMemory allows this (no FK enforcement), real DB would block it
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AssignItemToPackage_FullFlow()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "pkgassign1", "TestPass123!");

        // Create product
        var productResponse = await client.PostAsJsonAsync("/api/v2/Products",
            new ProductCreateDto { ProductName = "Assign Test Product", ProductPrice = 60 });
        var product = await productResponse.Content.ReadFromJsonAsync<ProductV2ResponseDto>();

        // Create package type
        var typeResponse = await client.PostAsJsonAsync("/api/v2/PackageTypes",
            new PackageTypeCreateDto { PackageTypeName = "Assign Box" });
        var packageType = await typeResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();

        // Create package
        var packageResponse = await client.PostAsJsonAsync("/api/v2/Packages",
            new PackageCreateDto { ProductId = product!.ProductId, PackageTypeId = packageType!.PackageTypeId });
        var package = await packageResponse.Content.ReadFromJsonAsync<PackageResponseDto>();

        // Create item
        var itemResponse = await client.PostAsJsonAsync("/api/v2/Items",
            new ItemCreateDto { ItemName = "Test Widget" });
        var item = await itemResponse.Content.ReadFromJsonAsync<ItemResponseDto>();

        // Act - Assign item to package
        var assignDto = new PackageItemAssignDto { PackageId = package!.PackageId, ItemId = item!.ItemId };
        var assignResponse = await client.PostAsJsonAsync("/api/v2/Items/assign-to-package", assignDto);

        // Assert
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
