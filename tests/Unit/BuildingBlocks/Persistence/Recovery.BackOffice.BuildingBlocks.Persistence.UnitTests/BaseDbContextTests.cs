using System.Data;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Core.Domain;
using Recovery.BackOffice.BuildingBlocks.Persistence.Context;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Xunit;

namespace Recovery.BackOffice.BuildingBlocks.Persistence.UnitTests;

public sealed class BaseDbContextTests
{
    private const string DefaultConnection = "Server=localhost;Database=DefaultTests;Integrated Security=True";
    private const string TenantConnection = "Server=localhost;Database=TenantTests;Integrated Security=True";

    [Fact]
    public void Query_Should_CombineTenantAndSoftDeleteFilters_When_EntitySupportsSoftDeletion()
    {
        using var context = CreateContext();

        var sql = context.Set<Document>().ToQueryString();

        Assert.Contains("WHERE", sql);
        Assert.Contains("[TenantId] =", sql);
        Assert.Contains("company-a", sql);
        Assert.Contains("[IsDeleted] = CAST(0 AS bit)", sql);
    }

    [Fact]
    public void Query_Should_KeepTenantFilter_When_OnlySoftDeletionIsIgnored()
    {
        using var context = CreateContext();

        var sql = context.Set<Document>().IgnoreQueryFilters([QueryFilters.SoftDelete]).ToQueryString();

        Assert.Contains("[TenantId] =", sql);
        Assert.Contains("company-a", sql);
        Assert.DoesNotContain("[IsDeleted] = CAST(0 AS bit)", sql);
    }

    [Fact]
    public void Query_Should_FilterGlobalSoftDeletedEntities_When_EntityOptsOutOfTenantIsolation()
    {
        using var context = CreateContext();

        var sql = context.Set<GlobalDocument>().ToQueryString();

        Assert.DoesNotContain("TenantId", sql);
        Assert.Contains("[IsDeleted] = CAST(0 AS bit)", sql);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void OnConfiguring_Should_KeepDefaultConnection_When_TenantHasNoDedicatedConnection(string? connection)
    {
        using var context = CreateContext(connection);

        Assert.Equal("DefaultTests", context.Database.GetDbConnection().Database);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Fact]
    public void OnConfiguring_Should_UseDedicatedConnection_When_ContextIsConstructedDirectly()
    {
        using var context = CreateContext(TenantConnection);

        Assert.Equal("TenantTests", context.Database.GetDbConnection().Database);
        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
        Assert.True(context.Database.CreateExecutionStrategy().RetriesOnFailure);
        Assert.Equal(ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    [Fact]
    public void OnConfiguring_Should_ShareDedicatedConnection_When_ContextsUseTheSameScope()
    {
        using var host = CreateHost();
        using var scope = host.Services.CreateScope();
        var first = scope.ServiceProvider.GetRequiredService<FirstContext>();
        var second = scope.ServiceProvider.GetRequiredService<SecondContext>();
        var provider = scope.ServiceProvider.GetRequiredService<IScopedDbConnectionProvider>();

        var connection = first.Database.GetDbConnection();

        Assert.Same(connection, second.Database.GetDbConnection());
        Assert.Same(connection, provider.GetConnection("MSSQL", TenantConnection));
        Assert.Equal(TenantConnection, connection.ConnectionString);
        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    [Fact]
    public void OnConfiguring_Should_IsolateDedicatedConnections_When_ScopesDiffer()
    {
        using var host = CreateHost();
        using var firstScope = host.Services.CreateScope();
        using var secondScope = host.Services.CreateScope();
        var first = firstScope.ServiceProvider.GetRequiredService<FirstContext>();
        var second = secondScope.ServiceProvider.GetRequiredService<FirstContext>();

        Assert.NotSame(first.Database.GetDbConnection(), second.Database.GetDbConnection());
    }

    [Fact]
    public void Dispose_Should_KeepSharedConnectionAlive_When_OnlyContextIsDisposed()
    {
        using var host = CreateHost();
        var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FirstContext>();
        var connection = context.Database.GetDbConnection();
        var disposalCount = 0;
        connection.Disposed += (_, _) => disposalCount++;

        context.Dispose();
        Assert.Equal(0, disposalCount);
        scope.Dispose();

        Assert.Equal(1, disposalCount);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_AssignCurrentTenant_When_NewEntityHasNoTenant()
    {
        using var context = CreateContext();
        var document = new Document { Id = 1 };
        context.Add(document);

        await context.SaveChangesAsync();

        Assert.Equal("company-a", context.Entry(document).Property<string>("TenantId").CurrentValue);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_RejectForeignTenant_When_TrackedEntityTenantIsChanged()
    {
        using var context = CreateContext();
        var document = new Document { Id = 1 };
        context.Add(document);
        context.Entry(document).Property<string>("TenantId").CurrentValue = "company-b";

        await Assert.ThrowsAsync<MultiTenantException>(() => context.SaveChangesAsync());
    }

    private static FirstContext CreateContext(string? tenantConnection = null)
    {
        var options = new DbContextOptionsBuilder<FirstContext>()
            .UseSqlServer(DefaultConnection)
            .AddInterceptors(new SuppressSaveInterceptor()).Options;
        var environment = Host.CreateApplicationBuilder().Environment;
        return new FirstContext(CreateAccessor(tenantConnection), options,
            Options.Create(new DatabaseOptions
            {
                Provider = "MSSQL",
                MigrationsAssembly = typeof(BaseDbContextTests).Assembly.GetName().Name!
            }), environment);
    }

    private static IMultiTenantContextAccessor<AppTenantInfo> CreateAccessor(string? connection)
    {
        var accessor = new AsyncLocalMultiTenantContextAccessor<AppTenantInfo>();
        ((IMultiTenantContextSetter)accessor).MultiTenantContext = new MultiTenantContext<AppTenantInfo>(
            new AppTenantInfo("company-a", "company-a", "Company A")
            {
                ConnectionString = connection ?? string.Empty
            });
        return accessor;
    }

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = "MSSQL",
            ["DatabaseOptions:ConnectionString"] = DefaultConnection,
            ["DatabaseOptions:MigrationsAssembly"] = typeof(BaseDbContextTests).Assembly.GetName().Name
        });
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        builder.Services.AddSingleton<ICurrentUser>(new TestCurrentUser());
        builder.Services.AddScoped(_ => CreateAccessor(TenantConnection));
        builder.Services.AddRecoveryDbContext<FirstContext>();
        builder.Services.AddRecoveryDbContext<SecondContext>();
        return builder.Build();
    }

    public sealed class FirstContext(
        IMultiTenantContextAccessor<AppTenantInfo> accessor,
        DbContextOptions<FirstContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment) : BaseDbContext(accessor, options, settings, environment)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>().HasKey(entity => entity.Id);
            modelBuilder.Entity<GlobalDocument>().HasKey(entity => entity.Id);
            base.OnModelCreating(modelBuilder);
        }
    }

    public sealed class SecondContext(
        IMultiTenantContextAccessor<AppTenantInfo> accessor,
        DbContextOptions<SecondContext> options,
        IOptions<DatabaseOptions> settings,
        IHostEnvironment environment) : BaseDbContext(accessor, options, settings, environment);

    private sealed class SuppressSaveInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(InterceptionResult<int>.SuppressWithResult(0));
    }

    private sealed class Document : ISoftDeletable
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOnUtc { get; set; }
        public string? DeletedBy { get; set; }
    }

    private sealed class GlobalDocument : ISoftDeletable, IGlobalEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedOnUtc { get; set; }
        public string? DeletedBy { get; set; }
    }
}
