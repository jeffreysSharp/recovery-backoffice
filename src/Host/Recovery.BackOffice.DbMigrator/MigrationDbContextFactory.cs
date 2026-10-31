using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.BuildingBlocks.Shared.Persistence;
using Recovery.BackOffice.Multitenancy.Data;
using IdentityContext = Recovery.BackOffice.Identity.Data.IdentityDbContext;

namespace Recovery.BackOffice.DbMigrator;

public sealed class MigrationDbContextFactory(DatabaseOptions settings, IHostEnvironment environment)
{
    public TenantDbContext CreateCatalog()
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>();
        options.ConfigureRecoveryDatabase(settings.Provider, settings.ConnectionString,
            settings.MigrationsAssembly, environment.IsDevelopment());
        return new TenantDbContext(options.Options);
    }

    public IdentityContext CreateIdentity(AppTenantInfo tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        ArgumentException.ThrowIfNullOrWhiteSpace(tenant.Id);
        var options = new DbContextOptionsBuilder<IdentityContext>();
        options.ConfigureRecoveryDatabase(settings.Provider, settings.ConnectionString,
            settings.MigrationsAssembly, environment.IsDevelopment());
        var accessor = new AsyncLocalMultiTenantContextAccessor<AppTenantInfo>();
        ((IMultiTenantContextSetter)accessor).MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenant);
        return new IdentityContext(accessor, options.Options, Options.Create(settings), environment);
    }
}
