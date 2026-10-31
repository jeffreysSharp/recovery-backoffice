using Microsoft.Extensions.Logging.Abstractions;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Xunit;

namespace Recovery.BackOffice.DbMigrator.UnitTests;

public sealed class MigrationRunnerTests
{
    [Fact]
    public async Task RunAsync_Should_PrepareCatalogBeforeTenants_When_MigrationsAreApplied()
    {
        var operations = new RecordingOperations();
        using var cancellation = new CancellationTokenSource();

        await CreateRunner(operations).RunAsync(cancellation.Token);

        Assert.Equal(["catalog", "root", "tenants", "identity:root", "identity:company-a"], operations.Calls);
        Assert.All(operations.Tokens, token => Assert.Equal(cancellation.Token, token));
    }

    [Theory]
    [InlineData("catalog", 1)]
    [InlineData("root", 2)]
    [InlineData("tenants", 3)]
    public async Task RunAsync_Should_StopBeforeTenantMigration_When_CatalogPreparationFails(string step, int expectedCalls)
    {
        var operations = new RecordingOperations { FailureStep = step };

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateRunner(operations).RunAsync());

        Assert.Equal(expectedCalls, operations.Calls.Count);
        Assert.DoesNotContain(operations.Calls, call => call.StartsWith("identity:", StringComparison.Ordinal));
    }

    [Fact]
    public async Task RunAsync_Should_StopFollowingTenants_When_OneTenantFails()
    {
        var operations = new RecordingOperations { FailureStep = "identity:root" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateRunner(operations).RunAsync());

        Assert.DoesNotContain("identity:company-a", operations.Calls);
    }

    [Fact]
    public async Task RunAsync_Should_AvoidOperations_When_AlreadyCancelled()
    {
        var operations = new RecordingOperations();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => CreateRunner(operations).RunAsync(cancellation.Token));

        Assert.Empty(operations.Calls);
    }

    [Fact]
    public async Task RunAsync_Should_StopFollowingTenants_When_CancelledBetweenTenants()
    {
        using var cancellation = new CancellationTokenSource();
        var operations = new RecordingOperations
        {
            OnStep = step => { if (step == "identity:root") cancellation.Cancel(); }
        };

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => CreateRunner(operations).RunAsync(cancellation.Token));

        Assert.DoesNotContain("identity:company-a", operations.Calls);
    }

    [Fact]
    public async Task RunAsync_Should_RejectEmptyCatalog_When_RootInitializationProducesNoTenant()
    {
        var operations = new RecordingOperations { Tenants = [] };

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateRunner(operations).RunAsync());

        Assert.Equal(["catalog", "root", "tenants"], operations.Calls);
    }

    private static MigrationRunner CreateRunner(IMigrationOperations operations) =>
        new(operations, NullLogger<MigrationRunner>.Instance);

    private sealed class RecordingOperations : IMigrationOperations
    {
        public List<string> Calls { get; } = [];
        public List<CancellationToken> Tokens { get; } = [];
        public string? FailureStep { get; init; }
        public Action<string>? OnStep { get; init; }
        public IReadOnlyList<AppTenantInfo> Tenants { get; init; } =
            [new("root", "root"), new("company-a", "company-a")];

        public Task MigrateCatalogAsync(CancellationToken cancellationToken) => Record("catalog", cancellationToken);
        public Task EnsureRootTenantAsync(CancellationToken cancellationToken) => Record("root", cancellationToken);

        public async Task<IReadOnlyList<AppTenantInfo>> GetTenantsAsync(CancellationToken cancellationToken)
        {
            await Record("tenants", cancellationToken);
            return Tenants;
        }

        public Task MigrateIdentityAsync(AppTenantInfo tenant, CancellationToken cancellationToken) =>
            Record($"identity:{tenant.Id}", cancellationToken);

        private Task Record(string step, CancellationToken token)
        {
            Calls.Add(step);
            Tokens.Add(token);
            OnStep?.Invoke(step);
            return step == FailureStep
                ? Task.FromException(new InvalidOperationException("Simulated migration failure."))
                : Task.CompletedTask;
        }
    }
}
