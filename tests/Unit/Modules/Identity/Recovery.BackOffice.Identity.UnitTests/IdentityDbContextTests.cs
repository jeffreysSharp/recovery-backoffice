using System.Data;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Identity.Domain;
using Xunit;
using IdentityContext = Recovery.BackOffice.Identity.Data.IdentityDbContext;

namespace Recovery.BackOffice.Identity.UnitTests;

public sealed class IdentityDbContextTests
{
    private const string DefaultConnection = "Server=localhost;Database=IdentityDefault;Integrated Security=True";
    private const string TenantConnection = "Server=localhost;Database=IdentityTenant;Integrated Security=True";

    public static TheoryData<Type, string> TenantEntities => new()
    {
        { typeof(RecoveryUser), "Users" },
        { typeof(RecoveryRole), "Roles" },
        { typeof(RecoveryRoleClaim), "RoleClaims" },
        { typeof(IdentityUserClaim<string>), "UserClaims" },
        { typeof(IdentityUserRole<string>), "UserRoles" },
        { typeof(IdentityUserLogin<string>), "UserLogins" },
        { typeof(IdentityUserToken<string>), "UserTokens" },
        { typeof(PasswordHistory), "PasswordHistory" }
    };

    [Theory]
    [MemberData(nameof(TenantEntities))]
    public void Model_Should_MapTenantEntity_When_IdentityModelIsBuilt(Type type, string table)
    {
        using var context = CreateContext("company-a");

        var entity = context.Model.FindEntityType(type)!;

        Assert.Equal(table, entity.GetTableName());
        Assert.Equal("identity", entity.GetSchema());
        Assert.NotNull(entity.FindProperty("TenantId"));
    }

    [Fact]
    public void Query_Should_UseCurrentTenant_When_UserModelsAreCached()
    {
        using var first = CreateContext("company-a");
        using var second = CreateContext("company-b");

        var firstSql = first.Users.ToQueryString();
        var secondSql = second.Users.ToQueryString();

        Assert.Same(first.Model, second.Model);
        Assert.Contains("[TenantId] =", firstSql);
        Assert.Contains("company-a", firstSql);
        Assert.DoesNotContain("company-b", firstSql);
        Assert.Contains("[TenantId] =", secondSql);
        Assert.Contains("company-b", secondSql);
        Assert.DoesNotContain("company-a", secondSql);
    }

    [Fact]
    public void Model_Should_ScopeUniqueNamesByTenant_When_IdentityIndexesAreCreated()
    {
        using var context = CreateContext("company-a");

        foreach (var type in new[] { typeof(RecoveryUser), typeof(RecoveryRole) })
        {
            var index = Assert.Single(context.Model.FindEntityType(type)!.GetIndexes(), candidate => candidate.IsUnique);
            Assert.Contains(index.Properties, property => property.Name == "TenantId");
        }
    }

    [Fact]
    public void Model_Should_ExcludeDomainEvents_When_SqlServerScriptIsGenerated()
    {
        using var context = CreateContext("company-a");

        var script = context.Database.GenerateCreateScript();

        Assert.Contains("CREATE TABLE [identity].[Users]", script);
        Assert.Contains("CREATE TABLE [identity].[PasswordHistory]", script);
        Assert.DoesNotContain("DomainEvents", script);
        Assert.Null(context.Model.FindEntityType(typeof(RecoveryUser))!.FindNavigation("DomainEvents"));
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Fact]
    public void Query_Should_FilterPasswordHistory_When_LookingUpPriorHashes()
    {
        using var context = CreateContext("company-a");

        var sql = context.PasswordHistories.ToQueryString();

        Assert.Contains("[identity].[PasswordHistory]", sql);
        Assert.Contains("[TenantId] =", sql);
        Assert.Contains("company-a", sql);
        var user = context.Model.FindEntityType(typeof(RecoveryUser))!;
        Assert.Equal(10, user.FindProperty(nameof(RecoveryUser.Locale))!.GetMaxLength());
        Assert.Equal(256, user.FindProperty(nameof(RecoveryUser.ObjectId))!.GetMaxLength());
    }

    [Theory]
    [InlineData(null, "IdentityDefault")]
    [InlineData(" ", "IdentityDefault")]
    [InlineData(TenantConnection, "IdentityTenant")]
    public void OnConfiguring_Should_SelectDatabase_When_TenantConnectionIsOptional(string? connection, string database)
    {
        using var context = CreateContext("company-a", connection);

        Assert.Equal(database, context.Database.GetDbConnection().Database);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Fact]
    public void OnConfiguring_Should_ReuseScopedConnection_When_TenantUsesDedicatedDatabase()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = "MSSQL",
            ["DatabaseOptions:ConnectionString"] = DefaultConnection,
            ["DatabaseOptions:MigrationsAssembly"] = typeof(IdentityDbContextTests).Assembly.GetName().Name
        });
        new IdentityModule().ConfigureServices(builder);
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        builder.Services.AddScoped(_ => CreateAccessor("company-a", TenantConnection));
        builder.Services.AddRecoveryDbContext<IdentityContext>();
        using var host = builder.Build();
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var provider = scope.ServiceProvider.GetRequiredService<IScopedDbConnectionProvider>();

        var connection = context.Database.GetDbConnection();

        Assert.Same(provider.GetConnection("MSSQL", TenantConnection), connection);
        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    private static IdentityContext CreateContext(string tenantId, string? connection = null) => new(
        CreateAccessor(tenantId, connection),
        new DbContextOptionsBuilder<IdentityContext>().UseSqlServer(DefaultConnection).Options,
        Options.Create(new DatabaseOptions
        {
            Provider = "MSSQL",
            MigrationsAssembly = typeof(IdentityDbContextTests).Assembly.GetName().Name!
        }),
        Host.CreateApplicationBuilder().Environment);

    private static IMultiTenantContextAccessor<AppTenantInfo> CreateAccessor(string tenantId, string? connection)
    {
        var accessor = new AsyncLocalMultiTenantContextAccessor<AppTenantInfo>();
        ((IMultiTenantContextSetter)accessor).MultiTenantContext = new MultiTenantContext<AppTenantInfo>(
            new AppTenantInfo(tenantId, tenantId) { ConnectionString = connection ?? string.Empty });
        return accessor;
    }
}
