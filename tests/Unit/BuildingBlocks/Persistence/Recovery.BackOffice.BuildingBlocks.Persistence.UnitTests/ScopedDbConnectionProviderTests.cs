using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class ScopedDbConnectionProviderTests
{
    private const string ConnectionString = "Server=localhost;Database=RecoveryTests;Integrated Security=True";

    [Fact]
    public void GetConnection_Should_ReuseUnopenedConnection_When_ConnectionStringIsTheSame()
    {
        // Arrange
        using var provider = new ScopedDbConnectionProvider();

        // Act
        var first = provider.GetConnection(DbProviders.MSSQL, ConnectionString);
        var second = provider.GetConnection("mssql", ConnectionString);

        // Assert
        Assert.Same(first, second);
        Assert.Equal(ConnectionState.Closed, first.State);
    }

    [Fact]
    public void GetConnection_Should_CreateDifferentConnections_When_ConnectionStringsDiffer()
    {
        using var provider = new ScopedDbConnectionProvider();

        var first = provider.GetConnection(DbProviders.MSSQL, ConnectionString);
        var second = provider.GetConnection(DbProviders.MSSQL,
            "Server=localhost;Database=AnotherDatabase;Integrated Security=True");

        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetConnection_Should_RejectUnsupportedProvider_When_ConnectionIsAlreadyCached()
    {
        using var provider = new ScopedDbConnectionProvider();
        provider.GetConnection(DbProviders.MSSQL, ConnectionString);

        Assert.Throws<InvalidOperationException>(
            () => provider.GetConnection("POSTGRESQL", ConnectionString));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GetConnection_Should_RejectConnectionString_When_ValueIsEmpty(string connectionString)
    {
        using var provider = new ScopedDbConnectionProvider();

        Assert.Throws<ArgumentException>(
            () => provider.GetConnection(DbProviders.MSSQL, connectionString));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Dispose_Should_ReleaseAllConnectionsOnce_When_CalledRepeatedly(bool useAsync)
    {
        // Arrange
        var provider = new ScopedDbConnectionProvider();
        var first = provider.GetConnection(DbProviders.MSSQL, ConnectionString);
        var second = provider.GetConnection(DbProviders.MSSQL,
            "Server=localhost;Database=AnotherDatabase;Integrated Security=True");
        var firstDisposals = 0;
        var secondDisposals = 0;
        first.Disposed += (_, _) => firstDisposals++;
        second.Disposed += (_, _) => secondDisposals++;

        // Act
        if (useAsync)
        {
            await provider.DisposeAsync();
        }
        else
        {
            provider.Dispose();
        }

        provider.Dispose();
        await provider.DisposeAsync();

        // Assert
        Assert.Equal(1, firstDisposals);
        Assert.Equal(1, secondDisposals);
        Assert.Throws<ObjectDisposedException>(
            () => provider.GetConnection(DbProviders.MSSQL, ConnectionString));
    }

    [Fact]
    public void GetConnection_Should_IsolateConnectionsAndLifetime_When_ScopesDiffer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddRecoveryDatabaseOptions(new ConfigurationBuilder().Build());
        using var container = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        var firstScope = container.CreateScope();
        using var secondScope = container.CreateScope();
        var firstProvider = firstScope.ServiceProvider.GetRequiredService<IScopedDbConnectionProvider>();
        var secondProvider = secondScope.ServiceProvider.GetRequiredService<IScopedDbConnectionProvider>();
        var first = firstProvider.GetConnection(DbProviders.MSSQL, ConnectionString);
        var second = secondProvider.GetConnection(DbProviders.MSSQL, ConnectionString);
        var firstDisposed = false;
        var secondDisposed = false;
        first.Disposed += (_, _) => firstDisposed = true;
        second.Disposed += (_, _) => secondDisposed = true;

        // Act
        firstScope.Dispose();

        // Assert
        Assert.Same(secondProvider,
            secondScope.ServiceProvider.GetRequiredService<IScopedDbConnectionProvider>());
        Assert.NotSame(firstProvider, secondProvider);
        Assert.NotSame(first, second);
        Assert.True(firstDisposed);
        Assert.False(secondDisposed);
        Assert.Same(second, secondProvider.GetConnection(DbProviders.MSSQL, ConnectionString));
    }
}
