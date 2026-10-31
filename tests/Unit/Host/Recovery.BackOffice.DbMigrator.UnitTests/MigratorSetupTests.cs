using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Migrations.SqlServer;
using Xunit;

namespace Recovery.BackOffice.DbMigrator.UnitTests;

public sealed class MigratorSetupTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("--help")]
    [InlineData("-h")]
    public void Parse_Should_PrintHelpWithoutConfiguration_When_HelpOrNoCommandIsProvided(string? argument)
    {
        var command = MigratorCommand.Parse(argument is null ? [] : [argument]);

        Assert.True(command.Help);
        Assert.Empty(command.ConfigurationArgs);
    }

    [Theory]
    [InlineData("drop")]
    [InlineData("seed")]
    [InlineData("list-pending")]
    public void Parse_Should_RejectUnsupportedCommand_When_UnknownOperationIsRequested(string command)
    {
        Assert.Throws<ArgumentException>(() => MigratorCommand.Parse([command]));
    }

    [Fact]
    public void Parse_Should_RemoveVerbFromConfiguration_When_ApplyHasArguments()
    {
        var command = MigratorCommand.Parse(["apply", "--DatabaseOptions:Provider", "MSSQL"]);

        Assert.False(command.Help);
        Assert.Equal(["--DatabaseOptions:Provider", "MSSQL"], command.ConfigurationArgs);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Read_Should_RejectMissingConnection_When_ConfigurationIsIncomplete(string? connection)
    {
        var configuration = Configure(connection);

        Assert.Throws<InvalidOperationException>(() => MigratorConfiguration.Read(configuration));
    }

    [Fact]
    public void Read_Should_RejectUnsupportedProvider_When_ConfiguredForPostgresql()
    {
        var configuration = Configure("Server=localhost;Database=MigrationTests;Integrated Security=True", "POSTGRESQL");

        Assert.Throws<NotSupportedException>(() => MigratorConfiguration.Read(configuration));
    }

    [Fact]
    public void CreateContexts_Should_SelectCatalogAndTenantDatabases_When_TenantHasDedicatedConnection()
    {
        var settings = MigratorConfiguration.Read(Configure(
            "Server=localhost;Database=CatalogTests;Integrated Security=True"));
        var factory = new MigrationDbContextFactory(settings, Host.CreateApplicationBuilder().Environment);
        using var catalog = factory.CreateCatalog();
        using var first = factory.CreateIdentity(new AppTenantInfo("root", "root"));
        using var second = factory.CreateIdentity(new AppTenantInfo("company-a", "company-a")
        {
            ConnectionString = "Server=localhost;Database=CompanyTests;Integrated Security=True"
        });

        Assert.Equal(typeof(MigrationsAssemblyMarker).Assembly.GetName().Name, settings.MigrationsAssembly);
        Assert.Equal("CatalogTests", catalog.Database.GetDbConnection().Database);
        Assert.Equal("CatalogTests", first.Database.GetDbConnection().Database);
        Assert.Equal("CompanyTests", second.Database.GetDbConnection().Database);
        Assert.Contains("root", first.Users.ToQueryString());
        Assert.Contains("company-a", second.Users.ToQueryString());
        Assert.False(catalog.Database.HasPendingModelChanges());
        Assert.False(first.Database.HasPendingModelChanges());
        Assert.False(second.Database.HasPendingModelChanges());
        Assert.Equal(ConnectionState.Closed, catalog.Database.GetDbConnection().State);
        Assert.Equal(ConnectionState.Closed, first.Database.GetDbConnection().State);
        Assert.Equal(ConnectionState.Closed, second.Database.GetDbConnection().State);
    }

    private static IConfiguration Configure(string? connection, string provider = "MSSQL") =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = provider,
            ["DatabaseOptions:ConnectionString"] = connection
        }).Build();
}
