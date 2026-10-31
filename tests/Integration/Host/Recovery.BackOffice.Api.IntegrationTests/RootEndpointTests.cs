using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Recovery.BackOffice.Api.IntegrationTests;

public sealed class RootEndpointTests
{
    [Fact]
    public async Task GetRoot_Should_ReturnHelloWorld_When_HostRunsInProduction()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Production"));
        using var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello World!", content);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
    }
}
