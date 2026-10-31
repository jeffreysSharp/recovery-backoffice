using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class PersistenceExtensionsTests
{
    [Fact]
    public void AddRecoveryDatabaseOptions_Should_BindValues_When_ConfigurationIsValid()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["DatabaseOptions:Provider"] = DbProviders.MSSQL,
                ["DatabaseOptions:ConnectionString"] = "Server=localhost;Database=RecoveryTests",
                ["DatabaseOptions:MigrationsAssembly"] = "Recovery.Migrations"
            }).Build();
        var services = new ServiceCollection();

        // Act
        services.AddRecoveryDatabaseOptions(configuration);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        // Assert
        Assert.Equal(DbProviders.MSSQL, options.Provider);
        Assert.Equal("Server=localhost;Database=RecoveryTests", options.ConnectionString);
        Assert.Equal("Recovery.Migrations", options.MigrationsAssembly);
    }

    [Fact]
    public async Task AddRecoveryDatabaseOptions_Should_FailAtStartup_When_ConnectionStringIsMissing()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        using var host = builder.Build();

        await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
    }

    [Fact]
    public void AddRecoveryDatabaseOptions_Should_RejectConfiguration_When_ProviderIsBlank()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["DatabaseOptions:Provider"] = " ",
                ["DatabaseOptions:ConnectionString"] = "Server=localhost;Database=RecoveryTests"
            }).Build();
        var services = new ServiceCollection();
        services.AddRecoveryDatabaseOptions(configuration);
        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(
            () => provider.GetRequiredService<IOptions<DatabaseOptions>>().Value);
    }
}
