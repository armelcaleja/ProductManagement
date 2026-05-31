using System.Net.Http.Headers;
using System.Net.Http.Json;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Tests.Integration;

public static class IntegrationTestHelper
{
    /// <summary>
    /// Registers a user and logs in, returning an HttpClient with the Bearer token set.
    /// </summary>
    public static async Task<HttpClient> GetAuthenticatedClientAsync(CustomWebApplicationFactory factory, string username = "testuser99", string password = "TestPass123!")
    {
        var client = factory.CreateClient();

        // Register
        var registerDto = new UserRegistrationDto
        {
            Username = username,
            Password = password,
            ConfirmPassword = password
        };
        await client.PostAsJsonAsync("/api/v1/Auth/register", registerDto);

        // Login
        var loginDto = new UserLoginDto
        {
            Username = username,
            Password = password
        };
        var loginResponse = await client.PostAsJsonAsync("/api/v1/Auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();

        // Set auth header
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        return client;
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
