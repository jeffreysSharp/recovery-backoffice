using Finbuckle.MultiTenant.EntityFrameworkCore.Stores;
using Microsoft.EntityFrameworkCore;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;

namespace Recovery.BackOffice.Multitenancy.Data;

public class TenantDbContext(DbContextOptions<TenantDbContext> options)
    : EFCoreStoreDbContext<AppTenantInfo>(options)
{
    public const string Schema = "tenant";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
    }
}
