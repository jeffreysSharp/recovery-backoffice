using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using IdentityContext = Recovery.BackOffice.Identity.Data.IdentityDbContext;

namespace Recovery.BackOffice.Api.DesignTime;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var settings = DesignTimeConfiguration.Read(args);
        var options = new DbContextOptionsBuilder<IdentityContext>();
        options.ConfigureRecoveryDatabase(settings.Provider, settings.ConnectionString,
            settings.MigrationsAssembly, isDevelopment: false);
        var accessor = new AsyncLocalMultiTenantContextAccessor<AppTenantInfo>();
        // Schema generation needs tenant metadata, not a runtime fallback or a seeded tenant.
        ((IMultiTenantContextSetter)accessor).MultiTenantContext = new MultiTenantContext<AppTenantInfo>(
            new AppTenantInfo(MultitenancyConstants.Root.Id, MultitenancyConstants.Root.Id));
        return new IdentityContext(accessor, options.Options, Options.Create(settings),
            Host.CreateApplicationBuilder().Environment);
    }
}
