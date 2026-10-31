using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Recovery.BackOffice.BuildingBlocks.Persistence;
using Recovery.BackOffice.Multitenancy.Data;

namespace Recovery.BackOffice.Api.DesignTime;

public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var settings = DesignTimeConfiguration.Read(args);
        var options = new DbContextOptionsBuilder<TenantDbContext>();
        options.ConfigureRecoveryDatabase(settings.Provider, settings.ConnectionString,
            settings.MigrationsAssembly, isDevelopment: false);
        return new TenantDbContext(options.Options);
    }
}
