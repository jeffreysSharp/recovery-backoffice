using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class DbContextRegistrationTests
{
    private const string ConnectionString = "Server=localhost;Database=RecoveryTests;Integrated Security=True";

    [Fact]
    public void AddRecoveryDbContext_Should_ShareConnection_When_ContextTypesUseTheSameScope()
    {
        // Arrange
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();

        // Act
        var first = scope.ServiceProvider.GetRequiredService<FirstContext>();
        var second = scope.ServiceProvider.GetRequiredService<SecondContext>();
        var connections = scope.ServiceProvider.GetRequiredService<IScopedDbConnectionProvider>();
        var connection = first.Database.GetDbConnection();

        // Assert
        Assert.Same(first, scope.ServiceProvider.GetRequiredService<FirstContext>());
        Assert.Same(connection, second.Database.GetDbConnection());
        Assert.Same(connection, connections.GetConnection("MSSQL", ConnectionString));
        Assert.Equal(ConnectionState.Closed, connection.State);
        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", first.Database.ProviderName);
        Assert.True(first.Database.CreateExecutionStrategy().RetriesOnFailure);
    }

    [Fact]
    public void AddRecoveryDbContext_Should_IsolateContextsAndConnections_When_ScopesDiffer()
    {
        using var host = CreateHost();
        using var firstScope = host.Services.CreateScope();
        using var secondScope = host.Services.CreateScope();

        var first = firstScope.ServiceProvider.GetRequiredService<FirstContext>();
        var second = secondScope.ServiceProvider.GetRequiredService<FirstContext>();

        Assert.NotSame(first, second);
        Assert.NotSame(first.Database.GetDbConnection(), second.Database.GetDbConnection());
    }

    [Theory]
    [InlineData("Development", true)]
    [InlineData("Production", false)]
    public void AddRecoveryDbContext_Should_ApplyEnvironmentDiagnostics_When_OptionsAreResolved(
        string environmentName, bool diagnosticsEnabled)
    {
        using var host = CreateHost(environmentName);
        using var scope = host.Services.CreateScope();

        var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<FirstContext>>();
        var coreOptions = options.FindExtension<CoreOptionsExtension>();

        Assert.NotNull(coreOptions);
        Assert.Equal(diagnosticsEnabled, coreOptions.IsSensitiveDataLoggingEnabled);
        Assert.Equal(diagnosticsEnabled, coreOptions.DetailedErrorsEnabled);
    }

    [Fact]
    public void AddRecoveryDbContext_Should_RejectResolution_When_DatabaseConfigurationIsInvalid()
    {
        using var host = CreateHost(connectionString: string.Empty);
        using var scope = host.Services.CreateScope();

        Assert.Throws<OptionsValidationException>(
            () => scope.ServiceProvider.GetRequiredService<FirstContext>());
    }

    [Fact]
    public void AddRecoveryDbContext_Should_ReleaseConnection_When_ScopeIsDisposed()
    {
        // Arrange
        using var host = CreateHost();
        var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FirstContext>();
        var connection = context.Database.GetDbConnection();
        var disposalCount = 0;
        connection.Disposed += (_, _) => disposalCount++;

        // Act
        scope.Dispose();

        // Assert
        Assert.Equal(1, disposalCount);
    }

    private static IHost CreateHost(
        string environmentName = "Production",
        string connectionString = ConnectionString)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = environmentName
        });
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = "MSSQL",
            ["DatabaseOptions:ConnectionString"] = connectionString,
            ["DatabaseOptions:MigrationsAssembly"] = typeof(DbContextRegistrationTests).Assembly.GetName().Name
        });
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        builder.Services.AddRecoveryDbContext<FirstContext>();
        builder.Services.AddRecoveryDbContext<SecondContext>();

        return builder.Build();
    }

    public sealed class FirstContext(DbContextOptions<FirstContext> options) : DbContext(options);

    public sealed class SecondContext(DbContextOptions<SecondContext> options) : DbContext(options);
}
