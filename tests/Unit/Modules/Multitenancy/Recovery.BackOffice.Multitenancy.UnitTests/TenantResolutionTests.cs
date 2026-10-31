using System.Security.Claims;
using System.Text.Encodings.Web;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Core.Context;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Constants;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Multitenancy.Data;
using Xunit;

namespace Recovery.BackOffice.Multitenancy.UnitTests;

public sealed class TenantResolutionTests
{
    [Theory]
    [InlineData("company-a", "company-b", "company-b", "company-a")]
    [InlineData(null, "company-a", "company-b", "company-a")]
    [InlineData(null, null, "company-b", "company-b")]
    [InlineData(null, "", "company-b", "company-b")]
    [InlineData(null, null, null, null)]
    [InlineData(null, "unknown-company", null, null)]
    [InlineData(null, null, "unknown-company", null)]
    public async Task ResolveAsync_Should_RespectStrategyPrecedence_When_IdentifiersAreProvided(
        string? authenticatedTenant, string? header, string? query, string? expectedTenant)
    {
        using var host = CreateHost(useTestStore: true);
        using var scope = host.Services.CreateScope();
        var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        if (authenticatedTenant is not null)
        {
            context.Request.Headers["X-Test-Authenticated-Tenant"] = authenticatedTenant;
        }
        if (header is not null) context.Request.Headers["tenant"] = header;
        if (query is not null) context.Request.QueryString = new QueryString($"?tenant={query}");
        var resolver = scope.ServiceProvider.GetRequiredService<ITenantResolver<AppTenantInfo>>();

        var resolved = await resolver.ResolveAsync(context);

        Assert.Equal(expectedTenant, resolved.TenantInfo?.Id);
        Assert.Equal(expectedTenant is not null, resolved.IsResolved);
    }

    [Fact]
    public async Task ResolveAsync_Should_KeepRequestsIndependent_When_TenantsDiffer()
    {
        using var host = CreateHost(useTestStore: true);

        foreach (var tenant in new[] { "company-a", "company-b", "company-a" })
        {
            using var scope = host.Services.CreateScope();
            var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
            context.Request.Headers["tenant"] = tenant;
            var resolver = scope.ServiceProvider.GetRequiredService<ITenantResolver<AppTenantInfo>>();

            var resolved = await resolver.ResolveAsync(context);

            Assert.Equal(tenant, resolved.TenantInfo?.Id);
        }
    }

    [Fact]
    public void ConfigureServices_Should_RegisterScopedEfStore_When_TestStoreIsNotInstalled()
    {
        using var host = CreateHost(useTestStore: false);
        using var firstScope = host.Services.CreateScope();
        using var secondScope = host.Services.CreateScope();

        var first = firstScope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
        var same = firstScope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
        var other = secondScope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();

        Assert.IsType<EFCoreStore<TenantDbContext, AppTenantInfo>>(first);
        Assert.Same(first, same);
        Assert.NotSame(first, other);
        var context = firstScope.ServiceProvider.GetRequiredService<TenantDbContext>();
        Assert.Equal(System.Data.ConnectionState.Closed, context.Database.GetDbConnection().State);
    }

    private static IHost CreateHost(bool useTestStore)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabaseOptions:Provider"] = "MSSQL",
            ["DatabaseOptions:ConnectionString"] = "Server=localhost;Database=ResolutionTests;Integrated Security=True",
            ["DatabaseOptions:MigrationsAssembly"] = typeof(TenantResolutionTests).Assembly.GetName().Name
        });
        builder.Services.AddRecoveryDatabaseOptions(builder.Configuration);
        builder.Services.AddSingleton<ICurrentUser>(new AnonymousUser());
        builder.Services.AddAuthentication("TestTenant")
            .AddScheme<AuthenticationSchemeOptions, TestTenantHandler>("TestTenant", _ => { });
        new MultitenancyModule().ConfigureServices(builder);
        if (useTestStore)
        {
            // Only these tests replace SQL persistence; production keeps the EF Core store.
            builder.Services.RemoveAll<IMultiTenantStore<AppTenantInfo>>();
            builder.Services.AddMultiTenant<AppTenantInfo>().WithInMemoryStore(options =>
            {
                options.Tenants.Add(new AppTenantInfo("company-a", "company-a", "Company A"));
                options.Tenants.Add(new AppTenantInfo("company-b", "company-b", "Company B"));
            });
        }

        return builder.Build();
    }

    private sealed class TestTenantHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var tenant = Request.Headers["X-Test-Authenticated-Tenant"].ToString();
            if (string.IsNullOrEmpty(tenant)) return Task.FromResult(AuthenticateResult.NoResult());

            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(CustomClaims.Tenant, tenant)], Scheme.Name));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
        }
    }

    private sealed class AnonymousUser : ICurrentUser
    {
        public string? Name => null;
        public Guid GetUserId() => Guid.Empty;
        public string? GetUserEmail() => null;
        public string? GetTenant() => null;
        public bool IsAuthenticated() => false;
        public bool IsInRole(string role) => false;
        public IEnumerable<Claim> GetUserClaims() => [];
    }
}
