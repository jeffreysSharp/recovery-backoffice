using Recovery.BackOffice.Integration.Tests.Infrastructure;

namespace Recovery.BackOffice.Integration.Tests.Tests.Health;

[Collection(RecoveryCollectionDefinition.Name)]
public sealed class HealthProbesTests
{
    private readonly RecoveryWebApplicationFactory _factory;

    public HealthProbesTests(RecoveryWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Liveness_Should_Return200_WithoutDependencyChecks()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/live");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("\"status\":\"Healthy\"");
    }

    [Fact]
    public async Task Readiness_Should_Return200_When_DependenciesHealthy()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/ready");

        // All registered health checks should pass in the integration test setup
        // (PostgreSQL testcontainer is up, Hangfire in-memory storage is running).
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Liveness_Should_NotRequireAuthentication()
    {
        // Probes run anonymously for orchestrators (k8s, ECS) — they can't hold tokens.
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/live");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
