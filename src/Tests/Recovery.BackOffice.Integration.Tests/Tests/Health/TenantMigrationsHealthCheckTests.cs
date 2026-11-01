using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Testcontainers.MsSql;

namespace Recovery.BackOffice.Integration.Tests.Tests.Health;

/// <summary>
/// Direct unit tests for <see cref="TenantMigrationsHealthCheck"/> — the production-readiness
/// gate added when API-side auto-migration was removed. Asserts that the check returns
/// <c>Unhealthy</c> against an unmigrated database and <c>Healthy</c> after the schema is
/// brought to head (the exact transition <c>Recovery.BackOffice.DbMigrator</c> performs in production).
/// Uses a real SqlServer testcontainer + an in-memory <see cref="IMultiTenantStore{TTenantInfo}"/>
/// to keep the test independent of the shared <c>RecoveryWebApplicationFactory</c> — avoiding
/// the global Hangfire state that would otherwise leak across test classes.
/// </summary>
public sealed class TenantMigrationsHealthCheckTests : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Integration_Test_Only123!")
        .WithAutoRemove(true)
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync() { await _sqlServer.StartAsync(); await CreateDatabaseAsync(); }

    public Task DisposeAsync() => _sqlServer.DisposeAsync().AsTask();

    [Fact]
    public async Task CheckHealthAsync_Should_TransitionFromUnhealthyToHealthy_AsMigrationsApply()
    {
        await using var provider = BuildServiceProvider();
        var check = new TenantMigrationsHealthCheck(provider.GetRequiredService<IServiceScopeFactory>());
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("db:tenants-migrations", check, HealthStatus.Unhealthy, tags: null)
        };

        // ── Before migrations ────────────────────────────────────────────
        // Every migration is pending → check must be Unhealthy so /health/ready returns 503.
        var beforeResult = await check.CheckHealthAsync(context, CancellationToken.None);
        beforeResult.Status.ShouldBe(HealthStatus.Unhealthy);
        beforeResult.Description.ShouldNotBeNull();
        beforeResult.Description.ShouldContain("Recovery.BackOffice.DbMigrator");

        // ── Apply migrations (what DbMigrator does in production) ────────
        using (var scope = provider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await db.Database.MigrateAsync();
        }

        // ── After migrations ─────────────────────────────────────────────
        var afterResult = await check.CheckHealthAsync(context, CancellationToken.None);
        afterResult.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_Should_ReturnUnhealthy_When_TenantProbeThrows()
    {
        // A connection-string pointed at a port nothing listens on simulates an
        // unreachable tenant DB. The per-tenant try/catch must convert that to an
        // Unhealthy aggregate result, not propagate the exception.
        await using var provider = BuildServiceProvider(badConnectionString: true);
        var check = new TenantMigrationsHealthCheck(provider.GetRequiredService<IServiceScopeFactory>());
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("db:tenants-migrations", check, HealthStatus.Unhealthy, tags: null)
        };

        var result = await check.CheckHealthAsync(context, CancellationToken.None);
        result.Status.ShouldBe(HealthStatus.Unhealthy);
        result.Description.ShouldNotBeNull();
        result.Description.ShouldContain("error probing tenant", Case.Insensitive);
    }

    private ServiceProvider BuildServiceProvider(bool badConnectionString = false)
    {
        var connectionString = badConnectionString
            // Reserved discard port — refuses connections immediately.
            ? "Server=127.0.0.1,1;Database=does_not_exist;User Id=sa;Password=NotARealPassword123!;Connect Timeout=2;TrustServerCertificate=true"
            : GetDatabaseConnectionString();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IMultiTenantStore<AppTenantInfo>>(new SingleTenantStore());
        services.AddScoped<IMultiTenantContextSetter, FakeMultiTenantContextSetter>();
        services.AddDbContext<TenantDbContext>(opts =>
            opts.UseSqlServer(connectionString, b => b.MigrationsAssembly("Recovery.BackOffice.Migrations.SqlServer")));
        return services.BuildServiceProvider();
    }

    /// <summary>Minimal in-memory tenant store — returns one tenant for the test.</summary>
    private sealed class SingleTenantStore : IMultiTenantStore<AppTenantInfo>
    {
        private readonly AppTenantInfo _tenant = new(
            MultitenancyConstants.Root.Id,
            MultitenancyConstants.Root.Name,
            connectionString: string.Empty,
            MultitenancyConstants.Root.EmailAddress,
            issuer: MultitenancyConstants.Root.Issuer);

        public Task<bool> AddAsync(AppTenantInfo tenantInfo) => Task.FromResult(true);
        public Task<bool> RemoveAsync(string identifier) => Task.FromResult(true);
        public Task<bool> UpdateAsync(AppTenantInfo tenantInfo) => Task.FromResult(true);
        public Task<AppTenantInfo?> GetByIdentifierAsync(string identifier) => Task.FromResult<AppTenantInfo?>(_tenant);
        public Task<AppTenantInfo?> GetAsync(string id) => Task.FromResult<AppTenantInfo?>(_tenant);
        public Task<IEnumerable<AppTenantInfo>> GetAllAsync() => Task.FromResult<IEnumerable<AppTenantInfo>>([_tenant]);
        public Task<IEnumerable<AppTenantInfo>> GetAllAsync(int take, int skip) => Task.FromResult<IEnumerable<AppTenantInfo>>([_tenant]);
    }

    /// <summary>
    /// Fake <see cref="IMultiTenantContextSetter"/> that just records the value. The health
    /// check writes to this before resolving <see cref="TenantDbContext"/>; reads of the
    /// context are not exercised by <c>GetPendingMigrationsAsync</c>.
    /// </summary>
    private sealed class FakeMultiTenantContextSetter : IMultiTenantContextSetter
    {
        public IMultiTenantContext MultiTenantContext { get; set; } = new MultiTenantContext<AppTenantInfo>(
            new AppTenantInfo("placeholder", "placeholder", string.Empty, "x@x", null));
    }

    private string GetDatabaseConnectionString() => new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
    { InitialCatalog = "RecoveryMigrationsCheck" }.ConnectionString;

    private async Task CreateDatabaseAsync()
    {
        await using var connection = new Microsoft.Data.SqlClient.SqlConnection(_sqlServer.GetConnectionString());
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "CREATE DATABASE [RecoveryMigrationsCheck]";
        await command.ExecuteNonQueryAsync();
    }
}
