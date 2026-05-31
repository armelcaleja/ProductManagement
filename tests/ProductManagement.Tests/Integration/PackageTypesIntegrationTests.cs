using System.Net;
using System.Net.Http.Json;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Tests.Integration;

public class PackageTypesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PackageTypesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v2/PackageTypes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_Authenticated_ReturnsOk()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "ptgetall", "TestPass123!");

        // Act
        var response = await client.GetAsync("/api/v2/PackageTypes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAndGetPackageType_FullCycle()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "ptcycle1", "TestPass123!");
        var createDto = new PackageTypeCreateDto { PackageTypeName = "Integration Crate" };

        // Act - Create
        var createResponse = await client.PostAsJsonAsync("/api/v2/PackageTypes", createDto);

        // Assert - Create
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();
        created.Should().NotBeNull();
        created!.PackageTypeName.Should().Be("Integration Crate");

        // Act - Get by ID
        var getResponse = await client.GetAsync($"/api/v2/PackageTypes/{created.PackageTypeId}");

        // Assert - Get
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();
        fetched!.PackageTypeId.Should().Be(created.PackageTypeId);
        fetched.PackageTypeName.Should().Be("Integration Crate");
    }

    [Fact]
    public async Task CreateUpdateDelete_FullLifecycle()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "ptlifecycle", "TestPass123!");

        // Create
        var createDto = new PackageTypeCreateDto { PackageTypeName = "Lifecycle Box" };
        var createResponse = await client.PostAsJsonAsync("/api/v2/PackageTypes", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();

        // Update
        var updateDto = new PackageTypeCreateDto { PackageTypeName = "Updated Lifecycle Box" };
        var updateResponse = await client.PutAsJsonAsync($"/api/v2/PackageTypes/{created!.PackageTypeId}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify update
        var getResponse = await client.GetAsync($"/api/v2/PackageTypes/{created.PackageTypeId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<PackageTypeResponseDto>();
        updated!.PackageTypeName.Should().Be("Updated Lifecycle Box");

        // Delete
        var deleteResponse = await client.DeleteAsync($"/api/v2/PackageTypes/{created.PackageTypeId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify deleted
        var getDeletedResponse = await client.GetAsync($"/api/v2/PackageTypes/{created.PackageTypeId}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        // Arrange
        var client = await IntegrationTestHelper.GetAuthenticatedClientAsync(_factory, "ptnotfound", "TestPass123!");

        // Act
        var response = await client.GetAsync("/api/v2/PackageTypes/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
