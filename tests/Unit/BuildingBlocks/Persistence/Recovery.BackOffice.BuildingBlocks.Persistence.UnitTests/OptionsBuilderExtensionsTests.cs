using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Data.SqlClient;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class OptionsBuilderExtensionsTests
{
    private const string ConnectionString = "Server=localhost;Database=RecoveryTests;Integrated Security=True";

    [Theory]
    [InlineData(DbProviders.MSSQL)]
    [InlineData("mssql")]
    public void ConfigureRecoveryDatabase_Should_SelectSqlServerAndRetries_When_ProviderIsSupported(string provider)
    {
        // Arrange
        var builder = new DbContextOptionsBuilder();
        var migrationsAssembly = typeof(OptionsBuilderExtensionsTests).Assembly.GetName().Name!;

        // Act
        builder.ConfigureRecoveryDatabase(provider, ConnectionString, migrationsAssembly, false);
        using var context = new DbContext(builder.Options);

        // Assert
        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
        var connection = Assert.IsType<SqlConnection>(context.Database.GetDbConnection());
        Assert.Equal("localhost", connection.DataSource);
        Assert.Equal("RecoveryTests", connection.Database);
        Assert.True(new SqlConnectionStringBuilder(connection.ConnectionString).IntegratedSecurity);
        Assert.True(context.Database.CreateExecutionStrategy().RetriesOnFailure);
        Assert.Equal(migrationsAssembly, context.GetService<IMigrationsAssembly>().Assembly.GetName().Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfigureRecoveryDatabase_Should_SetDiagnosticsForEnvironment_When_OptionsAreConfigured(bool isDevelopment)
    {
        var builder = new DbContextOptionsBuilder();

        builder.ConfigureRecoveryDatabase(DbProviders.MSSQL, ConnectionString,
            typeof(OptionsBuilderExtensionsTests).Assembly.GetName().Name!, isDevelopment);

        var options = builder.Options.FindExtension<CoreOptionsExtension>();
        Assert.NotNull(options);
        Assert.Equal(isDevelopment, options.IsSensitiveDataLoggingEnabled);
        Assert.Equal(isDevelopment, options.DetailedErrorsEnabled);
    }

    [Fact]
    public void ConfigureRecoveryDatabase_Should_PreserveSharedConnection_When_ContextsAreDisposed()
    {
        // Arrange
        using var provider = new ScopedDbConnectionProvider();
        var connection = provider.GetConnection(DbProviders.MSSQL, ConnectionString);
        var disposalCount = 0;
        connection.Disposed += (_, _) => disposalCount++;
        var firstBuilder = new DbContextOptionsBuilder();
        var secondBuilder = new DbContextOptionsBuilder();
        var assemblyName = typeof(OptionsBuilderExtensionsTests).Assembly.GetName().Name!;
        firstBuilder.ConfigureRecoveryDatabase(DbProviders.MSSQL, connection, assemblyName, false);
        secondBuilder.ConfigureRecoveryDatabase(DbProviders.MSSQL, connection, assemblyName, false);

        // Act
        using (var firstContext = new DbContext(firstBuilder.Options))
        using (var secondContext = new DbContext(secondBuilder.Options))
        {
            Assert.Same(connection, firstContext.Database.GetDbConnection());
            Assert.Same(connection, secondContext.Database.GetDbConnection());
            Assert.True(firstContext.Database.CreateExecutionStrategy().RetriesOnFailure);
            Assert.Equal(assemblyName,
                firstContext.GetService<IMigrationsAssembly>().Assembly.GetName().Name);
        }

        // Assert
        Assert.Equal(0, disposalCount);
        provider.Dispose();
        Assert.Equal(1, disposalCount);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfigureRecoveryDatabase_Should_ConfigureDiagnostics_When_ConnectionIsProvided(bool isDevelopment)
    {
        using var connection = new SqlConnection(ConnectionString);
        var builder = new DbContextOptionsBuilder();

        builder.ConfigureRecoveryDatabase(DbProviders.MSSQL, connection,
            typeof(OptionsBuilderExtensionsTests).Assembly.GetName().Name!, isDevelopment);

        var options = builder.Options.FindExtension<CoreOptionsExtension>();
        Assert.NotNull(options);
        Assert.Equal(isDevelopment, options.IsSensitiveDataLoggingEnabled);
        Assert.Equal(isDevelopment, options.DetailedErrorsEnabled);
    }

    [Fact]
    public void ConfigureRecoveryDatabase_Should_RejectNullConnection_When_ConnectionOverloadIsUsed()
    {
        var builder = new DbContextOptionsBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.ConfigureRecoveryDatabase(
            DbProviders.MSSQL, (System.Data.Common.DbConnection)null!, "Recovery.Migrations", false));
    }

    [Fact]
    public void ConfigureRecoveryDatabase_Should_RejectUnsupportedProvider_When_ConnectionIsProvided()
    {
        using var connection = new SqlConnection(ConnectionString);
        var builder = new DbContextOptionsBuilder();

        Assert.Throws<InvalidOperationException>(() => builder.ConfigureRecoveryDatabase(
            "POSTGRESQL", connection, "Recovery.Migrations", false));
    }

    [Fact]
    public void ConfigureRecoveryDatabase_Should_RejectProvider_When_ProviderIsUnsupported()
    {
        var builder = new DbContextOptionsBuilder();

        Assert.Throws<InvalidOperationException>(() => builder.ConfigureRecoveryDatabase(
            "POSTGRESQL", ConnectionString, "Recovery.Migrations", false));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ConfigureRecoveryDatabase_Should_RejectProvider_When_ProviderIsEmpty(string provider)
    {
        var builder = new DbContextOptionsBuilder();

        Assert.Throws<ArgumentException>(() => builder.ConfigureRecoveryDatabase(
            provider, ConnectionString, "Recovery.Migrations", false));
    }
}
