using System.Data;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recovery.BackOffice.Api.DesignTime;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Identity;
using Recovery.BackOffice.Migrations.SqlServer;
using Xunit;

namespace Recovery.BackOffice.Migrations.UnitTests;

public sealed class MigrationModelTests
{
    [Fact]
    public void Snapshot_Should_MatchRuntimeIdentityModel_When_RealIdentityServicesAreRegistered()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = "MSSQL",
            ["DatabaseOptions:ConnectionString"] = "Server=localhost;Database=RuntimeModel;Integrated Security=True",
            ["DatabaseOptions:MigrationsAssembly"] = typeof(MigrationsAssemblyMarker).Assembly.GetName().Name
        });
        new IdentityModule().ConfigureServices(builder);
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        builder.Services.AddRecoveryIdentity();
        builder.Services.AddScoped<IMultiTenantContextAccessor<AppTenantInfo>>(_ =>
        {
            var accessor = new AsyncLocalMultiTenantContextAccessor<AppTenantInfo>();
            ((IMultiTenantContextSetter)accessor).MultiTenantContext = new MultiTenantContext<AppTenantInfo>(
                new AppTenantInfo("company-a", "company-a"));
            return accessor;
        });
        using var host = builder.Build();
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Recovery.BackOffice.Identity.Data.IdentityDbContext>();

        Assert.False(context.Database.HasPendingModelChanges());
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Theory]
    [InlineData("Identity")]
    [InlineData("Multitenancy")]
    public void Snapshot_Should_MatchCurrentModel_When_ContextIsCreated(string contextName)
    {
        using var context = CreateContext(contextName);

        Assert.Single(context.Database.GetMigrations());
        Assert.False(context.Database.HasPendingModelChanges());
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Theory]
    [InlineData("Identity", "identity", "Users")]
    [InlineData("Multitenancy", "tenant", "Tenants")]
    public void Migrations_Should_GenerateIdempotentUpAndDownScripts_When_SqlServerIsSelected(
        string contextName, string schema, string table)
    {
        using var context = CreateContext(contextName);
        var migrator = context.GetService<IMigrator>();
        var migration = Assert.Single(context.Database.GetMigrations());

        var up = migrator.GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent);
        var down = migrator.GenerateScript(migration, "0", MigrationsSqlGenerationOptions.Idempotent);

        Assert.Contains($"CREATE TABLE [{schema}].[{table}]", up);
        Assert.Contains("IF NOT EXISTS", up);
        Assert.Contains("__EFMigrationsHistory", up);
        Assert.DoesNotContain("jsonb", up);
        Assert.DoesNotContain("Fsh", up);
        Assert.Contains($"DROP TABLE [{schema}].[{table}]", down);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Theory]
    [InlineData("Identity")]
    [InlineData("Multitenancy")]
    public void Factory_Should_UseExplicitConnection_When_DesignTimeArgumentsAreProvided(string contextName)
    {
        using var context = CreateContext(contextName,
            ["--DatabaseOptions:Provider", "MSSQL", "--DatabaseOptions:ConnectionString",
                "Server=localhost;Database=ConfiguredDesignTime;Integrated Security=True"]);

        Assert.Equal("ConfiguredDesignTime", context.Database.GetDbConnection().Database);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Theory]
    [InlineData("Identity")]
    [InlineData("Multitenancy")]
    public void Factory_Should_RejectUnsupportedProvider_When_ConfigurationRequestsAnotherDatabase(string contextName)
    {
        Assert.Throws<NotSupportedException>(() => CreateContext(contextName,
            ["--DatabaseOptions:Provider", "POSTGRESQL"]));
    }

    private static DbContext CreateContext(string contextName, string[]? args = null) => contextName switch
    {
        "Identity" => new IdentityDbContextFactory().CreateDbContext(args ?? ["--DatabaseOptions:Provider", "MSSQL"]),
        "Multitenancy" => new TenantDbContextFactory().CreateDbContext(args ?? ["--DatabaseOptions:Provider", "MSSQL"]),
        _ => throw new ArgumentOutOfRangeException(nameof(contextName))
    };
}
